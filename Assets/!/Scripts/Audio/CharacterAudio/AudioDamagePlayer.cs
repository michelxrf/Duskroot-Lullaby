using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioDamagePlayer : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference damageEvent;

    enum ImpactType
    {
        Melee = 0,
        Fly = 1,
        Magic = 2,
        Projectile = 3
    }

    private ImpactType _impactType = 0;
    private AudioHitNotifier hitNotifier;

    void Awake()
    {
        hitNotifier = GetComponent<AudioHitNotifier>();

        if (hitNotifier != null)
        {
            hitNotifier.OnHit += OnPlayerHit;
        }
    }

    private void OnPlayerHit(CHARACTERTYPES character, WEAPONTYPES weapon)
    {
        switch (weapon)
        {
            case WEAPONTYPES.Club:
            case WEAPONTYPES.Blade:
            case WEAPONTYPES.Unarmed:
                _impactType = ImpactType.Melee;
                break;

            case WEAPONTYPES.Fly:
                _impactType = ImpactType.Fly;
                break;

            case WEAPONTYPES.Magic:
                _impactType = ImpactType.Magic;
                break;

            case WEAPONTYPES.Projectile:
                _impactType = ImpactType.Projectile;
                break;

            default:
                _impactType = ImpactType.Melee;
                break;
        }
    }
    public void PlayDamage()
    {
        EventInstance punchInstance = RuntimeManager.CreateInstance(damageEvent);
        punchInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        punchInstance.setParameterByName("Impact_type", (int)_impactType);
        punchInstance.start();
        punchInstance.release();
    }

    private void OnDestroy()
    {
        if (hitNotifier != null)
        {
            hitNotifier.OnHit -= OnPlayerHit;
        }
    }
}
