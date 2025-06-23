using System.Drawing;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool gameRunning = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            StartGame();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame();
        }
    }

    public void StartGame()
    {
        if (!gameRunning) // Replace with your desired input condition
        {
            gameRunning = true;
            PlayerController.instance.StartPlayerGameplay();
            SpawnItems.instance.ToggleSpawning(true);
        }
    }

    public void EndGame()
    {
        if (gameRunning) // Replace with your desired input condition
        {
            ScoreManager.instance.UpgradeHighScore();
            ScoreManager.instance.ResetPoints();
            gameRunning = false;
            PlayerController.instance.EndPlayerGameplay();
            SpawnItems.instance.ToggleSpawning(false);
        }
    }
}
