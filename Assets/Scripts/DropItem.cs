using UnityEngine;

public class DropItem : MonoBehaviour
{
    [Header("고철 설정")]
    [SerializeField] private float expAmount = 2f;
    [SerializeField] private float fallSpeed = 3f;

    void Update()
    {
        // 밑으로 툭 떨어짐
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        // 화면 밖으로 나가면 삭제
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddExp(expAmount);
            Destroy(gameObject);
        }
    }
}
