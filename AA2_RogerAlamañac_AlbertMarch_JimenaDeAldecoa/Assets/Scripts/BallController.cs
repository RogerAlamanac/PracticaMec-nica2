using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float mass = 1f;
    public float radius = 0.1f;
    public float dragCoefficient = 0.47f;
    public float area = 0.04f;
    public float airDensity = 1.2f;
    public float currentFriction = 0.4f;

    private float maxDragDistance = 1.9f;

    public Vector3 velocity;
    public Vector3 angularVelocity;
    public Vector3 lastPosition;
    public bool isMoving = true;
    public float stillTime = 0f;

    private Vector3 dragStart;
    private Camera cam;

    public int reboundCount = 0;
    public int maxRebounds = 2;

    public LineRenderer forceLine;

    void Start()
    {
        cam = Camera.main;
        lastPosition = transform.position;

        if (forceLine != null)
        {
            forceLine.positionCount = 3;
            forceLine.gameObject.SetActive(false);
        }
        reboundCount = 0;
    }

    void Update()
    {
        if(transform.position.y <= -5f)
        {

        }
        HandleInput();
        PhysicsManager.Instance.ApplyForces(this);
        lastPosition = transform.position;
    }

    void HandleInput()
    {
        if (!isMoving && Input.GetMouseButtonDown(0))
        {
            dragStart = GetMouseWorld();
            if (forceLine != null)
            {
                forceLine.positionCount = 3;
                forceLine.gameObject.SetActive(true);
            }
        }

        if (!isMoving && Input.GetMouseButton(0))
        {
            Vector3 current = GetMouseWorld();
            Vector3 dir = dragStart - current;

            if (dir.magnitude > maxDragDistance)
                dir = dir.normalized * maxDragDistance;

            Vector3 start = transform.position;
            Vector3 mid = start + dir;

            if (forceLine != null)
            {
                forceLine.SetPosition(0, start);
                forceLine.SetPosition(1, mid);
                forceLine.SetPosition(2, mid);
            }
        }

        if (!isMoving && Input.GetMouseButtonUp(0))
        {
            Vector3 dragEnd = GetMouseWorld();
            Vector3 dir = dragStart - dragEnd;

            if (dir.magnitude > maxDragDistance)
                dir = dir.normalized * maxDragDistance;

            Vector3 force = dir * 10f;
            velocity += force / mass;
            isMoving = true;

            if (forceLine != null)
                forceLine.gameObject.SetActive(false);
        }
    }

    Vector3 GetMouseWorld()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position.y);
        float distance;
        plane.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}