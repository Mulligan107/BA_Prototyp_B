using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Standardgegner: läuft auf den Spieler zu (FA-15), verursacht bei Kontakt Schaden (FA-05, FA-06)
/// und besitzt eigene Lebenspunkte.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Enemy : MonoBehaviour
{
    private static readonly List<Enemy> activeEnemies = new List<Enemy>();

    [SerializeField] private int contactDamage = 1; // PW-12

    private Rigidbody2D body;
    private int health = 2;      // PW-11
    private float moveSpeed = 3f; // PW-24

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        activeEnemies.Add(this);
    }

    private void OnDisable()
    {
        activeEnemies.Remove(this);
    }

    /// <summary>Setzt die vom Spawner ermittelten Werte für Lebenspunkte und Tempo.</summary>
    public void Initialize(int startHealth, float startSpeed)
    {
        health = startHealth;
        moveSpeed = startSpeed;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsRoundOver || PlayerController.Instance == null)
        {
            return;
        }

        Vector2 playerPosition = PlayerController.Instance.transform.position;
        Vector2 direction = (playerPosition - body.position).normalized;
        body.MovePosition(body.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(contactDamage);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            GameManager.Instance.RegisterKill();
            Destroy(gameObject);
        }
    }

    /// <summary>Liefert den zur Position nächstgelegenen Gegner oder null.</summary>
    public static Enemy FindNearest(Vector3 position)
    {
        Enemy nearest = null;
        float nearestSqrDistance = float.MaxValue;

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            Enemy candidate = activeEnemies[i];
            float sqrDistance = ((Vector2)(candidate.transform.position - position)).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearest = candidate;
            }
        }

        return nearest;
    }
}
