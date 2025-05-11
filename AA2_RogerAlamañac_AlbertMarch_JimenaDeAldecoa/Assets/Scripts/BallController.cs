using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float mass = 1f;
    public float radius = 0.1f;
    public float dragCoefficient = 0.47f;  // La resistència de l’aire
    public float area = 0.04f;             // Secció transversal de la pilota
    public float airDensity = 1.2f;        // Densitat de l’aire
    public float currentFriction = 0.4f;   // Fricció actual del terreny

    private float maxDragDistance = 1.9f;  // Força màxima aplicada amb el ratolí

    public Vector3 velocity;
    public Vector3 angularVelocity;
    public Vector3 lastPosition;
    public bool isMoving = true;
    public float stillTime = 0f;

    private Vector3 dragStart;
    private Camera cam;

    public int reboundCount = 0;           // Comptador de col·lisions per limitar els rebots
    public int maxRebounds = 2;

    public LineRenderer forceLine;         // Visualització de la força aplicada

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
        HandleInput(); // Clic i arrossegar
        PhysicsManager.Instance.ApplyForces(this); // Aplicació de gravitació, fricció, aire
        lastPosition = transform.position;
    }

    void HandleInput()
    {
        // Quan el jugador comença a arrossegar
        if (!isMoving && Input.GetMouseButtonDown(0))
        {
            dragStart = GetMouseWorld(); // Posició inicial del drag a l'espai del món
            if (forceLine != null)
            {
                forceLine.positionCount = 3;
                forceLine.gameObject.SetActive(true); // Mostra la línia de força
            }
        }

        // Mentre manté premut el botó del ratolí
        if (!isMoving && Input.GetMouseButton(0))
        {
            Vector3 current = GetMouseWorld();
            Vector3 dir = dragStart - current;

            // Limita la força màxima segons maxDragDistance
            if (dir.magnitude > maxDragDistance)
                dir = dir.normalized * maxDragDistance;

            if (forceLine != null)
            {
                Vector3 start = transform.position;
                Vector3 mid = start + dir;

                // Dibuixa la línia de força per visualitzar direcció i intensitat
                forceLine.SetPosition(0, start);
                forceLine.SetPosition(1, mid);
                forceLine.SetPosition(2, mid);
            }
        }

        // Quan s’allibera el clic del ratolí, aplica la força
        if (!isMoving && Input.GetMouseButtonUp(0))
        {
            Vector3 dragEnd = GetMouseWorld();
            Vector3 dir = dragStart - dragEnd;

            if (dir.magnitude > maxDragDistance)
                dir = dir.normalized * maxDragDistance;

            // Força aplicada sobre la pilota: proporcional al drag
            // Simula una acceleració inicial: F = m·a -> a = F/m
            Vector3 force = dir * 10f;
            velocity += force / mass;
            isMoving = true;

            if (forceLine != null)
                forceLine.gameObject.SetActive(false);
        }
    }

    Vector3 GetMouseWorld()
    {
        // Converteix la posició del cursor a una posició en el món
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Defineix un pla horitzontal a l'alçada de la pilota
        Plane plane = new Plane(Vector3.up, transform.position.y);
        float distance;

        // Intersecta el ratolí amb aquest pla
        plane.Raycast(ray, out distance);
        return ray.GetPoint(distance); // Retorna la posició 3D al món
    }
}