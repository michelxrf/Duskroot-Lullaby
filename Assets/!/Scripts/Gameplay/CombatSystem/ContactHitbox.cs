using CombatSystem;
using UnityEngine;
using Fusion;

/// <summary>
/// Automatically hurts anything that enters it`s collider, used by enemies
/// </summary>
[RequireComponent(typeof(Collider))]
public class ContactHitbox : NetworkBehaviour
{
    WeaponDataInstance weaponDataInstance;
    GameObject owner;
    Collider hitboxCollider;

    public override void Spawned()
    {
        hitboxCollider = GetComponent<Collider>();
        hitboxCollider.enabled = false; // Start disabled, will be enabled when Setup is called

        EnemySetup enemySetup = transform.parent.GetComponent<EnemySetup>();

        if (enemySetup.IsInitialized())
            Setup(enemySetup.GetEnemyData().weapon, transform.parent.gameObject);
        else
            enemySetup.OnInit += () => Setup(enemySetup.GetEnemyData().weapon, transform.parent.gameObject);
    }

    public void Setup(WeaponData weaponData, GameObject owner)
    {
        this.weaponDataInstance = new WeaponDataInstance(weaponData, 0, "1");
        this.owner = owner;
        hitboxCollider = GetComponent<Collider>();
        hitboxCollider.enabled = true;
    }

    public void Disable()
    {
        hitboxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            CombatFuncs.CastHitBox(other.transform, owner, weaponDataInstance);
        }
    }
}
