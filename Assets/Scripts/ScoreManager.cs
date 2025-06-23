using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField]
    private int points;

    [SerializeField]
    private Text scoreText;

    private static int highScore;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            UpdateScoreText(highScore.ToString());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int Points
    {
        get { return points; }
        set { points = value; }
    }

    public int HighScore
    {
        get { return highScore; }
        set { highScore = value; }
    }

    public void UpdateScoreText(string highScore)
    {
        scoreText.text = highScore;
    }

    public void IncreasePoints(int amount = 1)
    {
        points += amount;
        UpdateScoreText(points.ToString());
    }

    public void ResetPoints()
    {
        points = 0;
        UpdateScoreText(points.ToString());
    }

    public void UpgradeHighScore()
    {
        if (points > highScore)
        {
            highScore = points;
        }
    }
}
