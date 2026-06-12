using CombatSystem;
using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFeedbacks : MonoBehaviour
{
    [Header("Alert Icons")]
    [SerializeField] GameObject alertCanvas;
    [SerializeField] GameObject lostTargetCanvas;
    [SerializeField] float alertDuration = 0.5f;


    [Header("Feedbacks")]
    [SerializeField] MMF_Player alertFeedback;
    [SerializeField] MMF_Player lostTargetFeedback;
    [SerializeField] MMF_Player hitFeedback;
    [SerializeField] MMF_Player deathFeedback;
    [SerializeField] MMF_Player spawnFeedback;
    [SerializeField] MMF_Player attackFeedback;
    [SerializeField] MMF_Player startWalkFeedback;
    [SerializeField] MMF_Player walkingFeedback;

    Coroutine alertRoutine;
    Coroutine lostTargetRoutine;

    Health health;

    private void Start()
    {
        health = GetComponent<Health>();

        if (health != null)
        {
            health.OnHit += PlayHit;
            health.OnDied += PlayDeath;
        }
    }

    public void Play(EnemyFeedbackEvent feedback)
    {
        switch (feedback)
        {
            case EnemyFeedbackEvent.Alert:
                PlayAlert();
                break;

            case EnemyFeedbackEvent.LostTarget:
                PlayLostTarget();
                break;

            case EnemyFeedbackEvent.Hit:
                PlayHit();
                break;

            case EnemyFeedbackEvent.Death:
                PlayDeath();
                break;

            case EnemyFeedbackEvent.Spawn:
                PlaySpawn();
                break;

            case EnemyFeedbackEvent.Atack:
                PlayAtack();
                break;
            case EnemyFeedbackEvent.StartWalk:
                PlayStartWalk();
                break;
            case EnemyFeedbackEvent.Walking:
                PlayWalking();
                break;
        }
    }
    public void PlayAlert()
    {
        //alertFeedback?.PlayFeedbacks();
        if (alertCanvas == null)
            return;

        if (alertRoutine != null)
            StopCoroutine(alertRoutine);

        alertRoutine = StartCoroutine(AlertRoutine());
    }

    IEnumerator AlertRoutine()
    {
        alertCanvas.SetActive(true);
        yield return new WaitForSeconds(alertDuration);
        alertCanvas.SetActive(false);
        alertRoutine = null;
    }

    public void PlayLostTarget()
    {
        //lostTargetFeedback?.PlayFeedbacks();
        if (lostTargetCanvas == null)
            return;

        if (lostTargetRoutine != null)
            StopCoroutine(lostTargetRoutine);

        lostTargetRoutine = StartCoroutine(LostTargetRoutine());
    }

    IEnumerator LostTargetRoutine()
    {
        lostTargetCanvas.SetActive(true);
        yield return new WaitForSeconds(alertDuration);
        lostTargetCanvas.SetActive(false);
        lostTargetRoutine = null;
    }
    public void PlayHit()
    {
        hitFeedback?.PlayFeedbacks();
    }

    public void PlayDeath()
    {
        deathFeedback?.PlayFeedbacks();
    }

    public void PlaySpawn()
    {
        spawnFeedback?.PlayFeedbacks();
    }

    public void PlayAtack()
    {
        attackFeedback?.PlayFeedbacks();
    }

    public void PlayStartWalk()
    {
        startWalkFeedback?.PlayFeedbacks();
    }

    public void PlayWalking()
    {
        walkingFeedback?.PlayFeedbacks();
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnHit -= PlayHit;
            health.OnDied -= PlayDeath;
        }
    }
}