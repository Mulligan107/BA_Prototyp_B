using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float shootInterval = 0.5f;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private float projectileRadius = 0.2f;

    public float ProjectileSpeed { get { return projectileSpeed; } }
    public int ProjectileDamage { get { return projectileDamage; } }
    public float ProjectileRadius { get { return projectileRadius; } }

    private float baseProjectileRadius;
    private float timer;

    private void Awake()
    {
        baseProjectileRadius = projectileRadius;
    }

    private void Update()
    {
        if (GameManager.Instance.IsRoundOver)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer < shootInterval)
        {
            return;
        }

        Enemy target = Enemy.FindNearest(transform.position);
        if (target == null)
        {
            timer = shootInterval;
            return;
        }

        timer -= shootInterval;
        Shoot(target);
    }

    private void Shoot(Enemy target)
    {
        Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Launch(direction, projectileSpeed, projectileDamage, projectileRadius);
    }

    public void IncreaseProjectileSpeed(float amount)
    {
        projectileSpeed += amount;
    }

    public void IncreaseProjectileDamage(int amount)
    {
        projectileDamage += amount;
    }
    
    public void IncreaseProjectileRadius(float percentOfBaseValue)
    {
        projectileRadius += baseProjectileRadius * percentOfBaseValue;
    }
}
