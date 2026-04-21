using UnityEngine;

public class ShootController : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 0.5f;

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if(_timer >= fireRate)
        {
            Shoot();
            _timer = 0f;
        }
    }
    void Shoot()
    {
        Instantiate(bullet, firePoint.transform.position, Quaternion.identity);
    }
}
