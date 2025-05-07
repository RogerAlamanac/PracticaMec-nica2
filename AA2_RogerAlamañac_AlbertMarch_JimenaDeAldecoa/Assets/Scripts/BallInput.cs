using UnityEngine;

public class BallInput : MonoBehaviour
{
    private Camera cam;
    private BallPhysics ballPhysics;

    private Vector3 dragStart;
    private Vector3 dragEnd;
    private bool isDragging = false;

    public float fuerzaMaxima = 10f;
    public LineRenderer lineRenderer;

    void Start()
    {
        cam = Camera.main;
        ballPhysics = GetComponent<BallPhysics>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ballPhysics == null || ballPhysics.IsMoving()) return;

            isDragging = true;
            dragStart = ObtenerPuntoSobrePlano();
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            dragEnd = ObtenerPuntoSobrePlano();
            DibujarLinea(dragStart, dragEnd);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            dragEnd = ObtenerPuntoSobrePlano();
            Vector3 direccion = dragStart - dragEnd;
            float fuerza = Mathf.Min(direccion.magnitude, 1f) * fuerzaMaxima;

            ballPhysics.AplicarImpulso(direccion.normalized, fuerza);
            lineRenderer.positionCount = 0;
            isDragging = false;
        }
    }

    Vector3 ObtenerPuntoSobrePlano()
    {
        Plane plano = new Plane(Vector3.up, Vector3.zero);
        Ray rayo = cam.ScreenPointToRay(Input.mousePosition);
        float distancia;

        if (plano.Raycast(rayo, out distancia))
        {
            return rayo.GetPoint(distancia);
        }

        return transform.position;
    }

    void DibujarLinea(Vector3 inicio, Vector3 fin)
    {
        if (lineRenderer == null) return;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, inicio + Vector3.up * 0.05f);
        lineRenderer.SetPosition(1, fin + Vector3.up * 0.05f);
    }
}
