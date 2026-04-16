using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.InputSystem;

public class GramophoneController : MonoBehaviour
{
    [Header("FMOD Events")]
    [SerializeField] private EventReference gramophoneEvent; // 3D
    [SerializeField] private EventReference music2DEvent;    // 2D

    private EventInstance gramophoneInstance;
    private EventInstance music2DInstance;

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
        gramophoneInstance = RuntimeManager.CreateInstance(gramophoneEvent);
        RuntimeManager.AttachInstanceToGameObject(gramophoneInstance, transform);

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

        gramophoneInstance.setParameterByName("Gramophone", 0);
        gramophoneInstance.start();

        currentState = State.GramophoneLoop;
    }

    private void StopGramophoneAndStart2D()
    {
        StartCoroutine(WaitForGramophoneToStop());
    }

    private IEnumerator WaitForGramophoneToStop()
    {
        if (gramophoneInstance.isValid())
        {
            gramophoneInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            PLAYBACK_STATE state;

            do
            {
                gramophoneInstance.getPlaybackState(out state);
                yield return null;
            }
            while (state != PLAYBACK_STATE.STOPPED);

            //gramophoneInstance.release();
        }
        music2DInstance = RuntimeManager.CreateInstance(music2DEvent);
        music2DInstance.start();

        currentState = State.Music2DLoop;
    }
    private void StopMusic2DAndPlayTurnOff()
    {
        if (music2DInstance.isValid())
        {
            music2DInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            music2DInstance.release();
        }

        //gramophoneInstance = RuntimeManager.CreateInstance(gramophoneEvent);
        //RuntimeManager.AttachInstanceToGameObject(gramophoneInstance, transform);

        gramophoneInstance.setParameterByName("Gramophone", 2);
        gramophoneInstance.start();

        currentState = State.Off;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;

        if (currentState == State.GramophoneLoop)
        {
            StopGramophoneAndStart2D();
        }
    }
    
    private void OnDestroy()
    {
        if (gramophoneInstance.isValid())
        {
            gramophoneInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            gramophoneInstance.release();
        }

        if (music2DInstance.isValid())
        {
            music2DInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            music2DInstance.release();
        }
    }
}