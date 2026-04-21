using UnityEngine;
using System.Linq;

public class BeamWeapon : MonoBehaviour
{
    [Header("무기 상태")]
    [SerializeField] private EBlockColor currMode = EBlockColor.Red;  //기본 빔 색은 빨강

    [Header("시각 효과")]
    [SerializeField] private LineRenderer lineRenderer;

    [Header("빔 값 설정")]
    [SerializeField] private float beamSpeed = 40f;
    private Vector2 _currEndPos;

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.playerWeapon = this;
        //빔 끝점을 로봇 시작 위치로 초기화
        _currEndPos = transform.position;
    }

    void FireBeam()
    {
        lineRenderer.enabled = true;

        Vector2 startPos = transform.position;
        lineRenderer.SetPosition(0,startPos);
        // 위쪽 레이저를 쏴서 닿는 모든 것을 저장
        RaycastHit2D[] rayCastHits = Physics2D.RaycastAll(transform.position, Vector2.up,20f);  //그리드 크기 바뀌면 설정 확인

        // 맞은 물체들을 아래에서 위 순서로 정렬
        rayCastHits = rayCastHits.OrderBy(r=>r.distance).ToArray();

        Vector2 targetEndPos = startPos + Vector2.up * 20f;
        Block targetBlock = null;
        foreach(RaycastHit2D hit in rayCastHits)
        {
            Block block = hit.collider.GetComponent<Block>();
            if (block == null)
                continue;
            if (block.myColor == currMode)
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
            _currEndPos.y = Mathf.MoveTowards(_currEndPos.y, targetEndPos.y,beamSpeed * Time.deltaTime);
        }

        //빔이 타겟 도달시 블록 파괴
        if(targetBlock!=null && _currEndPos.y>= targetEndPos.y-0.05f)
        {
            targetBlock.TakeDamage(1);
        }
        lineRenderer.SetPosition(1, _currEndPos);
    }

    #region 스킬 업그레이드 함수들

    //1. 빔 속도 증가
    public void UpgradeBeamSpeed(float amount)
    {
        beamSpeed += amount;
        Debug.Log($"[업그레이드] 빔 스피드 증가! 현재: {beamSpeed}");
    }
    // 2. 빔 굵기 증가
    public void UpgradeBeamWidth(float amount)
    {
        lineRenderer.startWidth += amount;
        lineRenderer.endWidth += amount;
        Debug.Log($"[업그레이드] 빔 굵기 증가! 현재: {lineRenderer.startWidth}");
    }
    #endregion

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            FireBeam();
        }
        else
        {
            lineRenderer.enabled = false; // 손을 떼면 빔 끄기
            _currEndPos = transform.position;
        }
    }
}
