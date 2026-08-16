using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private UpgradeManager upgradeManager;

    [SerializeField] private Text healthText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timeText;

    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button[] upgradeButtons;
    [SerializeField] private Text[] upgradeTitleTexts;
    [SerializeField] private Text[] upgradeDescriptionTexts;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverText;

    private readonly List<UpgradeType> currentOptions = new List<UpgradeType>();

    private void Start()
    {
        upgradePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            int index = i;
            upgradeButtons[i].onClick.AddListener(delegate { OnOptionSelected(index); });
        }

        GameManager.Instance.RoundEnded += ShowGameOverScreen;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RoundEnded -= ShowGameOverScreen;
        }
    }

    private void Update()
    {
        PlayerController player = PlayerController.Instance;
        GameManager game = GameManager.Instance;

        healthText.text = "Leben: " + player.CurrentHealth + " / " + player.MaxHealth;
        scoreText.text = "Punkte: " + game.Score;
        timeText.text = "Zeit: " + FormatTime(game.RoundTime);
    }
    
    public void ShowUpgradeMenu(List<UpgradeType> options)
    {
        currentOptions.Clear();
        currentOptions.AddRange(options);

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            bool hasOption = i < currentOptions.Count;
            upgradeButtons[i].gameObject.SetActive(hasOption);

            if (hasOption)
            {
                upgradeTitleTexts[i].text = upgradeManager.GetTitle(currentOptions[i]);
                upgradeDescriptionTexts[i].text = upgradeManager.GetDescription(currentOptions[i]);
            }
        }

        upgradePanel.SetActive(true);
    }

    public void HideUpgradeMenu()
    {
        upgradePanel.SetActive(false);
    }

    private void OnOptionSelected(int index)
    {
        if (index >= currentOptions.Count)
        {
            return;
        }

        upgradeManager.SelectUpgrade(currentOptions[index]);
    }

    private void ShowGameOverScreen()
    {
        GameManager game = GameManager.Instance;

        upgradePanel.SetActive(false);
        gameOverText.text = "Runde beendet\n\n"
            + "Punkte: " + game.Score + "\n"
            + "Überlebenszeit: " + FormatTime(game.RoundTime) + "\n"
            + "Besiegte Gegner: " + game.Kills;

        gameOverPanel.SetActive(true);
    }

    private static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        return (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
    }
}
