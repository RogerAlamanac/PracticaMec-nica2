using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public BallController ball;
    public Transform goal;

    void Awake()
    {
        // Troba automàticament la pilota a l’escena
        ball = FindObjectOfType<BallController>();
    }

    void Update()
    {
        // Calcula la distància entre la pilota i l'objectiu
        float distance = Vector3.Distance(ball.transform.position, goal.position);

        // Velocitat actual de la pilota
        float speed = ball.velocity.magnitude;

        // Comprova si la pilota ha arribat a l’objectiu i està gairebé aturada
        if (distance < 0.5f && speed < 0.5f)
        {
            Debug.Log("Nivell completat!");
            LoadNextLevel();
        }

        // Si cau del nivell o fa massa rebots, reinicia
        if (ball.transform.position.y <= -5f || ball.reboundCount > ball.maxRebounds)
        {
            RestartPosition();
        }
    }

    // Carrega el següent nivell si n'hi ha més
    void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("Has completat tots els nivells!");
        }
    }

    // Reinicia l’escena actual si la pilota ha sortit del mapa o ha superat els rebots permesos
    void RestartPosition()
    {
        int restartScene = SceneManager.GetActiveScene().buildIndex;
        if (restartScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(restartScene);
        }
    }
}