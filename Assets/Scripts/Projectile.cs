using UnityEngine;

/// <summary>
/// Geradlinig fliegendes Projektil, das bei Überschneidung mit einem Gegner Schaden verursacht (FA-05, FA-06).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D body;
    private Vector2 direction;
    private float speed;
    private int damage;
    private bool hasHit;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    /// <summary>Setzt Flugrichtung, Tempo, Schaden und Radius des Projektils.</summary>
    public void Launch(Vector2 flightDirection, float flightSpeed, int hitDamage, float radius)
    {
        direction = flightDirection;
        speed = flightSpeed;
        damage = hitDamage;

        // Der Kollisionsradius des Prefabs beträgt 0,5; über die Skalierung ergibt sich der Weltradius.
        transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
    }

    private void FixedUpdate()
    {
        body.MovePosition(body.position + direction * speed * Time.fixedDeltaTime);

        Vector2 min = GameManager.Instance.PlayAreaMin;
        Vector2 max = GameManager.Instance.PlayAreaMax;
        Vector2 position = body.position;

        if (position.x < min.x - 1f || position.x > max.x + 1f || position.y < min.y - 1f || position.y > max.y + 1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }

        hasHit = true;
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
