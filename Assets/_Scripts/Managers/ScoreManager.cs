using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    private float _totalScore = 0;
    private float _currentLevelScore = 0;

    public void AddScore(float addedScore, Color color)
    {
        _currentLevelScore += addedScore;
        _totalScore += addedScore;
        UIManager.Instance.SetScoreText(_currentLevelScore.ToString(),addedScore.ToString(), color);
    }

    public void ResetLevelScore()
    {
        _currentLevelScore = 0;
    }
}
