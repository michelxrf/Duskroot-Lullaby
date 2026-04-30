using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.InputSystem;

public class GramophoneController : MonoBehaviour
{
    [Header("FMOD Events")]
    [SerializeField] private EventReference gramophoneOff; //desliga Gramofone
    [SerializeField] private EventReference musicGramophone;    // 2D
    [SerializeField] private GameObject cameraGramophone;
    [SerializeField] GameObject interactionTooltip;

    private EventInstance musicInstance;
    private EventInstance gramophoneOffInstance;

    private bool playerInside = false;

    private enum State
    {
        Off,
        GramophoneLoop,
        Music2DLoop
    }

    private State currentState = State.Off;
    private void Start()
    {
        MusicManager.instance.StopMusic();
        musicInstance = RuntimeManager.CreateInstance(musicGramophone);
        RuntimeManager.AttachInstanceToGameObject(musicInstance, transform);

        gramophoneOffInstance = RuntimeManager.CreateInstance(gramophoneOff);
        RuntimeManager.AttachInstanceToGameObject(gramophoneOffInstance, transform);
        
        currentState = State.Off;

        interactionTooltip.SetActive(false);
    }

    private void Update()
    {
        if (playerInside && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        switch (currentState)
        {
            case State.Off:
                StartGramophoneLoop();
                break;

            case State.GramophoneLoop:
                break;

            case State.Music2DLoop:
                StopMusic2DAndPlayTurnOff();
                break;
        }
    }

    private void StartGramophoneLoop()
    {

        musicInstance.setParameterByName("radioEffect", 1);
        musicInstance.start();

        currentState = State.GramophoneLoop;
    }

    private void StopGramophoneAndStart2D()
    {
        musicInstance.setParameterByName("radioEffect", 0);
        currentState = State.Music2DLoop;
    }

    private void StopMusic2DAndPlayTurnOff()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            //musicInstance.release();
        }
        PlayTurnOff();
        
    }

    private void PlayTurnOff()
    {
        musicInstance.setParameterByName("radioEffect", 1);
        gramophoneOffInstance.start();
        currentState = State.Off;

    }

    private void OnTriggerEnter(Collider other)
    {
        //if (!other.CompareTag("Player")) return;
        PlayerSetup otherPlayer = other.GetComponent<PlayerSetup>();
        if (otherPlayer == null) return;
        if (!otherPlayer.IsLocalPlayer()) return;
        if (interactionTooltip != null) interactionTooltip.SetActive(true);
        if (cameraGramophone!=null) cameraGramophone.SetActive(true);
        playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //if (!other.CompareTag("Player")) return;
        PlayerSetup otherPlayer = other.GetComponent<PlayerSetup>();
        if (otherPlayer == null) return;
        if (!otherPlayer.IsLocalPlayer()) return;
        if(interactionTooltip!=null) interactionTooltip.SetActive(false);

        playerInside = false;
        if (cameraGramophone != null) cameraGramophone.SetActive(false);
        if (currentState == State.GramophoneLoop)
        {
            StopGramophoneAndStart2D();
        }
    }
    
    private void OnDestroy()
    {
        if (gramophoneOffInstance.isValid())
        {
            gramophoneOffInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            gramophoneOffInstance.release();
        }

        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }
    }
}