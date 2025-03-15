using System.Collections;
using Skyward.Characters;
using Skyward.Core;
using Skyward.Systems;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

public class CheckpointComponent : MonoBehaviour, ISkywardComponent
{
    public Transform checkpointPositionOverride;
    public Collider trigger;
    public UnityEvent checkpointReachedEvent;
    
    public DeathZoneComponent DeathZone { get; private set; }
    public Vector3 Position => checkpointPositionOverride != null ? checkpointPositionOverride.position : transform.position;

    private Coroutine coroutine;

    private void Awake()
    {
        DeathZone = GetComponentInChildren<DeathZoneComponent>();
        trigger.isTrigger = true;
    }

    void ISkywardComponent.WorldLoaded()
    {
        coroutine = StartCoroutine(CheckForPlayer());
    }

    void ISkywardComponent.Cleanup()
    {
        StopCoroutine(coroutine);
        coroutine = null;
    }

    private IEnumerator CheckForPlayer()
    {
        yield return new WaitUntil(() => CheckpointSystem.IsReady);

        var player = PlayerSystem.Player;
        while (true)
        {
            Vector3 closestPoint = trigger.ClosestPoint(player.transform.position);
            if (Vector3.Distance(closestPoint, PlayerSystem.PlayerColliderCenter) < 1f)
            {
                CheckpointSystem.OnCheckpointReached(this, player);
                checkpointReachedEvent.Invoke();
                yield break;
            }

            yield return null;
        }
    }
    
    public void ActivateDeathZone() => DeathZone.gameObject.SetActive(true);
}

#if UNITY_EDITOR
[CustomEditor(typeof(CheckpointComponent))]
public class CheckpointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Teleport character"))
        {
            var checkpoint = (CheckpointComponent)target;
            if (PlayerSystem.Instance != null && PlayerSystem.Player != null)
            {
                Undo.RecordObject(PlayerSystem.Player.transform, "Character teleported");
                CheckpointSystem.OnCheckpointReached(checkpoint, PlayerSystem.Player);
            }
            else
            {
                var player = FindAnyObjectByType<LocomotionController>();
                if (player != null)
                { 
                    Undo.RecordObject(player.transform, "Character teleported");
                    player.transform.position = checkpoint.Position;
                }
            }
        }
    }
}
#endif
