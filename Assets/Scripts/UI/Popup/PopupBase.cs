using UnityEngine;

public class PopupBase : MonoBehaviour
{
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    public virtual void OnShow()
    {
        gameObject.SetActive(true);
    }
    public virtual void OnClose()
    {
        gameObject.SetActive(false);
    }
}
