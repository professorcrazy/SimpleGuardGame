using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private LayerMask damageableLayer;
    [SerializeField] private LayerMask obstacleLayer;

    private Rigidbody projectileRigidbody;
    private Transform ownerRoot;
    private int damage;
    private bool hasHit;

    private void Awake()
    {
        projectileRigidbody = GetComponent<Rigidbody>();
    }

    public void Initialize(
        int damageAmount,
        Transform owner,
        Vector3 direction,
        float speed)
    {
        damage = damageAmount;
        ownerRoot = owner != null ? owner.root : null;

        IgnoreOwnerCollisions();

        projectileRigidbody.linearVelocity =
            direction.normalized * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (hasHit)
            return;

        if (ownerRoot != null &&
            other.transform.root == ownerRoot)
        {
            return;
        }

        int otherLayerMask = 1 << other.gameObject.layer;

        bool hitDamageableLayer =
            (damageableLayer.value & otherLayerMask) != 0;

        bool hitObstacleLayer =
            (obstacleLayer.value & otherLayerMask) != 0;

        if (!hitDamageableLayer && !hitObstacleLayer)
            return;

        hasHit = true;

        if (hitDamageableLayer)
        {
            Health targetHealth =
                other.gameObject.GetComponentInParent<Health>();

            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void IgnoreOwnerCollisions()
    {
        if (ownerRoot == null)
            return;

        Collider[] projectileColliders =
            GetComponentsInChildren<Collider>();

        Collider[] ownerColliders =
            ownerRoot.GetComponentsInChildren<Collider>();

        foreach (Collider projectileCollider in projectileColliders)
        {
            foreach (Collider ownerCollider in ownerColliders)
            {
                Physics.IgnoreCollision(
                    projectileCollider,
                    ownerCollider);
            }
        }
    }
}