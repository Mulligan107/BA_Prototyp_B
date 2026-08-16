using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    PlayerHealth,
    PlayerSpeed,
    ProjectileSpeed,
    ProjectileDamage,
    ProjectileSize
}

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameUI gameUI;
    [SerializeField] private float upgradeInterval = 30f;
    [SerializeField] private int optionsPerEvent = 3;
    [SerializeField] private int maxHealthBonus = 3; 
    [SerializeField] private float moveSpeedBonus = 1f;
    [SerializeField] private float projectileSpeedBonus = 1f;
    [SerializeField] private int projectileDamageBonus = 1;
    [SerializeField] private float projectileRadiusBonusPercent = 0.25f;

    public bool IsMenuOpen { get; private set; }

    private readonly List<UpgradeType> pool = new List<UpgradeType>
    {
        UpgradeType.PlayerHealth,
        UpgradeType.PlayerSpeed,
        UpgradeType.ProjectileSpeed,
        UpgradeType.ProjectileDamage,
        UpgradeType.ProjectileSize
    };

    private float timer;

    private void Update()
    {
        if (GameManager.Instance.IsRoundOver || IsMenuOpen)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer < upgradeInterval)
        {
            return;
        }

        timer -= upgradeInterval;
        OpenMenu();
    }

    private void OpenMenu()
    {
        IsMenuOpen = true;
        Time.timeScale = 0f;
        gameUI.ShowUpgradeMenu(PickOptions());
    }

    
    private List<UpgradeType> PickOptions()
    {
        List<UpgradeType> remaining = new List<UpgradeType>(pool);
        List<UpgradeType> options = new List<UpgradeType>();

        int count = Mathf.Min(optionsPerEvent, remaining.Count);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, remaining.Count);
            options.Add(remaining[index]);
            remaining.RemoveAt(index);
        }

        return options;
    }
    
    public void SelectUpgrade(UpgradeType type)
    {
        if (!IsMenuOpen)
        {
            return;
        }

        Apply(type);

        IsMenuOpen = false;
        gameUI.HideUpgradeMenu();
        Time.timeScale = 1f;
    }

    private void Apply(UpgradeType type)
    {
        PlayerController player = PlayerController.Instance;
        PlayerShooter shooter = player.GetComponent<PlayerShooter>();

        switch (type)
        {
            case UpgradeType.PlayerHealth:
                player.IncreaseMaxHealth(maxHealthBonus);
                break;
            case UpgradeType.PlayerSpeed:
                player.IncreaseMoveSpeed(moveSpeedBonus);
                break;
            case UpgradeType.ProjectileSpeed:
                shooter.IncreaseProjectileSpeed(projectileSpeedBonus);
                break;
            case UpgradeType.ProjectileDamage:
                shooter.IncreaseProjectileDamage(projectileDamageBonus);
                break;
            case UpgradeType.ProjectileSize:
                shooter.IncreaseProjectileRadius(projectileRadiusBonusPercent);
                break;
        }
    }

    public string GetTitle(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.PlayerHealth:
                return "Spielergesundheit";
            case UpgradeType.PlayerSpeed:
                return "Spielergeschwindigkeit";
            case UpgradeType.ProjectileSpeed:
                return "Projektiltempo";
            case UpgradeType.ProjectileDamage:
                return "Projektilschaden";
            default:
                return "Projektilgröße";
        }
    }
    
    public string GetDescription(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.PlayerHealth:
                return "Erhöht die maximalen Lebenspunkte des Spielers um " + maxHealthBonus + " Punkte.";
            case UpgradeType.PlayerSpeed:
                return "Erhöht die Bewegungsgeschwindigkeit des Spielers um " + moveSpeedBonus + " Einheiten pro Sekunde.";
            case UpgradeType.ProjectileSpeed:
                return "Erhöht die Projektilgeschwindigkeit um " + projectileSpeedBonus + " Einheiten pro Sekunde.";
            case UpgradeType.ProjectileDamage:
                return "Erhöht den Schadenswert eines Projektils um " + projectileDamageBonus + " Punkt.";
            default:
                return "Erhöht den Projektilradius um " + Mathf.RoundToInt(projectileRadiusBonusPercent * 100f) + " Prozent des Ausgangswerts.";
        }
    }
}
