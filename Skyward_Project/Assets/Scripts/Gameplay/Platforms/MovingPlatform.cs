using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    internal List<Vector3> points = new();
    [SerializeField] 
    internal bool closePoints;
    
    private int currentIndex = 0;
    private bool movingForward = true;

    private void Start()
    {
        if (!points.Any())
            points.Add(transform.position);
    }
    
    private void Update()
    {
        if (points.Count < 2) return;

        MoveBetweenPoints();
    }

    private void MoveBetweenPoints()
    {
        Vector3 target = points[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            if (closePoints && points.Count > 2) 
            {
                // ✅ Loop back to 0th index when reaching the last waypoint
                currentIndex = (currentIndex + 1) % points.Count;
            }
            else 
            {
                // ✅ Normal back and forth movement
                if (movingForward)
                {
                    currentIndex++;
                    if (currentIndex >= points.Count)
                    {
                        currentIndex = points.Count - 2; // Start moving back
                        movingForward = false;
                    }
                }
                else
                {
                    currentIndex--;
                    if (currentIndex < 0)
                    {
                        currentIndex = 1; // Start moving forward again
                        movingForward = true;
                    }
                }
            }
        }
    }
    
    #if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        if (points.Count == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i], 0.2f);
            if (i < points.Count - 1)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }
        
        if (closePoints && points.Count > 2)
            Gizmos.DrawLine(points[^1], points[0]);
    }
    
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor
{
    private void OnSceneGUI()
    {
        MovingPlatform platform = (MovingPlatform)target;

        if (platform.points == null || platform.points.Count == 0) return;

        // Handles Color
        Handles.color = Color.cyan;

        for (int i = 0; i < platform.points.Count; i++)
        {
            // Draw draggable handles
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(platform.points[i], Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(platform, "Move Waypoint");
                platform.points[i] = newPos;
            }

            // Label for each waypoint
            Handles.Label(platform.points[i] + Vector3.up * 0.2f, $"Waypoint {i}");
        }
    }
}
#endif
