using System;
using System.Runtime.CompilerServices;
using Skyward.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Video;

[RequiredSystem]
public class CutsceneSystem : BaseSystem<CutsceneSystem>
{
    private CutsceneInputAction cutceneInput;

    public static event EventHandler CutsceneStarted
    {
        add => Instance.cutsceneStarted += value;
        remove => Instance.cutsceneStarted -= value;
    }
    private event EventHandler cutsceneStarted;
    
    public static event EventHandler CutsceneStopped
    {
        add => Instance.cutsceneStopped += value;
        remove => Instance.cutsceneStopped -= value;
    }

    private event EventHandler cutsceneStopped;
    
    public static event EventHandler CutsceneSkipped
    {
        add => Instance.cutsceneSkipped += value;
        remove => Instance.cutsceneSkipped -= value;
    }

    private event EventHandler cutsceneSkipped;

    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);
        
        cutceneInput = new CutsceneInputAction();
        cutceneInput.Disable();
        cutceneInput.Cutscene.SkipCutscene.performed += OnCutsceneSkipped;
    }

    private void OnCutsceneSkipped(InputAction.CallbackContext obj)
    {
        cutsceneSkipped?.Invoke(this, EventArgs.Empty);
    }

    public static void Play(PlayableDirector director)
    {
        Instance.cutceneInput.Enable();
        Instance.cutsceneStarted?.Invoke(Instance, EventArgs.Empty);
        director.Play();
    }
    
    public static void Play(VideoPlayer videoPlayer)
    {
        Instance.cutceneInput.Enable();
        Instance.cutsceneStarted?.Invoke(Instance, EventArgs.Empty);
        videoPlayer.Play();
    }

    public static void OnCutsceneEnded(PlayableDirector director)
    {
        Instance.cutceneInput.Disable();
        Instance.cutsceneStopped?.Invoke(director, EventArgs.Empty);
    }
    
    public static void OnVideoEnded(VideoPlayer videoPlayer)
    {
        Instance.cutceneInput.Disable();
        Instance.cutsceneStopped?.Invoke(Instance, EventArgs.Empty);

    }

    public bool IsCutsceneSkipped()
    {
        return cutceneInput.Cutscene.SkipCutscene.WasCompletedThisFrame();
    }
}
