using UnityEngine;

public enum EBlockColor
{
    Red,
    Green,
    Blue,
    Yellow
};

public class Block : MonoBehaviour
{
    [Header("BlockSetting")]
    public EBlockColor blockColor;
    [SerializeField]
    private float fallSpeed = 2f;
    [SerializeField]
    private int life = 1;


    private void Start()
    {
        
    }

    public void TakeDamage(int damage)
    {
        life -= damage;
        if(life <= 0 )
        {
            Explode();
        }
    }
    void Explode()
    {
        // 폭발 이펙트 생성 (나중에 파티클 연결)
        Destroy(gameObject);
    }

    private void Update()
    {
        // 1. 아래로 지속 이동
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        // 2. 화면 밖(바닥)으로 나가면 삭제
        if (transform.position.y <= -6f)
        {
            Destroy(gameObject);
            //추후 플레이어 체력 감소
        }
    }
}
