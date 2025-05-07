using UnityEngine;

public class BallController : MonoBehaviour
{
    public Transform ballObject;

    public Vector2 initialPosition = new Vector2(-4, 0);
    public Vector2 initialVelocity = new Vector2(10f, 2f);
    public float mass = 1f;
    public float radius = 0.2f;
    public int terrainType = 0;

    public Vector2 position;
    public Vector2 velocity;

    public void InitBall()
    {
        position = initialPosition;
        velocity = initialVelocity;
        ballObject.transform.position = position;
    }

    public void Step(float stepTime)
    {
        (Vector2 newPos, Vector2 newVel) = BallPhysics.EulerStep(position, velocity, stepTime, mass, radius, terrainType);

        if (Mathf.Abs(newPos.x) >= 5) newVel.x *= -0.8f;
        if (Mathf.Abs(newPos.y) >= 3) newVel.y *= -0.8f;

        position = newPos;
        velocity = newVel;
        ballObject.transform.position = position;
    }

    public bool IsStoppedNear(Vector2 holePosition, float holeRadius, float velocityThreshold)
    {
        return Vector2.Distance(position, holePosition) <= holeRadius && velocity.magnitude < velocityThreshold;
    }
}
