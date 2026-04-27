using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("성장 시스템")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentExp = 0;
    [SerializeField] private float expToNextLevel = 2f;

    public PlayerController playerController { get; set; }
    public BeamWeapon playerWeapon { get; set; }

    private float _expIncreaseValue = 1.5f;
    void Awake()
    {
        if (Instance == null) 
            Instance = this;
        else 
            Destroy(gameObject);
    }
    public void AddExp(float amount)
    {
        currentExp += amount;
        Debug.Log($"경험치 획득: {currentExp} / {expToNextLevel}");

        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel *= _expIncreaseValue; // 요구 경험치 증가

        Debug.Log($"[레벨업!] 현재 레벨: {currentLevel}");

        UIManager.Instance.ShowPopup(UIManager.Instance.PopupManager.SkillPanel);
    }
}
