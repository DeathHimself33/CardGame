using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CardGame;

public class ShopView : MonoBehaviour
{
    public TMP_Text shopText;
    public TMP_Text[] cardButtonsText;   // size 3 in Inspector
    public TMP_Text[] relicButtonsText;  // size 3

    private RunView runView;
    private List<Card> cardOffers = new();
    private List<Relic> relicOffers = new();
    private Player player;

    public void Show(List<Card> cards, List<Relic> relics, RunView runView, Player player)
    {
        this.cardOffers = cards;
        this.relicOffers = relics;
        this.runView = runView;
        this.player = player;

        shopText.text = $"Gold: {player.Gold}";

        for (int i = 0; i < cardButtonsText.Length; i++)
        {
            if (i < cardOffers.Count)
                cardButtonsText[i].text = $"{cardOffers[i].Name} (30g)";
            else
                cardButtonsText[i].text = "-";
        }

        for (int i = 0; i < relicButtonsText.Length; i++)
        {
            if (i < relicOffers.Count)
                relicButtonsText[i].text = $"{relicOffers[i].Name} (120g)";
            else
                relicButtonsText[i].text = "-";
        }

        gameObject.SetActive(true);
    }

    public void OnBuyCard(int index)
    {
        runView.OnShopChoice(new ShopChoice(ShopChoiceKind.BuyCard, index));
    }

    public void OnBuyRelic(int index)
    {
        runView.OnShopChoice(new ShopChoice(ShopChoiceKind.BuyRelic, index));
    }

    public void OnLeave()
    {
        runView.OnShopChoice(new ShopChoice(ShopChoiceKind.Leave, -1));
    }
}
