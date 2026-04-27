using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public event Action<int> OnScoreChanged;

    private int _currentScore = 0;
    public int CurrentScore => _currentScore;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int points)
    {
        _currentScore += points;

        OnScoreChanged?.Invoke(_currentScore);
    }
}
