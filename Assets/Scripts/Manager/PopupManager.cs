using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [Header("UI 패널 연결")]
    [SerializeField] private SkillSelectPopup _skillPanel;
    public SkillSelectPopup SkillPanel { get { return _skillPanel; } set { _skillPanel = value; } }

}
