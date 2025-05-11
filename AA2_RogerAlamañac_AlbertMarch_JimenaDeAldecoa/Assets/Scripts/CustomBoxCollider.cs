using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBoxCollider : MonoBehaviour
{
    public enum MaterialType
    {
        Wall,           // e = 0.4, µ = 0.5
        Rubber,         // e = 0.8, quasi-elastic
        Foam,           // e = 0.2, inelastic
        SandInelastic,  // e = 0.2, µ = 0.6
        Grass,          // µ = 0.4
        Ice,            // µ = 0.1
        Sand            // µ = 0.6
    }

    public MaterialType material = MaterialType.Wall;

    public Vector3 center = Vector3.zero;
    public Vector3 size = Vector3.one;

    public float restitution = 0.8f;
    public float friction = 0.5f;

    public bool showGizmo = true;

    public Bounds GetBounds()
    {
        Vector3 scaledSize = new Vector3(
            size.x * transform.lossyScale.x,
            size.y * transform.lossyScale.y,
            size.z * transform.lossyScale.z
        );

        Vector3 worldCenter = transform.position + Vector3.Scale(center, transform.lossyScale);
        return new Bounds(worldCenter, scaledSize);
    }
    public bool ContainsPointOBB(Vector3 point)
    {
        Vector3 local = transform.worldToLocalMatrix.MultiplyPoint(point) - center;
        Vector3 halfSize = size * 0.5f;

        return Mathf.Abs(local.x) <= halfSize.x &&
               Mathf.Abs(local.y) <= halfSize.y &&
               Mathf.Abs(local.z) <= halfSize.z;
    }
    public Vector3 GetClosestPointOBB(Vector3 point)
    {
        Matrix4x4 worldToLocal = transform.worldToLocalMatrix;
        Vector3 localPoint = worldToLocal.MultiplyPoint(point) - center;
        Vector3 halfSize = size * 0.5f;

        Vector3 clamped = new Vector3(
            Mathf.Clamp(localPoint.x, -halfSize.x, halfSize.x),
            Mathf.Clamp(localPoint.y, -halfSize.y, halfSize.y),
            Mathf.Clamp(localPoint.z, -halfSize.z, halfSize.z)
        );

        return transform.localToWorldMatrix.MultiplyPoint(clamped + center);
    }

    void OnValidate()
    {
        switch (material)
        {
            case MaterialType.Rubber:
                restitution = 0.8f;
                friction = 0.5f;
                break;
            case MaterialType.Wall:
                restitution = 0.4f;
                friction = 0.5f;
                break;

            case MaterialType.Foam:
            case MaterialType.SandInelastic:
                restitution = 0.2f;
                friction = 0.6f;
                break;

            case MaterialType.Grass:
                restitution = 0.0f;
                friction = 0.4f;
                break;

            case MaterialType.Ice:
                restitution = 0.0f;
                friction = 0.1f;
                break;

            case MaterialType.Sand:
                restitution = 0.0f;
                friction = 0.6f;
                break;
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Gizmos.color = Color.red;

        Vector3 scaledSize = new Vector3(
            size.x * transform.lossyScale.x,
            size.y * transform.lossyScale.y,
            size.z * transform.lossyScale.z
        );

        Vector3 worldCenter = transform.position + transform.rotation * Vector3.Scale(center, transform.lossyScale);

        // Aplica rotació i escala al dibuix del collider
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(worldCenter, transform.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;

        Gizmos.DrawWireCube(Vector3.zero, scaledSize);
        Gizmos.matrix = Matrix4x4.identity;
    }

}