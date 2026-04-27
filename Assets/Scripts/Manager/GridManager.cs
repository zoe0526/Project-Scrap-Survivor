using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [Header("그리드 설정")]
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 10;
    private float _cellSize = 1f;    //한칸 크기

    public int Width => _width;
    public int Height=> _height;

    [Header("블록 설정")]
    [SerializeField] private GameObject[] _blockPrefabs;
    [SerializeField] private float _fallInterval = 2f; // 한칸씩 밀려나는 주기
    [SerializeField] private float _slideSpeed = 5f;   // 화면에서 스르륵 미끄러지는 속도

    private Block[,] _grid;
    private bool isGameOver = false;
    private int _bottomBlockCnt = 0;
    public float FallInterval { get { return _fallInterval; } set { _fallInterval = value; } }
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

        for(int x=0; x<_width;x++)
        {
            int randomColor = Random.Range(0, 4);
            Vector2 spawnPos = GetWorldPosition(x, y) + Vector2.up * _cellSize;

            Block newBlock = BlockPoolManager.Instance.Get().GetComponent<Block>();
            newBlock.SetColor((EBlockColor)randomColor);
            newBlock.transform.position = spawnPos;
            SetGrid(x, y, newBlock);
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
        block.SetCoordinate(x, y);
    }
    IEnumerator BlockDownRoutine()
    {
        while(!isGameOver)
        {
            yield return new WaitForSeconds(_fallInterval);
            BlockDown();
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
                    SetGrid(x, y - 1, _grid[x, y]);
                    SetGrid(x, y, null);
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
                Vector2 pos = GetWorldPosition(x,y);

                // 흰색 테두리 박스 그리기
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(pos, new Vector3(_cellSize, _cellSize, 0));
            }
        }
    }
    /// <summary>
    /// 블록이 파괴되면 호출. 위에 블록 내려오기(중력)
    /// </summary>
    /// <param name="destroyedBlock"></param>

    public void RemoveBlock(Block destroyedBlock)
    {
        int foundX = destroyedBlock.XCoord;
        int foundY = destroyedBlock.YCoord;

        // 2. 빈자리 채우기 (윗줄 블록들을 한 칸씩 아래로 내림)

        SetGrid(foundX, foundY, null);

        ApplyGravityToColumn(foundX,GetLowestBlockRow());
    }

    /// <summary>
    /// 해당 줄 블록들 밑에 빈칸 존재시 끌어내리기
    /// </summary>
    void ApplyGravityToColumn(int column,int row)
    {
        for (int i = row; i < _height; i++)
        {
            //빈 칸 발견하면
            if (_grid[column, i] == null)
            {
                //빈칸 바로 위 블록부터 끌어내려준다.
                for (int j = i + 1; j < _height; j++)
                {
                    if (_grid[column, j] != null)
                    {
                        SetGrid(column, i, _grid[column, j]);
                        SetGrid(column, j, null);
                        break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 전체 그리드에서 가장 낮게 위치한 블록 y축값 찾기
    /// </summary>
    /// <returns></returns>
    private int GetLowestBlockRow()
    {
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
    private void Update()
    {
        if (isGameOver) return;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_grid[x, y] != null)
                {
                    Vector2 targetPos = GetWorldPosition(x, y);

                    // MoveTowards 대신 Lerp를 사용!
                    // 멀리 떨어져 있으면 맹렬하게 가속하고, 가까워지면 부드럽게 감속하여 '움찔' 현상이 소멸됩니다.
                    _grid[x, y].transform.position = Vector2.Lerp(
                        _grid[x, y].transform.position,
                        targetPos,
                        _slideSpeed * Time.deltaTime
                    );

                    // MoveTowards를 사용해 현재 위치에서 목표 위치로 slideSpeed만큼 부드럽게 이동
                    //_grid[x, y].transform.position = Vector2.MoveTowards(_grid[x, y].transform.position, targetPos, _slideSpeed * Time.deltaTime);
                }
            }
        }
    }

}
