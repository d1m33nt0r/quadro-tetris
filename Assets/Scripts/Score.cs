using UnityEngine;
using UnityEngine.UI;
using System;

public class Score : MonoBehaviour
{
    private Text score;

    private void Start()
    {
        score = GetComponent<Text>();
    }

    public void AddToScore(int s)
    {
        int currentScore = Convert.ToInt32(score.text.Substring(5));
        int changedScore = currentScore + s;
        score.text = "SCORE " + Convert.ToString(changedScore);
    }

    public void ResetScore()
    {
        score.text = "SCORE 0";
    }
}
