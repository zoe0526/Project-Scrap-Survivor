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
    [Header("블록 속성")]
    public EBlockColor myColor=EBlockColor.Red;
    //읽기전용 프로퍼티
    public EBlockColor MyColor => myColor;
    [Header("파괴 효과 및 보상")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject dropItemPrefab;

    [SerializeField]
    private int life = 1;
    
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
        Destroy(gameObject);
    }

}
