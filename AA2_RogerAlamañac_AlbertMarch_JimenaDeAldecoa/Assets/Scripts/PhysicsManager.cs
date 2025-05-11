using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public static PhysicsManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void ApplyForces(BallController ball)
    {
        // Punt just sota la pilota per comprovar el terra
        Vector3 pointUnderBall = ball.transform.position - Vector3.up * (ball.radius + 0.01f);
        CustomBoxCollider detectedGround = null;

        // Detecta quin material hi ha sota la pilota
        foreach (CustomBoxCollider col in FindObjectsOfType<CustomBoxCollider>())
        {
            if (col.material == CustomBoxCollider.MaterialType.Grass ||
                col.material == CustomBoxCollider.MaterialType.Ice ||
                col.material == CustomBoxCollider.MaterialType.Sand)
            {
                if (col.ContainsPointOBB(pointUnderBall))
                {
                    detectedGround = col; // Trobat el terra sota la pilota
                    break;
                }
            }
        }

        // Si el terra ha canviat, actualitza la fricció de la pilota
        if (detectedGround != null && ball.currentFriction != detectedGround.friction)
        {
            ball.currentFriction = detectedGround.friction;
        }

        // Càlcul de la força gravitatòria: F = m * g
        float mass = ball.mass;
        Vector3 gravity = Physics.gravity;
        Vector3 Fgravity = gravity * mass;

        // Inicialitza les components de força paral·lela i normal
        Vector3 Fparallel = Fgravity;
        Vector3 Fnormal = Vector3.zero;

        // Si la pilota toca terra, separa la força gravitatòria en components
        if (detectedGround != null)
        {
            Vector3 groundNormal = detectedGround.transform.up;
            Fnormal = Vector3.Project(-Fgravity, groundNormal);  // Component normal, reacció del terra
            Fparallel = Fgravity + Fnormal;                      // Component paral·lela, la que fa que llisqui
        }

        // Càlcul de la força de fricció
        float mu = ball.currentFriction;
        Vector3 Ffriction = Vector3.zero;

        if (ball.velocity.magnitude > 0.01f)
        {
            Vector3 frictionDir = -ball.velocity.normalized;  // Direcció oposada al moviment
            Ffriction = frictionDir * mu * Fnormal.magnitude;
        }

        // Càlcul de la força de resistència de l'aire si la pilota està a certa alçada
        Vector3 Fair = Vector3.zero;
        if (ball.transform.position.y > 1.0f)
        {
            float v2 = ball.velocity.sqrMagnitude;
            Fair = -0.5f * ball.airDensity * v2 * ball.dragCoefficient * ball.area * ball.velocity.normalized;
        }

        // Suma de totes les forces aplicades
        Vector3 Ftotal = Fparallel + Ffriction + Fair;
        Vector3 acceleration = Ftotal / mass; // a = F/m

        // Actualitza la velocitat i la posició de la pilota
        ball.velocity += acceleration * Time.deltaTime;
        ball.transform.position += ball.velocity * Time.deltaTime;
        ball.angularVelocity = ball.velocity / ball.radius;

        // Comprova si la pilota està gairebé aturada
        if (ball.velocity.magnitude < 0.015f)
        {
            Vector3 groundNormal = detectedGround != null ? detectedGround.transform.up : Vector3.up;
            float angle = Vector3.Dot(groundNormal, Vector3.up); // 1 = pla horitzontal

            if (angle > 0.99f)
            {
                ball.stillTime += Time.deltaTime;
                if (ball.stillTime > 0.3f) // Considera aturada si porta quieta 0.3s
                {
                    ball.velocity = Vector3.zero;
                    ball.isMoving = false;
                }
            }
            else
            {
                ball.stillTime = 0f;
            }
        }
        else
        {
            ball.stillTime = 0f;
        }

        // Comprova si hi ha col·lisions amb altres col·liders
        CheckCollisions(ball);
    }

    void CheckCollisions(BallController ball)
    {
        CustomBoxCollider[] allColliders = FindObjectsOfType<CustomBoxCollider>();

        foreach (CustomBoxCollider col in allColliders)
        {
            // Troba el punt més proper del col·lisionador a la pilota
            Vector3 closest = col.GetClosestPointOBB(ball.transform.position);
            float distSqr = (closest - ball.transform.position).sqrMagnitude;

            // Comprova si està col·lisionant
            if (distSqr <= ball.radius * ball.radius)
            {
                Vector3 normal = (ball.transform.position - closest).normalized;
                float penetration = ball.radius - Mathf.Sqrt(distSqr);

                // Corregeix la posició per treure la pilota de la col·lisió
                ball.transform.position += normal * penetration;

                float normalSpeed = Vector3.Dot(ball.velocity, normal);
                if (normalSpeed < 0)
                {
                    // Calcula la resposta del rebot segons l'elasticitat
                    Vector3 vNormal = normal * normalSpeed;
                    Vector3 vTangent = ball.velocity - vNormal;
                    Vector3 rebound = -vNormal * col.restitution;
                    ball.velocity = vTangent + rebound;

                    // Si és un tipus de material que compta com rebot, incrementa el comptador
                    if (col.material == CustomBoxCollider.MaterialType.Wall ||
                        col.material == CustomBoxCollider.MaterialType.Rubber ||
                        col.material == CustomBoxCollider.MaterialType.Foam ||
                        col.material == CustomBoxCollider.MaterialType.SandInelastic)
                    {
                        ball.reboundCount++;
                    }
                }
            }
        }
    }
}