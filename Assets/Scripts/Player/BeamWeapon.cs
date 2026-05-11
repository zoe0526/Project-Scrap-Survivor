using System.Collections;
using UnityEngine;

public class BeamWeapon : MonoBehaviour
{
    [Header("블록 정보 사전")]
    [SerializeField] private BlockConfig _blockConfig;
    [Header("무기 상태")]
    [SerializeField] private EBlockType _currMode = EBlockType.Red;  //기본 빔 색은 빨강

    [Header("시각 효과")]
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("빔 값 설정")]
    [SerializeField] private float _beamSpeed = 40f;
    [SerializeField] private Transform _shootPoint;
    //블록 파괴 성공시 딜레이 시간
    private float _fireRate = .2f;
    private float _fireTimer = 0f;

    private Vector2 _currEndPos;
    [SerializeField] private GameObject _muzzleEffect;
    private Material _muzzleMaterial;
    private Color _muzzlebaseColor;

    private Material _lazerMaterial;
    private Color _lazerColor;

    private void Awake()
    {
        _muzzleMaterial = _muzzleEffect.transform.Find("MuzzleFlash").GetComponent<SpriteRenderer>().material;
        _muzzlebaseColor = _muzzleMaterial.GetColor(Constants.Shaders.EmissionColor);

        _lazerMaterial = _lineRenderer.material;
    }
    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.playerWeapon = this;
        //빔 끝점을 로봇 시작 위치로 초기화
        _currEndPos = transform.position;
    }
    IEnumerator FlashMuzzle()
    {
        float duration = 0.15f;
        float elapsed = 0f;
        float startIntensity = 5f;
        _muzzleEffect.SetActive(true);
        while(elapsed < duration)
        {
            elapsed+= Time.deltaTime;
            float currIntensity = Mathf.Lerp(startIntensity,0f, elapsed / duration);
            _muzzleMaterial.SetColor(Constants.Shaders.EmissionColor, _muzzlebaseColor * currIntensity);
            _lazerColor = _blockConfig.GetData(_currMode).color;
            _lazerMaterial.SetColor(Constants.Shaders.EmissionColor, _lazerColor * currIntensity);
            yield return null;
        }
        _muzzleEffect.SetActive(false);
    }
    void FireBeam()
    {
        StartCoroutine(FlashMuzzle());
        _lineRenderer.enabled = true;
        Vector3 startPos = _shootPoint.position;
        _lineRenderer.SetPosition(0,startPos);

        // 위쪽 레이저를 쏴서 닿는 모든 것을 저장
        RaycastHit2D[] rayCastHits = Physics2D.RaycastAll(transform.position, Vector3.up,20f);  //그리드 크기 바뀌면 설정 확인
        // 맞은 물체들을 아래에서 위 순서로 정렬
        System.Array.Sort(rayCastHits,(a,b)=>a.distance.CompareTo(b.distance));

        Vector3 targetEndPos = startPos + Vector3.up * 20f;
        Block targetBlock = null;
        foreach(RaycastHit2D hit in rayCastHits)
        {
            Block block = hit.collider.GetComponent<Block>();
            if (block == null)
                continue;
            if (block.MyBlockData.type == _currMode)
                continue;

            targetBlock = block;
            targetEndPos = hit.point;
            break;
        }

        //시각연출 : 빔 연장
        _currEndPos.x = startPos.x;
        if (targetEndPos.y < _currEndPos.y)
        {
            //위에 블록이 있을경우 광선 가로막기
            _currEndPos.y = targetEndPos.y;
        }
        else
        {
            //위에 블록 없을경우 광선을 서서히 뻗는다.
            _currEndPos.y = Mathf.MoveTowards(_currEndPos.y, targetEndPos.y, _beamSpeed * Time.deltaTime);
        }

        //빔이 타겟 도달시 블록 파괴
        if(targetBlock!=null && _currEndPos.y>= targetEndPos.y-0.05f)
        {
            if (System.Enum.TryParse<EEffect>(EEffect.Hit_Spark_Eff.ToString(), out EEffect effect))
                EffectManager.Instance.PlayEffect(effect, targetEndPos);
            targetBlock.TakeDamage(1);
            _fireTimer = Time.time + _fireRate;
        }
        _lineRenderer.SetPosition(1, _currEndPos);
    }

    #region 스킬 업그레이드 함수들

    //1. 빔 속도 증가
    public void UpgradeBeamSpeed(float amount)
    {
        _beamSpeed += amount;
        Debug.Log($"[업그레이드] 빔 스피드 증가! 현재: {_beamSpeed}");
    }
    // 2. 빔 굵기 증가
    public void UpgradeBeamWidth(float amount)
    {
        _lineRenderer.startWidth += amount;
        _lineRenderer.endWidth += amount;
        Debug.Log($"[업그레이드] 빔 굵기 증가! 현재: {_lineRenderer.startWidth}");
    }
    #endregion

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if(Time.time < _fireTimer)
            {
                _lineRenderer.enabled = false;
                _currEndPos = transform.position;
            }
            else
                FireBeam();
        }
        else
        {
            _lineRenderer.enabled = false; // 손을 떼면 빔 끄기
            _currEndPos = transform.position;
        }
    }
}
