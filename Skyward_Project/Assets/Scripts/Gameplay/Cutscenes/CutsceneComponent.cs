using System;
using System.Collections;
using Sirenix.OdinInspector;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
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
    public TimelineAsset Timeline { get; private set; }
    [field: SerializeField, ShowIf("@cutsceneType == ECutsceneType.Video")]
    public VideoClip VideoClip { get; private set; }
    [SerializeField] 
    private Collider trigger;
    [SerializeField] 
    private bool disableInput = true;
    [SerializeField] 
    private UnityEvent onCutsceneStarted;
    [SerializeField]
    private UnityEvent onCutsceneStopped;
    
    private Coroutine recognitionCoroutine;
    private bool hasPlayed;

    private PlayableDirector director;
    private VideoPlayer videoPlayer;

    void ISkywardComponent.WorldLoaded()
    {
        if (cutsceneType == ECutsceneType.Timeline && Timeline != null)
            SetupPlayableDirector();
        else if (cutsceneType == ECutsceneType.Video && VideoClip != null)
            SetupVideoPlayer();

        if (!playOnAwake && trigger == null)
            Debug.LogError($"The Cutscene on the object {gameObject.name} will not play because {nameof(playOnAwake)} is false and collider is not set.");

        if (playOnAwake)
            Play();
        else if (trigger != null)
            recognitionCoroutine = StartCoroutine(RecognizePlayer());
    }

    private void SetupPlayableDirector()
    {
        director = gameObject.AddComponent<PlayableDirector>();
        director.playOnAwake = playOnAwake;
        director.playableAsset = Timeline;
        director.stopped += OnCutsceneEnd;
    }

    private void SetupVideoPlayer()
    {
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = playOnAwake;
        videoPlayer.clip = VideoClip;
        videoPlayer.loopPointReached += OnVideoEnded;
        // TODO Omer: To be changed to Render Texture soon
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
        videoPlayer.targetCamera = CameraSystem.Camera;
        videoPlayer.Prepare();
    }

    void ISkywardComponent.Cleanup()
    {
        if (recognitionCoroutine != null)
        {
            StopCoroutine(recognitionCoroutine);
            recognitionCoroutine = null;
        }
        
        if (director != null)
            director.stopped -= OnCutsceneEnd;
        else if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoEnded;

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
                Play();

            yield return new WaitForFixedUpdate();
        }
    }

    private void Play()
    {
        hasPlayed = true;
        
        if (director != null)
            CutsceneSystem.Play(director);
        else if (videoPlayer != null)
            CutsceneSystem.Play(videoPlayer);

        if (disableInput)
            GameInputSystem.DisableInput();
        
        onCutsceneStarted?.Invoke();
    }
    
    private void OnCutsceneEnd(PlayableDirector _)
    {
        onCutsceneStopped?.Invoke();
        director.Stop();
        CutsceneSystem.OnCutsceneEnded(director);
        
        if (disableInput)
            GameInputSystem.EnableInput();
    }
    
    private void OnVideoEnded(VideoPlayer _)
    {
        onCutsceneStopped?.Invoke();
        videoPlayer.Stop();
        CutsceneSystem.OnVideoEnded(videoPlayer);
        
        if (disableInput)
            GameInputSystem.EnableInput();
    }
    
    private static bool IsPlayerInColliderBounds(Collider collider)
    {
        return collider.bounds.Contains(PlayerSystem.Player.transform.position);
    }
}
