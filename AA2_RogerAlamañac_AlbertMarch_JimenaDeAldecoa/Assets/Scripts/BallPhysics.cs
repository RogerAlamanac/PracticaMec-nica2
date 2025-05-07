using UnityEngine;

public static class BallPhysics
{
    public static Vector2 Gravity = new Vector2(0, -9.81f);
    public static float AirDensity = 1.2f;
    public static float DragCoefficient = 0.47f;

    public static (Vector2, Vector2) EulerStep(Vector2 position, Vector2 velocity, float stepTime, float mass, float radius, int terrainType)
    {
        float friction = TerrainManager.GetFriction(terrainType);
        Vector2 frictionForce = -friction * velocity.normalized * mass * Gravity.magnitude;

        Vector2 airResistance = Vector2.zero;
        if (position.y > 1f)
        {
            float area = Mathf.PI * radius * radius;
            airResistance = -0.5f * AirDensity * velocity.magnitude * velocity * DragCoefficient * area;
        }

        Vector2 netForce = mass * Gravity + frictionForce + airResistance;
        Vector2 acceleration = netForce / mass;

        Vector2 newVelocity = velocity + acceleration * stepTime;
        Vector2 newPosition = position + velocity * stepTime;

        return (newPosition, newVelocity);
    }
}
