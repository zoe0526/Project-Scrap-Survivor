using System.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("그리드 설정")]
    private int width = 5;
    private int height = 10;
    private float cellSize = 1f;    //한칸 크기

    [Header("블록 설정")]
    [SerializeField] public GameObject blockPrefeb;
    [SerializeField] public float fallSpeed;


    public GameObject[,] _grid;

    void Start()
    {
        _grid = new GameObject[width, height];
        SpawnTestBlock();
        StartCoroutine(BlockDownRoutine());
    }
    Vector2 GetWorldPosition(int x, int y)
    {
        // 바둑판의 가로 길이를 계산해서, 중앙 정렬을 위한 시작점 찾기
        float startX = transform.position.x - (width - 1) * cellSize / 2f;
        float startY = transform.position.y;

        return new Vector2(startX + (x * cellSize), startY + (y * cellSize));
    }
    void SpawnTestBlock()
    {
        // 1. 소환할 좌표(인덱스) 결정: 정중앙 윗줄
        int x = (width - 1) / 2;
        int y = height-1;
        Vector2 spawnPos = GetWorldPosition(x,y);
        // 2. 블록 생성
        GameObject testBlock = Instantiate(blockPrefeb,spawnPos,Quaternion.identity);
        // 3. 생성된 블록 기록
        _grid[x, y] = testBlock;

    }
    IEnumerator BlockDownRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(fallSpeed);
            BlockDown();
        }
    }
    void BlockDown()
    {
        for(int y=1 ; y<height ; y++)
        {
            for(int x=0 ; x<width ; x++)
            {
                if (_grid[x, y] != null)
                {
                    _grid[x, y - 1] = _grid[x, y];
                    _grid[x, y] = null;

                    _grid[x, y - 1].transform.position = GetWorldPosition(x,y-1);
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = GetWorldPosition(x,y);

                // 흰색 테두리 박스 그리기
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, cellSize, 0));
            }
        }
    }

}
