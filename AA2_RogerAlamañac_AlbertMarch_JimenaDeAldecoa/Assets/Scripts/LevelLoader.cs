using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public BallController ball;
    public Transform goal;

    void Update()
    {
        float distance = Vector3.Distance(ball.transform.position, goal.position);
        float speed = ball.velocity.magnitude;

        if (distance < 0.5f && speed < 0.5f)
        {
            Debug.Log("Nivell completat!");
            LoadNextLevel();
        }

        if (ball.transform.position.y <= -5f)
        {
            RestartPosition();
        }
    }

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

    void RestartPosition()
    {
        int restartScene = SceneManager.GetActiveScene().buildIndex;
        if (restartScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(restartScene);
        }
    }
}