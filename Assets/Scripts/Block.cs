using UnityEngine;
using TMPro;
using System.Collections;

public enum EBlockColor
{
    Red,
    Green,
    Blue,
    Yellow,
    None
};

public class Block : MonoBehaviour
{
    private EBlockColor _color;
    public EBlockColor Color => _color;
    [Header("블록 색상별")]
    [SerializeField] private Sprite[] _blockSpriteArr;

    [Header("파괴 효과 및 보상")]
    [SerializeField] private GameObject dropItemPrefab;


    [Header("시각적 요소들")]
    private TextMeshPro _hpText;
    private SpriteRenderer _spriteRenderer;
    private Shader _originalShader;
    private Shader _whiteShader;

    public int XCoord { get; private set; }
    public int YCoord { get; private set; }
    private int _hp;
    private Coroutine _moveCoroutine;
    private void Awake()
    {
        _hpText = transform.Find("HPText").GetComponent<TextMeshPro>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalShader = _spriteRenderer.material.shader;
        _whiteShader = Shader.Find("GUI/Text Shader");
    }
    public void Init(int startHP, EBlockColor color)
    {
        _hp = startHP;
        UpdateHPText();
        SetColor(color);
    }
    public void UpdateCoordinate(int x, int y)
    {
        XCoord = x;
        YCoord = y;
    }
    public void SetColor(EBlockColor color)
    {
        _color = color;
        _spriteRenderer.sprite = _blockSpriteArr[(int)color];
    }
    void UpdateHPText()
    {
        if (_hpText != null)
        {
            _hpText.text = _hp.ToString();
        }
    }
    IEnumerator HitFlashCoroutine()
    {
        if (_spriteRenderer != null && _whiteShader != null)
        {
            // 0.05초 동안 쉐이더를 강제로 하얀색으로 교체
            _spriteRenderer.material.shader = _whiteShader;
            yield return new WaitForSeconds(0.05f); // 아주 찰나의 순간 대기
            // 다시 원래 쉐이더로 복구!
            _spriteRenderer.material.shader = _originalShader;
        }
    }
    public void MoveToPosition(Vector3 targetPos, float duration = 0.15f)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(MoveRoutine(targetPos, duration));
    }
    IEnumerator MoveRoutine(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }
    public void TakeDamage(int damage)
    {
        _hp -= damage;
        UpdateHPText();
        StartCoroutine(HitFlashCoroutine());
        if (_hp <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        // 2. 고철(경험치) 아이템 드롭
        if (dropItemPrefab != null)
        {
            Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        }
        //3. 자신 폭발 및 연쇄폭발
        GridManager.Instance.StartCoroutine(GridManager.Instance.ExecuteChainReactionRoutine(this));
        DieEffect();
    }
    public void DieEffect()
    {
        // 1. 폭발 이펙트 생성
        string EffStr = "Explode_Block_" + _color.ToString() + "_Eff";
        if (System.Enum.TryParse<EEffect>(EffStr, out EEffect effect))
            EffectManager.Instance.PlayEffect(effect, transform.position);

        BlockPoolManager.Instance.ReturnToPool(gameObject); // 창고로 반납
    }
}