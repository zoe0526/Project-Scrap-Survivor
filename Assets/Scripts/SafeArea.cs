using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
    private Vector2 _lastScreenSize = new Vector2(0, 0);
    private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        Refresh();
    }

    void Update()
    {
        // 화면 해상도나 방향이 바뀔 때만 새로고침 (최적화)
        if (_lastSafeArea != Screen.safeArea ||
            _lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height ||
            _lastOrientation != Screen.orientation)
        {
            Refresh();
        }
    }

    void Refresh()
    {
        Rect safeArea = Screen.safeArea;

        // 현재 값 저장
        _lastSafeArea = safeArea;
        _lastScreenSize = new Vector2(Screen.width, Screen.height);
        _lastOrientation = Screen.orientation;

        // 픽셀 좌표를 0~1 사이의 앵커 값으로 변환
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // RectTransform에 적용
        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}
