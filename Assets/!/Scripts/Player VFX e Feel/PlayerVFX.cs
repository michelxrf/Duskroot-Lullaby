using CombatSystem;
using MoreMountains.Feedbacks;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using System.Collections;

public class PlayerVFX : NetworkBehaviour
{
    [Header("General")]
    [SerializeField] GameObject healVFX;
    [SerializeField] GameObject dashTrailPrefab;
    [SerializeField] private GameObject runStartDustPrefab;
    [SerializeField] GameObject hitVFX;
    [SerializeField] GameObject pauseVFX;

    [Header("Feedbacks")]
    [SerializeField] MMF_Player hitFeedback;
    [SerializeField] MMF_Player healFeedback;
    [SerializeField] MMF_Player deadFeedback;
    [SerializeField] MMF_Player dashFeedback;
    //[SerializeField] MMF_Player pauseFeedback;

    [Header("Level Feedbacks")]
    [SerializeField] private GameObject levelUpCanvasPrefab;
    [SerializeField] private GameObject rewardPopupPrefab;

    public void Play(PlayerVFXEvent vfxEvent)
    {
        switch (vfxEvent)
        {
            case PlayerVFXEvent.Heal:
                OnHeal();
                break;

            case PlayerVFXEvent.Dash:
                OnDashStart();
                break;

            case PlayerVFXEvent.RunStart:
                OnRunStart();
                break;

            case PlayerVFXEvent.Hit:
                OnHit();
                break;

            case PlayerVFXEvent.Death:
                OnDeath();
                break;

            case PlayerVFXEvent.PauseOpen:
                OnPauseOpen();
                break;

            case PlayerVFXEvent.PauseClose:
                OnPauseClose();
                break;
        }
    }

    public override void Spawned()
    {
        Debug.Log(
            $"PlayerVFX Spawned | HasStateAuthority={HasStateAuthority}");

        if (!HasStateAuthority)
            return;

        Debug.Log("INSCREVEU NO ONLEVELUP");

        CharacterDataManager.Instance.OnLevelUp += PlayLevelUp;
    }
    private void PlayParticles(GameObject vfxRoot)
    {
        if (vfxRoot == null)
            return;

        ParticleSystem[] particles = vfxRoot.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particle in particles)
        {
            particle.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            particle.Play();
        }
    }
    private void StartParticles(GameObject vfxRoot)
    {
        if (vfxRoot == null)
            return;

        ParticleSystem[] particles =
            vfxRoot.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }
    private void StopParticles(GameObject vfxRoot)
    {
        if (vfxRoot == null)
            return;

        ParticleSystem[] particles =
            vfxRoot.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particle in particles)
        {
            particle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }
    }

    private void OnHeal()
    {
        healFeedback?.PlayFeedbacks();
        PlayParticles(healVFX);
    }
    private void OnHit()
    {
        hitFeedback?.PlayFeedbacks();
        PlayParticles(hitVFX);
    }
    private void OnDeath()
    {
        deadFeedback?.PlayFeedbacks();
    }
    private void OnPauseOpen()
    {
        //pauseFeedback?.PlayFeedbacks();
        StartParticles(pauseVFX);
    }
    private void OnPauseClose()
    {
        //pauseFeedback?.StopFeedbacks();
        StopParticles(pauseVFX);
    }
    private void OnRunStart()
    {
        //runStartFeedback?.PlayFeedbacks();
    }
    public void SpawnDashTrail( Vector3 position, Vector3 direction)
    {
        if (dashTrailPrefab == null)
            return;

        Quaternion rotation = Quaternion.LookRotation(direction);
        Instantiate(dashTrailPrefab,position,rotation);
    }
    private void OnDashStart()
    {
        dashFeedback?.PlayFeedbacks();
    }

    public void SpawnRunStartDust(Vector3 position)
    {
        if (runStartDustPrefab == null)
            return;
       
        Instantiate( runStartDustPrefab, position,Quaternion.identity);
    }

    public void PlayLevelUp()
    {
        Debug.Log("PLAY LEVEL UP Feedback CHAMADO");
        StartCoroutine(LevelUpFeedbackRoutine());
    }

    private IEnumerator LevelUpFeedbackRoutine()
    {
        GameObject levelCanvas = Instantiate(levelUpCanvasPrefab);
        GameObject rewardPopup = Instantiate(rewardPopupPrefab);
        Image rewardImage = rewardPopup.GetComponentInChildren<Image>();

        if (rewardImage != null)
        {
            rewardImage.sprite =
                CharacterDataManager.Instance
                .GetCharacterPortrait(
                    CharacterDataManager.Instance
                    .GetCurrentPlayerCharacter()
                    .characterId);
        }

        Animator rewardAnimator = rewardPopup.GetComponentInChildren<Animator>();
        yield return new WaitForSeconds(2f);
        rewardAnimator.SetTrigger("Exit");

        yield return new WaitForSeconds(0.5f);

        Destroy(levelCanvas);
        Destroy(rewardPopup);
    }


    private void OnDestroy()
    {
        if (HasStateAuthority &&
            CharacterDataManager.Instance != null)
        {
            CharacterDataManager.Instance.OnLevelUp -= PlayLevelUp;
        }
    }
}
