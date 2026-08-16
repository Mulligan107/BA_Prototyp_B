using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float invulnerabilityDuration = 0.5f;

    public int MaxHealth { get { return maxHealth; } }
    public int CurrentHealth { get; private set; }
    public float MoveSpeed { get { return moveSpeed; } }

    private Rigidbody2D body;
    private float radius;
    private float invulnerableUntil;

    private void Awake()
    {
        Instance = this;
        body = GetComponent<Rigidbody2D>();
        CurrentHealth = maxHealth;

        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        radius = circle.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsRoundOver)
        {
            return;
        }

        Vector2 target = body.position + ReadMoveInput() * moveSpeed * Time.fixedDeltaTime;

        Vector2 min = GameManager.Instance.PlayAreaMin;
        Vector2 max = GameManager.Instance.PlayAreaMax;
        target.x = Mathf.Clamp(target.x, min.x + radius, max.x - radius);
        target.y = Mathf.Clamp(target.y, min.y + radius, max.y - radius);

        body.MovePosition(target);
    }
    
    private Vector2 ReadMoveInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return Vector2.zero;
        }

        float x = 0f;
        float y = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            x -= 1f;
        }
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            x += 1f;
        }
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            y -= 1f;
        }
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            y += 1f;
        }

        return new Vector2(x, y).normalized;
    }
    
    public void TakeDamage(int amount)
    {
        if (CurrentHealth <= 0 || Time.time < invulnerableUntil)
        {
            return;
        }

        CurrentHealth -= amount;
        invulnerableUntil = Time.time + invulnerabilityDuration;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            GameManager.Instance.EndRound();
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        CurrentHealth += amount;
    }

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }
}
