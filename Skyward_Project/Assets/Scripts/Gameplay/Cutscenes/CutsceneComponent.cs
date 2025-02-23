using System;
using System.Collections;
using Sirenix.OdinInspector;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Video;

public class CutsceneComponent : MonoBehaviour, ISkywardComponent
{
    private enum ECutsceneType
    {
        Timeline,
        Video
    }
    
    [SerializeField]
    private bool playOnAwake;
    [SerializeField]
    private ECutsceneType cutsceneType;
    [field: SerializeField, ShowIf("@cutsceneType == ECutsceneType.Timeline")]
    public PlayableDirector Director { get; private set; }
    [field: SerializeField, ShowIf("@cutsceneType == ECutsceneType.Video")]
    public VideoPlayer VideoPlayer { get; private set; }
    [SerializeField] 
    private Collider trigger;
    [SerializeField] 
    private UnityEvent onCutsceneStarted;
    [SerializeField]
    private UnityEvent onCutsceneStopped;
    
    private Coroutine recognitionCoroutine;
    private bool hasPlayed;

    void ISkywardComponent.WorldLoaded()
    {
        if (Director != null)
            Director.stopped += OnCutsceneEnd;
        else if (VideoPlayer != null)
        {
            VideoPlayer.loopPointReached += OnVideoEnded;
            VideoPlayer.targetCamera = CameraSystem.Camera;
        }

        if (!playOnAwake && trigger == null)
            Debug.LogError($"The Cutscene on the object {gameObject.name} will not play because {nameof(playOnAwake)} is false and collider is not set.");

        if (playOnAwake)
            Play();
        else if (trigger != null)
            recognitionCoroutine = StartCoroutine(RecognizePlayer());
    }

    void ISkywardComponent.Cleanup()
    {
        if (recognitionCoroutine != null)
        {
            StopCoroutine(recognitionCoroutine);
            recognitionCoroutine = null;
        }
        
        if (Director != null)
            Director.stopped -= OnCutsceneEnd;
        else if (VideoPlayer != null)
            VideoPlayer.loopPointReached -= OnVideoEnded;

        hasPlayed = false;
    }

    private IEnumerator RecognizePlayer()
    {
        yield return new WaitUntil(() => PlayerSystem.Player != null);
        
        while (true)
        {
            if (hasPlayed)
                yield break;
            
            if (IsPlayerInColliderBounds(trigger))
            {
                Play();
                hasPlayed = true;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void Play()
    {
        if (Director != null)
            CutsceneSystem.Play(Director);
        else if (VideoPlayer != null)
            CutsceneSystem.Play(VideoPlayer);
        
        onCutsceneStarted?.Invoke();
        CameraSystem.DisableCamera();
    }

    private void OnCutsceneEnd(PlayableDirector _)
    {
        onCutsceneStopped?.Invoke();
        CameraSystem.EnableCamera();
    }
    
    private void OnVideoEnded(VideoPlayer _)
    {
        onCutsceneStopped?.Invoke();
        CameraSystem.EnableCamera();
    }
    
    private static bool IsPlayerInColliderBounds(Collider collider)
    {
        return collider.bounds.Contains(PlayerSystem.Player.transform.position);
    }
}
