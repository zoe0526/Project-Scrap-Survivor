using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float xLimit = 2.5f; // 화면 밖으로 나가지 않게 제한

    void Update()
    {
        Move();
    }

    private void Move()
    {
        // 1. 입력 받기 (A, D 또는 좌우 화살표)
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2. 이동 방향 계산
        Vector3 direction = new Vector3(horizontalInput, 0, 0);

        // 3. 이동 처리
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 4. 화면 이동 제한 (Clamp)
        float clampedX = Mathf.Clamp(transform.position.x, -xLimit, xLimit);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
    
}
