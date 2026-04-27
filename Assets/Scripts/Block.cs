using UnityEngine;
using TMPro;
using System.Collections;

public enum EBlockColor
{
    Red,
    Green,
    Blue,
    Yellow
};

public class Block : MonoBehaviour
{
    private EBlockColor _myColor;
    public EBlockColor MyColor => _myColor;
    [Header("블록 색상별")]
    [SerializeField] private Sprite[] _blockSpriteArr;
    
    [Header("파괴 효과 및 보상")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject dropItemPrefab;

    [SerializeField] private int _hp = 1;

    [Header("시각적 요소들")]
    private TextMeshPro _hpText;
    private SpriteRenderer _spriteRenderer;
    private Shader _originalShader;
    private Shader _whiteShader;

    private int _xCoord = -1;
    private int _yCoord = -1;
    public int XCoord => _xCoord;
    public int YCoord => _yCoord;

    private void Awake()
    {
        _hpText = transform.Find("HPText").GetComponent<TextMeshPro>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalShader = _spriteRenderer.material.shader;
        _whiteShader = Shader.Find("GUI/Text Shader");
        SetCoordinate(-1, -1);
        UpdateHPText();
    }
    public void SetColor(EBlockColor color)
    {
        _myColor = color;
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
    public void SetCoordinate(int x, int y)
    {
        _xCoord = x;
        _yCoord = y;
    }
    public void TakeDamage(int damage)
    {
        _hp -= damage;
        UpdateHPText();
        StartCoroutine(HitFlashCoroutine());
        if(_hp <= 0 )
        {
            Die();
        }
    }
    void Die()
    {
        // 1. 폭발 이펙트 생성
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // 2. 고철(경험치) 아이템 드롭
        if (dropItemPrefab != null)
        {
            Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        }

        // 3. 자기 자신 파괴
        GridManager.Instance.RemoveBlock(this);
        Destroy(gameObject);
    }

}
