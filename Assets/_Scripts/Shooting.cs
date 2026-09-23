using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float rps = 5f;
    [SerializeField] private bool isAutomatic = true;
    private float shootCooldown;
    private bool canShoot = true;
    [SerializeField] private float bulletSpeed  = 10f;
    bool isShooting = false;
    private void Start()
    {
        shootCooldown = 1 / rps;
    }
    public void ShootAction(InputAction.CallbackContext context)
    {
        if (isAutomatic)
        {
            if (context.performed)
            {
                isShooting = true;
            }
            else if (context.canceled)
            {
                isShooting = false;
            }
        }
        else
        {
            isShooting = false;
            if (context.performed)
            {
                Shoot();
            }

        }
    }

    private void FixedUpdate()
    {
        if (isShooting && isAutomatic)
        {
            Shoot();
        }
    }
    void Shoot()
    {
        if (canShoot)
        {
            GameObject shot = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            if(shot.TryGetComponent<Rigidbody>(out Rigidbody bulletRB))
            {
                bulletRB.linearVelocity = shootPoint.forward * bulletSpeed;
            }
            Destroy(shot, 5f);
            StartCoroutine(ShootCooldown());
        }
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
}
