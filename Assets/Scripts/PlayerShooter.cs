using UnityEngine;

/// <summary>
/// Erzeugt ohne Benutzereingabe in festem Intervall Projektile in Richtung des nächsten Gegners (FA-02).
/// </summary>
public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float shootInterval = 0.5f;    // PW-02
    [SerializeField] private float projectileSpeed = 8f;    // PW-03
    [SerializeField] private int projectileDamage = 1;      // PW-04
    [SerializeField] private float projectileRadius = 0.2f; // PW-25

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
        projectileSpeed += amount; // PW-19
    }

    public void IncreaseProjectileDamage(int amount)
    {
        projectileDamage += amount; // PW-20
    }

    /// <summary>Erhöht den Projektilradius um einen Anteil des Ausgangswerts (PW-21).</summary>
    public void IncreaseProjectileRadius(float percentOfBaseValue)
    {
        projectileRadius += baseProjectileRadius * percentOfBaseValue;
    }
}
