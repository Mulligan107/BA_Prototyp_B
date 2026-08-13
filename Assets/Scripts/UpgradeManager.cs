using System.Collections.Generic;
using UnityEngine;

/// <summary>Die fünf vordefinierten Upgrade-Kategorien (FA-09).</summary>
public enum UpgradeType
{
    PlayerHealth,
    PlayerSpeed,
    ProjectileSpeed,
    ProjectileDamage,
    ProjectileSize
}

/// <summary>
/// Blendet in festen Zeitabständen ein Upgrade-Menü ein (FA-08), bietet drei Optionen
/// aus dem definierten Pool an (FA-09) und wendet den gewählten Effekt kumulativ an.
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameUI gameUI;
    [SerializeField] private float upgradeInterval = 30f;              // PW-15
    [SerializeField] private int optionsPerEvent = 3;                  // PW-16
    [SerializeField] private int maxHealthBonus = 3;                   // PW-17
    [SerializeField] private float moveSpeedBonus = 1f;                // PW-18
    [SerializeField] private float projectileSpeedBonus = 1f;          // PW-19
    [SerializeField] private int projectileDamageBonus = 1;            // PW-20
    [SerializeField] private float projectileRadiusBonusPercent = 0.25f; // PW-21

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
        Time.timeScale = 0f; // PW-15: das Menü pausiert die Zeit
        gameUI.ShowUpgradeMenu(PickOptions());
    }

    /// <summary>Wählt zufällig die geforderte Anzahl unterschiedlicher Kategorien aus dem Pool.</summary>
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

    /// <summary>Wendet die gewählte Option an und setzt das Spiel fort (PW-26, PW-27).</summary>
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

    /// <summary>Beschreibung der konkreten Wirkung einer Option (FA-10).</summary>
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
