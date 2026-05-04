using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMesh;
    [SerializeField] private float _moveSpeed = 2f; // 위로 떠오르는 속도
    [SerializeField] private float _lifeTime = 1f;  // 살아있는 시간
    public void Setup(string text, Color color, float size = 5f)
    {
        _textMesh.text = text;
        _textMesh.color = color;
        _textMesh.fontSize = size;

        StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        float timer = 0f;
        Color startColor = _textMesh.color;

        Vector3 startScale = transform.localScale;
        transform.localScale = startScale * 1.5f;

        while (timer < _lifeTime)
        {
            // 1. 위로 스르륵 이동
            transform.position += Vector3.up * _moveSpeed * Time.deltaTime;

            // 2. 스케일 서서히 원상복구 (초반 0.2초 동안만)
            if (timer < 0.2f)
                transform.localScale = Vector3.Lerp(transform.localScale, startScale, timer / 0.2f);

            // 3. 서서히 투명하게 페이드아웃
            float alpha = Mathf.Lerp(1f, 0f, timer / _lifeTime);
            _textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            timer += Time.deltaTime;
            yield return null;
        }
        ComboTextPoolManager.Instance.ReturnToPool(gameObject);
    }
}
