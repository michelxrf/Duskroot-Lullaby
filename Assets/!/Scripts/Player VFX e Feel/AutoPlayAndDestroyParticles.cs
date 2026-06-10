using UnityEngine;

public class AutoPlayAndDestroyParticles : MonoBehaviour
{
    private void Start()
    {
        float maxLifetime = 0f;

        ParticleSystem[] particles =
            GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particle in particles)
        {
            particle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);

            particle.Play();

            float duration =
                particle.main.duration +
                particle.main.startLifetime.constantMax;

            if (duration > maxLifetime)
                maxLifetime = duration;
        }

        Destroy(gameObject, maxLifetime + 0.5f);
    }
}