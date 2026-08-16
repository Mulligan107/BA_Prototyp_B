using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Enemy : MonoBehaviour
{
    private static readonly List<Enemy> activeEnemies = new List<Enemy>();

    [SerializeField] private int contactDamage = 1;

    private Rigidbody2D body;
    private int health = 2;
    private float moveSpeed = 3f;

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
