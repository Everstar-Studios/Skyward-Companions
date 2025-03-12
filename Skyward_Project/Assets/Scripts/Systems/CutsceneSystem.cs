using System;
using System.Runtime.CompilerServices;
using Skyward.Core;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

[RequiredSystem]
public class CutsceneSystem : BaseSystem<CutsceneSystem>
{
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

    public static void Play(PlayableDirector director)
    {
        Instance.cutsceneStarted?.Invoke(Instance, EventArgs.Empty);
        director.Play();
    }
    
    public static void Play(VideoPlayer videoPlayer)
    {
        Instance.cutsceneStarted?.Invoke(Instance, EventArgs.Empty);
        videoPlayer.Play();
    }

    public static void OnCutsceneEnded(PlayableDirector director)
    {
        Instance.cutsceneStopped?.Invoke(director, EventArgs.Empty);
    }
    
    public static void OnVideoEnded(VideoPlayer videoPlayer)
    {
        Instance.cutsceneStopped?.Invoke(Instance, EventArgs.Empty);

    }
}
