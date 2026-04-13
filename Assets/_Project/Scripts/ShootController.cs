using UnityEngine;

public class ShootController : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 0.5f;

    float timer;
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }
    void Shoot()
    {
        Instantiate(bullet, firePoint.position, Quaternion.identity);
    }
}
