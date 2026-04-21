using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public GridManager gridManager;

    [Header("로봇 상태")]
    public int currentX = 2;
    public int currentY = 0;

    [Header("조작감 세팅 (숫자로 부드러움 조절)")]
    public float moveSpeed = 15f;
    public float snapSpeed = 15f;   // 손을 뗐을 때 칸에 착! 붙는 속도

    private bool _isDragging = false;
    private float _targetX; // 로봇이 실제로 향해야 할 X 좌표

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.playerController = this;

        // 시작할 때 목표 위치를 정중앙으로 세팅
        _targetX = gridManager.GetWorldPosition(currentX, currentY).x;
        transform.position = new Vector2(_targetX, gridManager.GetWorldPosition(currentX, currentY).y);
    }

    void Update()
    {
        HandleInput();
        MoveRobotSmoothly();
    }

    // 1. 터치(마우스) 입력 감지
    void HandleInput()
    {
        // 화면에 손가락이 닿았을 때 (마우스 클릭)
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
        }

        // 손가락을 떼지 않고 문지를 때 (드래그)
        if (Input.GetMouseButton(0) && _isDragging)
        {
            // 로봇의 목표 X 좌표를 내 손가락 위치로 변경
            _targetX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

            // 로봇이 게임판 밖으로 나가지 못하게 가두기
            float minX = gridManager.GetWorldPosition(0, currentY).x;
            float maxX = gridManager.GetWorldPosition(gridManager.width - 1, currentY).x;
            _targetX = Mathf.Clamp(_targetX, minX, maxX);
        }

        // 손가락을 화면에서 뗐을 때 (자석처럼 붙기)
        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            SnapToClosestLane();
        }
    }

    // 2. 가장 가까운 레인을 찾아서 목표 좌표 수정
    void SnapToClosestLane()
    {
        float minDistance = float.MaxValue;
        int closestX = currentX;

        // 0번부터 4번 레인 중 어디가 내 로봇이랑 제일 가까운지 검사
        for (int x = 0; x < gridManager.width; x++)
        {
            float laneX = gridManager.GetWorldPosition(x, currentY).x;
            float distance = Mathf.Abs(_targetX - laneX);

            if (distance < minDistance)
            {
                minDistance = distance;
                // 제일 가까운 레인 번호 저장
                closestX = x;
            }
        }

        // 제일 가까운 레인으로 데이터 확정
        currentX = closestX;
        // 목표 위치를 그 레인의 '정중앙'으로 덮어쓰기
        _targetX = gridManager.GetWorldPosition(currentX, currentY).x;
    }

    // 3. 실제로 로봇 이동
    void MoveRobotSmoothly()
    {
        float currentSpeed = _isDragging ? moveSpeed : snapSpeed;
        // 현재 위치에서 목표 위치로 미끄러지듯 이동
        float newX = Mathf.Lerp(transform.position.x, _targetX, Time.deltaTime * currentSpeed);
        // Y축은 맨 아래 고정
        float fixedY = gridManager.GetWorldPosition(currentX, currentY).y;

        transform.position = new Vector2(newX, fixedY);
    }

    public void UpgradeMoveSpeed(float amount)
    {
        moveSpeed += amount;
        Debug.Log($"[업그레이드] 이동 속도 증가! 현재: {moveSpeed}");
    }
}
