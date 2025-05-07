using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BallController ballController;
    public Transform holeObject;

    public float stepTime = 0.01f;
    public float totalTime = 20f;
    public float holeRadius = 0.5f;
    public float maxHoleSpeed = 0.5f;

    private float time = 0f;
    private bool gameEnded = false;

    void Start()
    {
        ballController.InitBall();
    }

    void Update()
    {
        if (gameEnded || time >= totalTime) return;

        ballController.Step(stepTime);
        time += stepTime;

        if (ballController.IsStoppedNear(holeObject.position, holeRadius, maxHoleSpeed))
        {
            gameEnded = true;
            Debug.Log("🎯 ¡Victoria! Bola en el hoyo.");
        }
    }
}
