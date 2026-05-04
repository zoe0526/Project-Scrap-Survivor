using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [Header("그리드 설정")]
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 10;
    private float _cellSize = 1f;    //한칸 크기

    public int Width => _width;
    public int Height => _height;

    [Header("블록 설정")]
    [SerializeField] private float _fallInterval = 2f; // 한칸씩 밀려나는 주기
    [SerializeField] private float _slideSpeed = 5f;   // 화면에서 스르륵 미끄러지는 속도
    [SerializeField] private Transform _blockRoot;
    private Block[,] _grid;
    private bool isGameOver = false;
    private int _bottomBlockCnt = 0;
    private int _blockScore = 10;   //블록 하나당 점수
    public float FallInterval { get { return _fallInterval; } set { _fallInterval = value; } }
    private bool _isExplosionOnProcess = false; // 연쇄 폭발 중에는 다른 작업(중력 등)을 멈추기 위한 자물쇠
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        isGameOver = false;
        _bottomBlockCnt = 0;
    }
    void Start()
    {
        _grid = new Block[_width, _height];
        SpawnNewRow();
        StartCoroutine(BlockDownRoutine());
    }
    public Vector2 GetWorldPosition(int x, int y)
    {
        // 바둑판의 가로 길이를 계산해서, 중앙 정렬을 위한 시작점 찾기
        float startX = transform.position.x - (_width - 1) * _cellSize / 2f;
        float startY = transform.position.y - (_height - 1) * _cellSize / 2f + _cellSize;

        return new Vector2(startX + (x * _cellSize), startY + (y * _cellSize));
    }

    /// <summary>
    /// 맨위에 새로 생성되는 줄
    /// </summary>
    void SpawnNewRow()
    {
        int y = _height - 1;
        int randomColor = 0;
        bool isHorizontalMatch = false;
        for (int x = 0; x < _width; x++)
        {
            do
            {
                randomColor = Random.Range(0, 4);
                // 가로로만 3연속 같은 색인지 검사
                isHorizontalMatch = (x >= 2 &&
                       _grid[x - 1, y] != null && (int)_grid[x - 1, y].Color == randomColor &&
                       _grid[x - 2, y] != null && (int)_grid[x - 2, y].Color == randomColor);
            }
            while (isHorizontalMatch);

            Vector2 spawnPos = GetWorldPosition(x, y) + Vector2.up * _cellSize;

            Block newBlock = BlockPoolManager.Instance.Get().GetComponent<Block>();
            newBlock.transform.SetParent(_blockRoot);
            newBlock.Init(2, (EBlockColor)randomColor);
            newBlock.transform.position = spawnPos;

            SetGrid(x, y, newBlock);
            newBlock.MoveToPosition(GetWorldPosition(x,y));
        }
    }
    void SetGrid(int x, int y, Block block)
    {
        _grid[x, y] = block;
        if (block == null)
        {
            if (y == 0) _bottomBlockCnt--;
            return;
        }
        else
            if (y == 0) _bottomBlockCnt++;
        block.UpdateCoordinate(x, y);
    }
    IEnumerator BlockDownRoutine()
    {
        while (!isGameOver)
        {
            yield return new WaitUntil(()=>!_isExplosionOnProcess);
            yield return new WaitForSeconds(_fallInterval);
            if (isGameOver) break;

            BlockDown();

            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(CheckAllGridForCombosRoutine());
        }
    }
    void BlockDown()
    {
        // 1. 게임오버 체크 (맨 아랫줄에 도달했는가?)
        for (int x = 0; x < _width; x++)
        {
            if (_grid[x, 0] != null)
            {
                TriggerGameOver();
                return;
            }
        }

        // 2. 한 칸씩 아래로 덮어쓰기(실제 이동 x)
        for (int y = 1; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_grid[x, y] != null)
                {
                    Block moveBlock = _grid[x, y]; // 옮길 블록 기억
                    SetGrid(x, y - 1, moveBlock);
                    SetGrid(x, y, null);
                    moveBlock.MoveToPosition(GetWorldPosition(x, y - 1));
                }
            }
        }

        // 3. 맨 윗줄 새로 생성
        SpawnNewRow();
    }
    private void TriggerGameOver()
    {
        isGameOver = true;
        Debug.LogWarning("[게임 오버] 블록이 방어선에 도달했습니다!");
        Time.timeScale = 0f;
    }
    private void OnDrawGizmos()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2 pos = GetWorldPosition(x, y);

                // 흰색 테두리 박스 그리기
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(pos, new Vector3(_cellSize, _cellSize, 0));
            }
        }
    }


    /// <summary>
    /// 인자로 받은 Block 과 같은 색상의 연결된 블록들 모두 찾기
    /// </summary>
    /// <param name="startBlock"></param>
    /// <returns></returns>
    List<Block> FindConnectedBlocks(Block startBlock)
    {
        List<Block> connectedBlocks = new List<Block>();

        if (startBlock == null)
            return connectedBlocks;

        int startX = startBlock.XCoord;
        int startY = startBlock.YCoord;
        EBlockColor blockColor = startBlock.Color;

        Queue<Block> queue = new Queue<Block>();
        bool[,] visited = new bool[_width, _height]; // 이미 검사한 곳인지 체크

        Vector2[] dCoor = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(1, 0) };
        queue.Enqueue(startBlock);
        visited[startX, startY] = true;
        connectedBlocks.Add(startBlock);

        while (queue.Count > 0)
        {
            Block current = queue.Dequeue();

            // 상하좌우 4방향을 찔러보기
            for (int i = 0; i < 4; i++)
            {
                int nx = current.XCoord + (int)dCoor[i].x;
                int ny = current.YCoord + (int)dCoor[i].y;

                // 그리드 맵을 벗어나지 않았고, 아직 방문 안 한 곳이라면?
                if (nx >= 0 && nx < _width && ny >= 0 && ny < _height && !visited[nx, ny])
                {
                    Block neighbor = _grid[nx, ny];

                    // 그 자리에 블록이 있고, 색깔마저 똑같다면!
                    if (neighbor != null && neighbor.Color == blockColor)
                    {
                        visited[nx, ny] = true; // 출석 체크
                        queue.Enqueue(neighbor); // 너도 내 동료가 되라 (다음 탐색 대상)
                        connectedBlocks.Add(neighbor);    // 폭발 리스트에 추가
                    }
                }
            }
        }

        return connectedBlocks;
    }
    /// <summary>
    /// 연쇄 폭발
    /// </summary>
    /// <param name="targetBlock"></param>
    public IEnumerator ExecuteChainReactionRoutine(Block targetBlock)
    {
        _isExplosionOnProcess = true;
        List<Block> connectedBlocks = FindConnectedBlocks(targetBlock);

        //콤보 조건(3개 이상 뭉쳐야 터진다고 가정)
        if (connectedBlocks.Count >= 3)
        {
            ScoreManager.Instance.AddScore(_blockScore * connectedBlocks.Count);
            // 0.2초 동안 0.3의 강도로 강하게 쉐이크
            CameraManager.Instance.ShakeCamera(0.2f, 0.3f);
            Debug.Log($"<color=cyan>연쇄 폭발! {connectedBlocks.Count} COMBO!</color>");

            //콤보 화면에 표기
            GameObject popupObj = ComboTextPoolManager.Instance.Get();
            popupObj.transform.position = targetBlock.transform.position;
            popupObj.GetComponent<FloatingText>().Setup($"{connectedBlocks.Count} COMBO!", Color.yellow, 6f);

            HashSet<int> columnsToUpdate = new HashSet<int>();
            // 찾은 블록 전부 터뜨린다
            foreach (Block block in connectedBlocks)
            {
                int x = block.XCoord;
                int y = block.YCoord;
                columnsToUpdate.Add(x);
                SetGrid(x, y, null);
                block.DieEffect();
            }
            bool needsToWait = false;
            foreach (var col in columnsToUpdate)
            {// 해당 열을 압축했는데 떨어진 블록이 하나라도 있다면 대기열 추가
                if (ApplyGravityToColumn(col)) needsToWait = true;
            }
            // 블록이 떨어졌을 때만! 딱 떨어지는 시간(0.15초)만큼만 빠릿하게 대기
            if (needsToWait) 
                yield return new WaitForSeconds(0.15f);
            yield return StartCoroutine(CheckAllGridForCombosRoutine());
        }
        else
        {
            ScoreManager.Instance.AddScore(_blockScore);
            //0.05초 동안 0.1의 강도로 찌릿!
            CameraManager.Instance.ShakeCamera(0.05f, 0.1f);
            // 3개가 안 뭉쳐서 터지지 않는 경우 타겟 블록 한개만 지우기
            SetGrid(targetBlock.XCoord, targetBlock.YCoord, null);

            // 1. 해당 열만 중력 적용
            bool didMove = ApplyGravityToColumn(targetBlock.XCoord);

            // 2. 윗 블록이 떨어졌다면 딱 0.15초만 대기
            if (didMove) 
                yield return new WaitForSeconds(0.15f);
            yield return StartCoroutine(CheckAllGridForCombosRoutine());
        }
    }
    /// <summary>
    /// 중력 적용
    /// </summary>
    bool ApplyGravityToColumn(int column)
    {
        int writeY = GetGlobalFloorY();
        bool isMove = false;
        //빈칸 찾을때까지 한칸씩 위로 전진
        while (writeY < _height && _grid[column, writeY] != null)
            writeY++;

        // 터진 곳 바로 위부터 맨 윗줄까지 훑으면서
        for (int readY = writeY + 1; readY < _height; readY++)
        {
            if (_grid[column, readY] != null) // 살아있는 블록을 발견하면
            {
                Block moveBlock = _grid[column, readY];
                SetGrid(column, writeY, moveBlock);
                SetGrid(column, readY, null);

                moveBlock.MoveToPosition(GetWorldPosition(column, writeY));

                // 구멍이 한 칸 위로 올라갔으니 바닥도 올려준다
                writeY++;
                isMove = true;
            }
        }
        return isMove;
    }
    /// <summary>
    /// 그리드에서 가장 낮은 블록 높이 찾기
    /// </summary>
    /// <returns></returns>
    private int GetGlobalFloorY()
    {
        // 바닥부터 위로 올라가면서 블록이 하나라도 있는지 검사
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_grid[x, y] != null)
                    return y;
            }
        }
        return _height - 1;
    }
    /// <summary>
    /// 판 전체를 훑어서 터질 게 있는지 검사하고, 있으면 터뜨리고 다시 자신을 호출합니다 (무한 연쇄)
    /// </summary>
    private IEnumerator CheckAllGridForCombosRoutine()
    {
        _isExplosionOnProcess = true;

        bool[,] visited = new bool[_width, _height];
        List<Block> blocksToDestroy = new List<Block>();

        //전체 그리드 검사
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Block block = _grid[x, y];

                // 블록이 있고, 아직 검사 안 한 녀석이라면?
                if (block != null && !visited[x, y])
                {
                    List<Block> connected = FindConnectedBlocks(block);

                    // 검사 완료 도장 찍기 (중복 검사 방지)
                    foreach (Block c in connected)
                    {
                        visited[c.XCoord, c.YCoord] = true;
                    }

                    // 3개 이상 뭉쳐있다면 연쇄폭발 명단에 추가
                    if (connected.Count >= 3)
                    {
                        blocksToDestroy.AddRange(connected);
                    }
                }
            }
        }

        // 연쇄폭발
        if (blocksToDestroy.Count > 0)
        {
            // 점수 주고 카메라 더 쎄게 흔들기
            ScoreManager.Instance.AddScore(_blockScore * blocksToDestroy.Count);
            CameraManager.Instance.ShakeCamera(0.25f, 0.4f);
            Debug.Log($"<color=yellow>🌟 2차 연쇄 폭발! 추가 {blocksToDestroy.Count}개 파괴!</color>");

            //콤보 화면에 표기
            GameObject popupObj = ComboTextPoolManager.Instance.Get();
            popupObj.transform.position = blocksToDestroy[0].transform.position;
            popupObj.GetComponent<FloatingText>().Setup($"{blocksToDestroy.Count} COMBO!", Color.yellow, 6f);

            HashSet<int> columnsToUpdate = new HashSet<int>();

            // 명단에 있는 애들 싹 다 터뜨리기
            foreach (Block block in blocksToDestroy)
            {
                int x = block.XCoord;
                int y = block.YCoord;

                columnsToUpdate.Add(x);

                SetGrid(x, y, null);
                block.DieEffect();
            }

            // 빈칸 채우기 (스마트 중력)
            foreach (var col in columnsToUpdate)
            {
                ApplyGravityToColumn(col);
            }

            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(CheckAllGridForCombosRoutine());
        }
        else
        {
            _isExplosionOnProcess = false;
            Debug.Log("<color=grey>연쇄 반응 종료.</color>");
        }
    }

    private void Update()
    {
        if (isGameOver) return;

    }

}


