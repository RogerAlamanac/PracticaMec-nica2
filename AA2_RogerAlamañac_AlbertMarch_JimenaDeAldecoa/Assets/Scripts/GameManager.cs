using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform bola;
    public Transform hoyo;
    public float radioHoyo = 0.4f;
    public float velocidadMinima = 0.1f;
    public float tiempoEsperaReinicio = 2f;

    private BallPhysics ballPhysics;
    private bool victoriaMostrada = false;

    void Start()
    {
        ballPhysics = bola.GetComponent<BallPhysics>();
    }

    void Update()
    {
        if (victoriaMostrada) return;

        if (ballPhysics != null && HoyoAlcanzado())
        {
            victoriaMostrada = true;
            Debug.Log("¡Victoria! Bola en el hoyo");
            Invoke(nameof(ReiniciarNivel), tiempoEsperaReinicio);
        }
    }

    bool HoyoAlcanzado()
    {
        float distancia = Vector3.Distance(bola.position, hoyo.position);
        return distancia <= radioHoyo && !ballPhysics.IsMoving();
    }

    void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
