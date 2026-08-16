using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Vector2 playAreaSize = new Vector2(25f, 25f);
    [SerializeField] private float maxRoundDuration = 600f;
    [SerializeField] private int pointsPerKill = 10;
    [SerializeField] private int pointsPerSurvivedSecond = 1;

    public event System.Action RoundEnded;

    public float RoundTime { get; private set; }
    public int Score { get; private set; }
    public int Kills { get; private set; }
    public bool IsRoundOver { get; private set; }

    public Vector2 PlayAreaMin { get { return -playAreaSize * 0.5f; } }
    public Vector2 PlayAreaMax { get { return playAreaSize * 0.5f; } }

    private float secondAccumulator;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (IsRoundOver)
        {
            return;
        }

        RoundTime += Time.deltaTime;

        secondAccumulator += Time.deltaTime;
        while (secondAccumulator >= 1f)
        {
            secondAccumulator -= 1f;
            Score += pointsPerSurvivedSecond;
        }

        if (RoundTime >= maxRoundDuration)
        {
            RoundTime = maxRoundDuration;
            EndRound();
        }
    }
    
    public void RegisterKill()
    {
        Kills++;
        Score += pointsPerKill;
    }
    
    public void EndRound()
    {
        if (IsRoundOver)
        {
            return;
        }

        IsRoundOver = true;
        Time.timeScale = 0f;

        if (RoundEnded != null)
        {
            RoundEnded();
        }
    }
}
