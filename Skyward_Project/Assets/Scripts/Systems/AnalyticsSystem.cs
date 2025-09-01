using System.Collections;
using Skyward.Core;
using UnityEngine;

[RequiredSystem]
public class AnalyticsSystem : BaseSystem
{
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);

        StartCoroutine(InitializeFirebase());

    }

    private IEnumerator InitializeFirebase()
    {
        yield return null;
        // var handle = FirebaseApp.CheckAndFixDependenciesAsync();
        // yield return handle;

        // if (handle.Result == DependencyStatus.Available)
        //     Debug.Log("Firebase is successfully initialized.");
        // else
        //     Debug.LogError($"Could not resolve Firebase dependencies: {handle.Result}");
    }
}
