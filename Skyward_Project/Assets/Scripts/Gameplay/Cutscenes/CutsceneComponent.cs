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

    [SerializeField] private bool playOnAwake;
    [SerializeField] private ECutsceneType cutsceneType;
    [field: SerializeField, ShowIf("@cutsceneType == ECutsceneType.Timeline")]
    public TimelineAsset Timeline { get; private set; }
    [field: SerializeField, ShowIf("@cutsceneType == ECutsceneType.Video")]
    public VideoClip VideoClip { get; private set; }

    [SerializeField] private Collider trigger;
    [SerializeField] private bool disableInput = true;
    [SerializeField] private UnityEvent onCutsceneStarted;
    [SerializeField] private UnityEvent onCutsceneStopped;

    [SerializeField] private RectTransform[] resetTransforms;
    private Vector3[] originalPositions;
    private Vector3[] originalScales;

    private Coroutine recognitionCoroutine;
    private bool hasPlayed;
    private bool isPlaying;

    private PlayableDirector director;
    private VideoPlayer videoPlayer;
    private RenderTexture renderTexture;

    void ISkywardComponent.WorldLoaded(GameContext context)
    {
        GameManager.Instance.GameHUD.cutsceneRawImage.texture = null;
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        yield return new WaitForEndOfFrame();

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

        CacheOriginalTransformStates();
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

            if (IsPlayerInColliderBounds())
                Play();

            yield return new WaitForFixedUpdate();
        }
    }

    private void Play()
    {
        hasPlayed = true;
        isPlaying = true;

        GameManager.Instance.GameHUD.cutsceneRawImage.texture = null;
        GameManager.Instance.GameHUD.cutsceneRawImage.gameObject.SetActive(false);

        if (cutsceneType == ECutsceneType.Timeline && director != null)
        {
            CutsceneSystem.Play(director);
        }
        else if (cutsceneType == ECutsceneType.Video && videoPlayer != null)
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }

            renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            renderTexture.Create();
            videoPlayer.targetTexture = renderTexture;

            GameManager.Instance.GameHUD.cutsceneRawImage.texture = renderTexture;
            GameManager.Instance.GameHUD.cutsceneRawImage.gameObject.SetActive(true);

            CutsceneSystem.Play(videoPlayer);
            CutsceneSystem.CutsceneSkipped += SkipCutscene;
        }

        if (disableInput)
            GameInputSystem.DisableInput();

        AudioSystem.Pause();
        CameraSystem.DisableCamera();

        onCutsceneStarted?.Invoke();
    }

    private void OnCutsceneEnd(PlayableDirector _)
    {
        OnEnd();
        CutsceneSystem.OnCutsceneEnded(director);
        CutsceneSystem.CutsceneSkipped -= SkipCutscene;

        director.Stop();
        Destroy(director);
    }

    private void OnVideoEnded(VideoPlayer _)
    {
        OnEnd();
        CutsceneSystem.OnVideoEnded(videoPlayer);
        videoPlayer.Stop();
        GameManager.Instance.GameHUD.cutsceneRawImage.texture = null;
        Destroy(videoPlayer);
    }

    private void OnEnd()
    {
        isPlaying = false;
        onCutsceneStopped?.Invoke();

        if (disableInput)
            GameInputSystem.EnableInput();

        CameraSystem.EnableCamera();
        AudioSystem.Unpause();

        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
            renderTexture = null;
        }

        if (GameManager.Instance?.GameHUD?.cutsceneRawImage != null)
        {
            GameManager.Instance.GameHUD.cutsceneRawImage.texture = null;
        }

        ResetTransformStates();
    }

    private bool IsPlayerInColliderBounds()
    {
        return trigger.bounds.Contains(PlayerSystem.Player.transform.position);
    }

    private void SkipCutscene(object sender, EventArgs args)
    {
        if (videoPlayer != null)
            OnVideoEnded(videoPlayer);
        else if (director != null)
            OnCutsceneEnd(director);
    }

    private void CacheOriginalTransformStates()
    {
        if (resetTransforms == null || resetTransforms.Length == 0) return;

        originalPositions = new Vector3[resetTransforms.Length];
        originalScales = new Vector3[resetTransforms.Length];

        for (int i = 0; i < resetTransforms.Length; i++)
        {
            originalPositions[i] = resetTransforms[i].anchoredPosition3D;
            originalScales[i] = resetTransforms[i].localScale;
        }
    }

    private void ResetTransformStates()
    {
        if (resetTransforms == null || originalPositions == null) return;

        for (int i = 0; i < resetTransforms.Length; i++)
        {
            resetTransforms[i].anchoredPosition3D = originalPositions[i];
            resetTransforms[i].localScale = originalScales[i];

            // 🔁 Bu satırlar glitch'i çözer: UI öğesini yeniden çizdirir
            resetTransforms[i].gameObject.SetActive(false);
            resetTransforms[i].gameObject.SetActive(true);
        }
    }
}
