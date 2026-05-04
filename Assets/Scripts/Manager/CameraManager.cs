using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    private Vector3 _originalPos;
    private Coroutine _shakeCoroutine;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        _originalPos = transform.position;
    }
    /// <summary>
    /// 카메라 흔들어!!
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="magnitude"></param>
    public void ShakeCamera(float duration, float magnitude)
    {
        if(_shakeCoroutine!=null)
        {
            StopCoroutine(_shakeCoroutine);
            transform.position = _originalPos;
        }
        _shakeCoroutine = StartCoroutine(ShakeRoutine(duration,magnitude));
    }
    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        while(duration > 0)
        {
            // 랜덤한 위치로 카메라를 미세하게 비틀기
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(_originalPos.x + x, _originalPos.y + y, _originalPos.z);

            duration -= Time.unscaledDeltaTime;
            yield return null;
        }
        transform.position = _originalPos;
    }
}
