using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [Header("UI 패널 연결")]
    [SerializeField] private SkillSelectPopup _skillPanel;
    public SkillSelectPopup skillPanel { get { return _skillPanel; } set { _skillPanel = value; } }


    private void Awake()
    {
        if(Instance==null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
