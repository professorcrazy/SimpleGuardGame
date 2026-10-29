using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int maximumHealth = 100;

    public int CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maximumHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
            return;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);

        Debug.Log(
            $"{name} tog {damage} skade. " +
            $"Health: {CurrentHealth}/{maximumHealth}");

        if (CurrentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{name} døde.");
        gameObject.SetActive(false);
    }
}