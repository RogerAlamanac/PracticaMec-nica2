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
        Vector3 pointUnderBall = ball.transform.position - Vector3.up * (ball.radius + 0.01f);
        CustomBoxCollider detectedGround = null;

        foreach (CustomBoxCollider col in FindObjectsOfType<CustomBoxCollider>())
        {
            if (col.material == CustomBoxCollider.MaterialType.Grass ||
                col.material == CustomBoxCollider.MaterialType.Ice ||
                col.material == CustomBoxCollider.MaterialType.Sand)
            {
                if (col.ContainsPointOBB(pointUnderBall))
                {
                    detectedGround = col;
                    break;
                }
            }
        }

        if (detectedGround != null && ball.currentFriction != detectedGround.friction)
        {
            ball.currentFriction = detectedGround.friction;
        }

        float mass = ball.mass;
        Vector3 gravity = Physics.gravity;
        Vector3 Fgravity = gravity * mass;

        Vector3 Fparallel = Fgravity;
        Vector3 Fnormal = Vector3.zero;

        if (detectedGround != null)
        {
            Vector3 groundNormal = detectedGround.transform.up;
            Fnormal = Vector3.Project(-Fgravity, groundNormal);
            Fparallel = Fgravity + Fnormal;
        }

        float mu = ball.currentFriction;
        Vector3 Ffriction = Vector3.zero;

        if (ball.velocity.magnitude > 0.01f)
        {
            Vector3 frictionDir = -ball.velocity.normalized;
            Ffriction = frictionDir * mu * Fnormal.magnitude;
        }

        Vector3 Fair = Vector3.zero;
        if (ball.transform.position.y > 1.0f)
        {
            float v2 = ball.velocity.sqrMagnitude;
            Fair = -0.5f * ball.airDensity * v2 * ball.dragCoefficient * ball.area * ball.velocity.normalized;
        }

        Vector3 Ftotal = Fparallel + Ffriction + Fair;
        Vector3 acceleration = Ftotal / mass;

        ball.velocity += acceleration * Time.deltaTime;
        ball.transform.position += ball.velocity * Time.deltaTime;
        ball.angularVelocity = ball.velocity / ball.radius;
        if (ball.velocity.magnitude < 0.015f)
        {
            Vector3 groundNormal = detectedGround != null ? detectedGround.transform.up : Vector3.up;
            float angle = Vector3.Dot(groundNormal, Vector3.up); // 1 = horitzontal

            if (angle > 0.99f)
            {
                ball.stillTime += Time.deltaTime;
                if (ball.stillTime > 0.3f) // espera uns 0.3 segons abans de considerar-la parada
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

        CheckCollisions(ball);
    }

    void CheckCollisions(BallController ball)
    {
        CustomBoxCollider[] allColliders = FindObjectsOfType<CustomBoxCollider>();

        foreach (CustomBoxCollider col in allColliders)
        {
            Vector3 closest = col.GetClosestPointOBB(ball.transform.position);
            float distSqr = (closest - ball.transform.position).sqrMagnitude;

            if (distSqr <= ball.radius * ball.radius)
            {
                Vector3 normal = (ball.transform.position - closest).normalized;
                float penetration = ball.radius - Mathf.Sqrt(distSqr);
                ball.transform.position += normal * penetration;

                float normalSpeed = Vector3.Dot(ball.velocity, normal);
                if (normalSpeed < 0)
                {
                    Vector3 vNormal = normal * normalSpeed;
                    Vector3 vTangent = ball.velocity - vNormal;
                    Vector3 rebound = -vNormal * col.restitution;
                    ball.velocity = vTangent + rebound;
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