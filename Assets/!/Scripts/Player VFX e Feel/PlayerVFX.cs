using CombatSystem;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [Header("General")]
    [SerializeField] GameObject healVFX;
    [SerializeField] GameObject dashVFX;
    [SerializeField] GameObject hitVFX;
    [SerializeField] GameObject deathVFX;

    public void Play(PlayerVFXEvent vfxEvent)
    {
        switch (vfxEvent)
        {
            case PlayerVFXEvent.Heal:
                PlayParticles(healVFX);
                break;

            case PlayerVFXEvent.Dash:
                PlayParticles(dashVFX);
                break;

            case PlayerVFXEvent.Hit:
                PlayParticles(hitVFX);
                break;

            case PlayerVFXEvent.Death:
                PlayParticles(deathVFX);
                break;
        }
    }

    private void PlayParticles(GameObject vfxRoot)
    {
        if (vfxRoot == null)
            return;

        ParticleSystem[] particles =
            vfxRoot.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particle in particles)
        {
            particle.Stop(true,
                ParticleSystemStopBehavior.StopEmittingAndClear);

            particle.Play();
        }
    }
}
