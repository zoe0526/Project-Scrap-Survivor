using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI _scoreText;

    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScoreDisplay;
            UpdateScoreDisplay(ScoreManager.Instance.CurrentScore);
        }
    }

    void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
        }
    }

    // 방송이 울릴 때마다 자동으로 실행될 함수
    private void UpdateScoreDisplay(int newScore)
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"SCORE: {newScore:#,##0}";
        }
    }
}
