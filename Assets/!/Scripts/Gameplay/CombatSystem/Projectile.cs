using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;
using Fusion.Addons.SimpleKCC;
using CombatSystem;
using Unity.VisualScripting;
using FMOD.Studio;
using FMODUnity;


public class Projectile : NetworkBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float range = 10f;

    [Header("VFX")]
    [SerializeField] GameObject explosionVfx;
    //[SerializeField] private GameObject trailVfxPrefab;
    //private GameObject spawnedTrail;
    [Header("FMOD")]
    [SerializeField] private EventReference SFX_Explosion;

    GameObject owner;
    WeaponDataInstance weaponDataInstance;
    SimpleKCC characterController;
    Collider hitboxCollider;
    Vector3 startingPos;
    float distanceTraveled;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider>();
        hitboxCollider.enabled = false;
    }

    public override void Spawned()
    {
        characterController = GetComponent<SimpleKCC>();
        characterController.SetGravity(0); // Projectiles should not be affected by gravity
        hitboxCollider = GetComponent<Collider>();
        hitboxCollider.enabled = false;
        startingPos = transform.position;
    }

    public void SetUp(Vector3 direction, WeaponDataInstance weaponData, GameObject owner)
    {
        transform.forward = direction;
        hitboxCollider.enabled = true;
        this.weaponDataInstance = weaponData;
        this.owner = owner;

    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        characterController.Move(speed * transform.forward);

        distanceTraveled = Vector3.Distance(startingPos, transform.position);
        if (distanceTraveled >= range)
        {
            CombatFuncs.CastHitBox(transform, owner, weaponDataInstance);
            RunnerBootstrap.Instance.Runner.Despawn(Object);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority)
            return;

        if (other.gameObject == owner)
            return;

        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            if (health.IsDead())
                return;

            CombatFuncs.CastHitBox(other.transform, owner, weaponDataInstance);
        }

        RunnerBootstrap.Instance.Runner.Despawn(Object);
    }

    public override void Despawned(NetworkRunner runner, bool hasGracefulExit)
    {
        base.Despawned(runner, hasGracefulExit);

        if (explosionVfx != null)
        {
            Instantiate(explosionVfx, transform.position, Quaternion.identity);
        }

        EventInstance audioExplosion = RuntimeManager.CreateInstance(SFX_Explosion);
        audioExplosion.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        audioExplosion.start();
        audioExplosion.release();
    }

}
