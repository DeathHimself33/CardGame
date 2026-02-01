using UnityEngine;
using TMPro;
using CardGame;

public class RestView : MonoBehaviour
{
    public TMP_Text restText;

    private RunView runView;
    private int healAmount;

    public void Show(int healAmount, RunView runView)
    {
        this.healAmount = healAmount;
        this.runView = runView;

        restText.text = $"Rest site: heal {healAmount} HP (~30%) or upgrade a card.";
        gameObject.SetActive(true);
    }

    public void OnHealButton()
    {
        runView.OnRestChoice(new RestChoice(RestChoiceKind.Heal, -1));
    }

    public void OnUpgradeButton()
    {
        // For now, upgrade the first card in deck (index 0) for testing.
        runView.OnRestChoice(new RestChoice(RestChoiceKind.UpgradeCard, 0));
    }

    public void OnLeaveButton()
    {
        runView.OnRestChoice(new RestChoice(RestChoiceKind.Leave, -1));
    }
}
