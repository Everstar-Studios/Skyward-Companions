using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MovingPlatform : Platform
{
    public enum EMovementType
    {
        Linear,
        Loop,
        Circular
    }

    [Header("General")] 
    [SerializeField] 
    internal EMovementType movementType;
    [SerializeField] 
    internal float movementSpeed = 1.5f;
    [SerializeField] 
    internal bool waitForPlayerToStart;
    [SerializeField] 
    internal float delayBeforeMoving = 0.25f;
    [SerializeField] 
    internal bool stopWhenReachingEnd = false;
    [SerializeField] 
    internal float delayToMoveWhenReachingEnd = 0f;

    [Header("Linear Movement")] 
    [SerializeField]
    internal List<Vector3> points = new();

    [Header("Circular Settings")] 
    public Vector3 circularCenter = Vector3.zero;
    public float circularRadius = 5f;
    public Vector3 circularAxis = Vector3.up;
    public float circularAngleOffset = 0f;

    [Header("Debug")] 
    public float lineThickness = 2f;
    public Color lineColor = Color.green;
    public float sphereRadius = 0.2f;
    public Color sphereColor = Color.black;
    public Color circleColor = Color.white;

    private int currentIndex = 0;
    private bool movingForward = true;
    private float circularAngle = 0f;

    private bool hasPlayerStepped;
    private bool canMove = true;

    private void Start()
    {
        if (movementType == EMovementType.Circular)
        {
            circularAngle = circularAngleOffset * Mathf.Deg2Rad;
            Quaternion rotation = Quaternion.AngleAxis(circularAngle * Mathf.Rad2Deg, circularAxis);
            Vector3 offset = rotation * new Vector3(circularRadius, 0, 0);
            transform.position += offset;
            circularCenter = transform.position;
        }
        else if (!points.Any())
            points.Add(transform.position);
    }

    private void FixedUpdate()
    {
        if (!canMove)
            return;
        if (waitForPlayerToStart && !hasPlayerStepped)
            return;
        
        if (movementType == EMovementType.Circular)
        {
            circularAngle += movementSpeed * Time.fixedDeltaTime;
            HandleCircularMovement();
        }
        else if (points.Count > 1)
            MoveBetweenPoints();
    }

    private void HandleCircularMovement()
    {
        Vector3 normalizedAxis = circularAxis.normalized;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normalizedAxis);
        float angle = circularAngle;
        Vector3 offset = rotation * new Vector3(Mathf.Cos(angle) * circularRadius, Mathf.Sin(angle) * circularRadius, 0);
        transform.position = circularCenter + offset;
    }

    private void MoveBetweenPoints()
    {
        Vector3 target = points[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            if (movementType == EMovementType.Loop && points.Count > 2)
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
                        movingForward = false;
                        if (stopWhenReachingEnd)
                        {
                            canMove = false;
                        }
                        else if (delayToMoveWhenReachingEnd > float.Epsilon)
                        {
                            StartCoroutine(WaitBeforeMovingBack(points.Count - 2));
                        }
                        else
                        {
                            currentIndex = points.Count - 2;
                        }
                    }
                }
                else
                {
                    currentIndex--;
                    if (currentIndex < 0)
                    {
                        movingForward = true;
                        if (delayToMoveWhenReachingEnd > float.Epsilon)
                        {
                            StartCoroutine(WaitBeforeMovingBack(1));
                        }
                        else
                        {
                            currentIndex = 1;
                        }
                    }
                }
            }
        }
    }

    private IEnumerator WaitBeforeMovingBack(int nextIndex)
    {
        canMove = false;
        yield return new WaitForSeconds(delayToMoveWhenReachingEnd);
        currentIndex = nextIndex;
        canMove = true;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (movementType == EMovementType.Circular)
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, circularAxis.normalized);

            int segments = 32;
            if (!Application.isPlaying)
                circularCenter = transform.position;

            Vector3 prevPoint = circularCenter + rotation * new Vector3(circularRadius, 0, 0);

            Gizmos.color = circleColor;
            for (int i = 1; i <= segments; i++)
            {
                float angle = (i / (float)segments) * 360f * Mathf.Deg2Rad;
                Vector3 nextPoint = circularCenter + rotation * new Vector3(Mathf.Cos(angle) * circularRadius,
                    Mathf.Sin(angle) * circularRadius, 0);
                DrawThickLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
        else if (points.Count > 0)
        {
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

            if (movementType == EMovementType.Loop && points.Count > 2)
            {
                Gizmos.color = lineColor;
                DrawThickLine(points[^1], points[0]);
            }
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
    public void OnPlayerStepped()
    {
        if (hasPlayerStepped)
            return;
        
        StartCoroutine(StepFlow());
    }

    private IEnumerator StepFlow()
    {
        if (waitForPlayerToStart && delayBeforeMoving > float.Epsilon)
            yield return new WaitForSeconds(delayBeforeMoving);

        hasPlayerStepped = true;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor
{
    private SerializedProperty movementTypeProp;
    private SerializedProperty waitForPlayerToStartProp;
    private SerializedProperty delayBeforeMovingProp;
    private SerializedProperty stopWhenReachingEndProp;
    private SerializedProperty delayToMoveWhenReachingEndProp;
    private SerializedProperty pointsProp;
    private SerializedProperty movementSpeedProp;
    private SerializedProperty lineColorProp;
    private SerializedProperty circularCenterProp;
    private SerializedProperty circularRadiusProp;
    private SerializedProperty circularAxisProp;
    private SerializedProperty circularAngleOffsetProp;
    private SerializedProperty circleColorProp;
    private SerializedProperty lineThicknessProp;
    private SerializedProperty sphereRadiusProp;
    private SerializedProperty sphereColorProp;

    private MovingPlatform platform;

    private void OnEnable()
    {
        movementTypeProp = serializedObject.FindProperty(nameof(MovingPlatform.movementType));
        waitForPlayerToStartProp = serializedObject.FindProperty(nameof(MovingPlatform.waitForPlayerToStart));
        delayBeforeMovingProp = serializedObject.FindProperty(nameof(MovingPlatform.delayBeforeMoving));
        stopWhenReachingEndProp = serializedObject.FindProperty(nameof(MovingPlatform.stopWhenReachingEnd));
        delayToMoveWhenReachingEndProp = serializedObject.FindProperty(nameof(MovingPlatform.delayToMoveWhenReachingEnd));
        pointsProp = serializedObject.FindProperty(nameof(MovingPlatform.points));
        movementSpeedProp = serializedObject.FindProperty(nameof(MovingPlatform.movementSpeed));
        lineColorProp = serializedObject.FindProperty(nameof(MovingPlatform.lineColor));
        lineThicknessProp = serializedObject.FindProperty(nameof(MovingPlatform.lineThickness));
        sphereColorProp = serializedObject.FindProperty(nameof(MovingPlatform.sphereColor));
        sphereRadiusProp = serializedObject.FindProperty(nameof(MovingPlatform.sphereRadius));
        circleColorProp = serializedObject.FindProperty(nameof(MovingPlatform.circleColor));
        circularCenterProp = serializedObject.FindProperty(nameof(MovingPlatform.circularCenter));
        circularRadiusProp = serializedObject.FindProperty(nameof(MovingPlatform.circularRadius));
        circularAxisProp = serializedObject.FindProperty(nameof(MovingPlatform.circularAxis));
        circularAngleOffsetProp = serializedObject.FindProperty(nameof(MovingPlatform.circularAngleOffset));

        platform = (MovingPlatform)target;
    }

    private void OnSceneGUI()
    {
        if (platform.movementType == MovingPlatform.EMovementType.Circular)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 newCenter = Handles.PositionHandle(platform.circularCenter, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(platform, "Move Circular Center");
                platform.circularCenter = newCenter;
            }
        }
        else if (platform.points != null && platform.points.Count > 0)
        {
            Handles.color = platform.lineColor;

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
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(movementTypeProp);
        EditorGUILayout.PropertyField(movementSpeedProp);
        EditorGUILayout.PropertyField(waitForPlayerToStartProp);
        if (waitForPlayerToStartProp.boolValue)
            EditorGUILayout.PropertyField(delayBeforeMovingProp);
        
        EditorGUILayout.PropertyField(stopWhenReachingEndProp);
        if (!stopWhenReachingEndProp.boolValue)
            EditorGUILayout.PropertyField(delayToMoveWhenReachingEndProp);
            

        MovingPlatform.EMovementType movementType = (MovingPlatform.EMovementType)movementTypeProp.enumValueIndex;

        if (movementType is MovingPlatform.EMovementType.Linear or MovingPlatform.EMovementType.Loop)
        {
            EditorGUILayout.PropertyField(pointsProp);
            if (GUILayout.Button("Add Point"))
            {
                Undo.RecordObject(target, "Add Point");
                platform = (MovingPlatform)target;
                Vector3 reference = platform.points.Count > 0 ? platform.points[^1] : platform.transform.position;
                platform.points.Add(reference + Vector3.right * 2f);
            }

            EditorGUILayout.PropertyField(lineThicknessProp);
            EditorGUILayout.PropertyField(lineColorProp);
            EditorGUILayout.PropertyField(sphereColorProp);
            EditorGUILayout.PropertyField(sphereRadiusProp);
        }
        else if (movementType == MovingPlatform.EMovementType.Circular)
        {
            EditorGUILayout.PropertyField(circularCenterProp);
            EditorGUILayout.PropertyField(circularRadiusProp);
            EditorGUILayout.PropertyField(circularAxisProp);
            EditorGUILayout.PropertyField(circularAngleOffsetProp);
            EditorGUILayout.PropertyField(lineThicknessProp);
            EditorGUILayout.PropertyField(circleColorProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif