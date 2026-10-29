using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponentInChildren<Health>()?.TakeDamage(damage);
        Destroy(gameObject);
    }
}
