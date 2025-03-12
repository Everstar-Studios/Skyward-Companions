using System;
using UnityEngine;

public class RotatingPlatform : Platform
{
    [SerializeField]
    private float rotationSpeed = 30f;
    [SerializeField]
    private Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime);
    }
    
    private void OnDrawGizmos()
    {
        // Set the color for the gizmo
        Gizmos.color = Color.blue;

        // Get the position of the platform
        Vector3 position = transform.position;

        // Transform the rotation axis from local space to world space
        Vector3 worldRotationAxis = transform.TransformDirection(rotationAxis.normalized);

        // Draw an arrow to indicate the rotation direction
        float arrowLength = 2f;
        float arrowHeadAngle = 20f;
        float arrowHeadLength = 0.5f;

        // Draw the main line of the arrow
        Gizmos.DrawRay(position, worldRotationAxis * arrowLength);

        // Draw the arrowhead
        Vector3 right = Quaternion.LookRotation(worldRotationAxis) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(worldRotationAxis) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
        Gizmos.DrawRay(position + worldRotationAxis * arrowLength, right * arrowHeadLength);
        Gizmos.DrawRay(position + worldRotationAxis * arrowLength, left * arrowHeadLength);
    }
}

