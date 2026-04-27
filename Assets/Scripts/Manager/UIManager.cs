using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private PopupManager _popupManager;
    public PopupManager PopupManager => _popupManager;

    private void Awake()
    {
        if(Instance==null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void ShowPopup(PopupBase popup)
    {
        popup.OnShow();
    }
}
