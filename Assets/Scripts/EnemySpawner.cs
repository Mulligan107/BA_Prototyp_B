using UnityEngine;

/// <summary>
/// Erzeugt Gegnerwellen am Rand des Spielbereichs (FA-03) und skaliert die Schwierigkeit zeitbasiert (FA-04).
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private float waveInterval = 10f;          // PW-05
    [SerializeField] private int baseEnemiesPerWave = 10;       // PW-06
    [SerializeField] private int additionalEnemiesPerStep = 5;  // PW-07
    [SerializeField] private float countStepInterval = 30f;     // PW-07
    [SerializeField] private int baseEnemyHealth = 2;           // PW-11
    [SerializeField] private int additionalHealthPerStep = 1;   // PW-08
    [SerializeField] private float baseEnemySpeed = 3f;         // PW-24
    [SerializeField] private float additionalSpeedPerStep = 0.5f; // PW-09
    [SerializeField] private float statStepInterval = 60f;      // PW-08, PW-09

    private float timer;

    private void Start()
    {
        SpawnWave();
    }

    private void Update()
    {
        if (GameManager.Instance.IsRoundOver)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer < waveInterval)
        {
            return;
        }

        timer -= waveInterval;
        SpawnWave();
    }

    private void SpawnWave()
    {
        float roundTime = GameManager.Instance.RoundTime;
        int countSteps = Mathf.FloorToInt(roundTime / countStepInterval);
        int statSteps = Mathf.FloorToInt(roundTime / statStepInterval);

        int enemyCount = baseEnemiesPerWave + additionalEnemiesPerStep * countSteps;
        int enemyHealth = baseEnemyHealth + additionalHealthPerStep * statSteps;
        float enemySpeed = baseEnemySpeed + additionalSpeedPerStep * statSteps;

        for (int i = 0; i < enemyCount; i++)
        {
            Enemy enemy = Instantiate(enemyPrefab, GetRandomBorderPosition(), Quaternion.identity);
            enemy.Initialize(enemyHealth, enemySpeed);
        }
    }

    /// <summary>Liefert eine zufällige Position auf dem gesamten Rand des Spielbereichs (PW-28).</summary>
    private Vector2 GetRandomBorderPosition()
    {
        Vector2 min = GameManager.Instance.PlayAreaMin;
        Vector2 max = GameManager.Instance.PlayAreaMax;

        float width = max.x - min.x;
        float height = max.y - min.y;
        float perimeter = 2f * (width + height);
        float offset = Random.Range(0f, perimeter);

        if (offset < width)
        {
            return new Vector2(min.x + offset, min.y);
        }
        offset -= width;

        if (offset < height)
        {
            return new Vector2(max.x, min.y + offset);
        }
        offset -= height;

        if (offset < width)
        {
            return new Vector2(max.x - offset, max.y);
        }
        offset -= width;

        return new Vector2(min.x, max.y - offset);
    }
}
