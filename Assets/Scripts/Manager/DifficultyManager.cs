using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField] private float speedIncreaseAmount = 0.1f; // 난이도 상승 시 속도 증가량
    [SerializeField] private int scoreThreshold = 500;       // 몇 점마다 난이도를 올릴 것인가?

    private int _lastDifficultyLevel = 0;

    private void OnEnable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += CheckDifficultyUp;
        }
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= CheckDifficultyUp;
        }
    }

    private void CheckDifficultyUp(int currentScore)
    {
        // 현재 점수를 기준으로 난이도 레벨 계산
        int currentDifficultyLevel = currentScore / scoreThreshold;

        // 이전 난이도보다 높아졌다면?
        if (currentDifficultyLevel > _lastDifficultyLevel)
        {
            _lastDifficultyLevel = currentDifficultyLevel;
            ApplyDifficulty();
        }
    }

    private void ApplyDifficulty()
    {
        GridManager.Instance.FallInterval *= speedIncreaseAmount;
        Debug.Log($"<color=red>난이도 상승!</color> 현재 난이도: {_lastDifficultyLevel}");
    }
}
