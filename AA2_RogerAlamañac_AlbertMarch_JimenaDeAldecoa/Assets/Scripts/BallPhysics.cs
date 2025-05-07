using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    // Estado de la bola
    private Vector3 velocity;
    private float radius = 0.1f;
    private float mass = 1f;

    // Fricción
    public enum Terreno { Cesped, Hielo, Arena }
    public Terreno tipoTerreno = Terreno.Cesped;
    private float coeficienteFriccion;

    // Tiempo
    private float stepTime = 0.02f;

    void Start()
    {
        velocity = Vector3.zero;
        ActualizarFriccion();
    }

    void Update()
    {
        SimularMovimiento();
    }

    void SimularMovimiento()
    {
        if (velocity.magnitude < 0.01f)
        {
            velocity = Vector3.zero;
            return;
        }

        // Aplicar fricción al movimiento
        Vector3 friccion = -coeficienteFriccion * velocity.normalized * mass * 9.81f;
        Vector3 aceleracion = friccion / mass;

        // Integración de Euler
        velocity += aceleracion * stepTime;
        transform.position += velocity * stepTime;
    }

    void ActualizarFriccion()
    {
        switch (tipoTerreno)
        {
            case Terreno.Cesped:
                coeficienteFriccion = 0.4f;
                break;
            case Terreno.Hielo:
                coeficienteFriccion = 0.1f;
                break;
            case Terreno.Arena:
                coeficienteFriccion = 0.6f;
                break;
        }
    }

    public void AplicarImpulso(Vector3 direccion, float fuerza)
    {
        velocity = direccion.normalized * fuerza;
    }
    public bool IsMoving()
    {
        return velocity.magnitude > 0.01f;
    }

}
