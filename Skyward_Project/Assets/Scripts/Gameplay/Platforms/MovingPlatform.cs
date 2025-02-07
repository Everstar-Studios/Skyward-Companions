using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    internal float movementSpeed;
    [SerializeField]
    internal List<Vector3> points = new();
    [SerializeField] 
    internal bool closePoints;

    [Header("Debug")] 
    public Color lineColor = Color.green;
    public float lineThickness = 2f;
    public Color sphereColor = Color.green;
    public float sphereRadius = 0.2f;
    
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
                currentIndex = (currentIndex + 1) % points.Count;
            }
            else 
            {
                if (movingForward)
                {
                    currentIndex++;
                    if (currentIndex >= points.Count)
                    {
                        currentIndex = points.Count - 2;
                        movingForward = false;
                    }
                }
                else
                {
                    currentIndex--;
                    if (currentIndex < 0)
                    {
                        currentIndex = 1;
                        movingForward = true;
                    }
                }
            }
        }
    }
    
    #if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        if (points.Count == 0) 
            return;
        
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.color = sphereColor;
            Gizmos.DrawSphere(points[i], sphereRadius);
            if (i < points.Count - 1)
            {
                Gizmos.color = lineColor;
                DrawThickLine(points[i], points[i + 1]);
            }
        }

        if (closePoints && points.Count > 2)
        {
            Gizmos.color = lineColor;
            DrawThickLine(points[^1], points[0]);
        }
    }
    
    private void DrawThickLine(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up) * (lineThickness * 0.01f);

        for (float i = -lineThickness * 0.005f; i <= lineThickness * 0.005f; i += 0.005f)
        {
            Vector3 offset = perpendicular * i;
            Gizmos.DrawLine(start + offset, end + offset);
        }
    }
    
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor
{
    private SerializedProperty pointsProp;
    private SerializedProperty movementSpeedProp;
    private SerializedProperty closePointsProp;
    private SerializedProperty lineColorProp;
    private SerializedProperty lineThicknessProp;
    private SerializedProperty sphereRadiusProp;
    private SerializedProperty sphereColorProp;

    private MovingPlatform platform;
    
    private void OnEnable()
    {
        pointsProp = serializedObject.FindProperty(nameof(MovingPlatform.points));
        movementSpeedProp = serializedObject.FindProperty(nameof(MovingPlatform.movementSpeed));
        closePointsProp = serializedObject.FindProperty(nameof(MovingPlatform.closePoints));
        lineColorProp = serializedObject.FindProperty(nameof(MovingPlatform.lineColor));
        lineThicknessProp = serializedObject.FindProperty(nameof(MovingPlatform.lineThickness));
        sphereColorProp = serializedObject.FindProperty(nameof(MovingPlatform.sphereColor));
        sphereRadiusProp = serializedObject.FindProperty(nameof(MovingPlatform.sphereRadius));
        
        platform = (MovingPlatform)target;
    }
    
    private void OnSceneGUI()
    {
        if (platform.points.Count == 0) 
            return;

        Handles.color = Color.cyan;

        for (int i = 0; i < platform.points.Count; i++)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(platform.points[i], Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(platform, "Move Point");
                platform.points[i] = newPos;
            }

            Handles.Label(platform.points[i] + Vector3.up * 0.2f, $"Point {i}");
        }
        
        if (platform.closePoints && platform.points.Count > 2)
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(platform.points[^1], platform.points[0]);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        if (GUILayout.Button("Add Point"))
        {
            Undo.RecordObject(target, "Add Point");
            platform = (MovingPlatform)target;
            Vector3 reference = platform.points.Count > 0 ? platform.points[^1] : platform.transform.position;
            platform.points.Add(reference + Vector3.right * 2f);
        }

        EditorGUILayout.PropertyField(pointsProp);
        EditorGUILayout.PropertyField(movementSpeedProp);

        if (pointsProp.arraySize > 2)
        {
            EditorGUILayout.PropertyField(closePointsProp);
        }

        EditorGUILayout.PropertyField(lineColorProp);
        EditorGUILayout.PropertyField(lineThicknessProp);
        EditorGUILayout.PropertyField(sphereColorProp);
        EditorGUILayout.PropertyField(sphereRadiusProp);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
