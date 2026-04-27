using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [Header("블록 하강 설정")]
    [SerializeField] private float moveInterval = 5f;  // 5초마다 하강
    [SerializeField] private float moveDistance = 1f;  // 한 번에 내려올 거리 (유니티 좌표 기준 1칸)
    [SerializeField] private float gameOverYLine = -3f; // 로봇에 닿기 직전의 Y 좌표 (이 선을 넘으면 게임오버)

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        // 지정된 시간(5초)이 다 되면 하강 명령 실행
        if (timer >= moveInterval)
        {
            ShiftBlocksDown();
            timer = 0f; // 타이머 초기화
        }
    }

    private void ShiftBlocksDown()
    {
        // 씬에 살아있는 모든 'Block' 컴포넌트를 찾아서 배열로 묶음
        Block[] allBlocks = GetComponents<Block>();

        foreach (Block block in allBlocks)
        {
            // 1. 아래로 한 칸 이동
            block.transform.Translate(Vector2.down * moveDistance);

            // 2. 데드라인(게임오버 선)을 넘었는지 체크
            if (block.transform.position.y <= gameOverYLine)
            {
                TriggerGameOver();
            }
        }

        Debug.Log($"[시스템] 블록 전체 하강! 현재 남은 블록: {allBlocks.Length}개");
    }

    private void TriggerGameOver()
    {
        Debug.LogWarning("[게임 오버] 블록이 방어선을 돌파했습니다!");
        Time.timeScale = 0f; // 모든 게임 진행을 정지시킴

        // TODO: 나중에 여기에 UIManager.Instance.ShowGameOverUI(); 를 연결할 겁니다.
    }
}
