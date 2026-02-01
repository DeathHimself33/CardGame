using UnityEngine;
using TMPro;
using CardGame;

public class RunView : MonoBehaviour
{
    public TMP_Text floorText;
    public TMP_Text roomText;

    public GameObject combatPanel;
    public CombatView combatView;

    public GameObject shopPanel;
    public ShopView shopView;

    public GameObject restPanel;
    public RestView restView;

    private RunController run;
    private GameBootstrap bootstrap;
    private RunController.RoomType? currentCombatRoomType;
    private CombatController lastCombat;

    void Start()
    {
        bootstrap = FindFirstObjectByType<GameBootstrap>();
        Debug.Log($"Found bootstrap {bootstrap}");
        run = bootstrap.RunController;
        AdvanceRun();
    }
    public void AdvanceRun()
    {
        var result = run.AdvanceRun();

        switch (result.Kind)
        {
            case RunStepKind.EnterRoom:
                var enter = (EnterRoomResult)result;
                floorText.text = $"Floor {enter.Floor}";
                roomText.text = enter.RoomType.ToString();
                StartCoroutine(AdvanceRunNextFrame());
                break;

            case RunStepKind.NeedCombat:
                var combatResult = (CombatNeededResult)result;
                currentCombatRoomType = combatResult.RoomType;
                combatPanel.SetActive(true);
                combatView.runView = this;
                combatView.BeginCombat(bootstrap.Player, combatResult.Enemies, bootstrap.CardLibrary);
                break;

            case RunStepKind.NeedShopChoice:
                var shopNeeded = (ShopNeededResult)result;
                shopPanel.SetActive(true);
                shopView.Show(shopNeeded.CardOffers, shopNeeded.RelicOffers, this, run.Player);
                break;

            case RunStepKind.NeedRestChoice:
                var restNeeded = (RestNeededResult)result;
                restPanel.SetActive(true);
                restView.Show(restNeeded.HealAmount, this);
                break;

            case RunStepKind.NeedRewardsChoice:
                var rewards = (RewardsNeededResult)result;

                //Later replace with a way to pick a card to take
                int cardIndex = rewards.CardOffers.Count > 0 ? 0 : -1;
                int relicIndex = rewards.RelicOffers.Count > 0 ? 0 : -1;

                run.ApplyRewardsChoice(cardIndex, relicIndex);

                StartCoroutine(AdvanceRunNextFrame());
                break;

            case RunStepKind.RunEnded:
                var end = (RunEndedResult)result;
                roomText.text = end.PlayerWon ? "Run Complete!" : "You Died!";
                break;

        }
    }

    public void OnCombatEnded(bool PlayerWon, CombatController combat)
    {
        if (currentCombatRoomType.HasValue)
        {
            run.OnCombatFinished(PlayerWon, currentCombatRoomType.Value, combat);
        }

        combatPanel.SetActive(false);
        currentCombatRoomType = null;
        StartCoroutine(AdvanceRunNextFrame());
    }

    public void OnShopChoice(ShopChoice choice)
    {
        run.ApplyShopChoice(choice);
        if (choice.Kind == ShopChoiceKind.Leave)
        {
            shopPanel.SetActive(false);
            StartCoroutine(AdvanceRunNextFrame());
        }
        else
        {
            shopView.Show(run.CurrentShopCardOffers, run.CurrentShopRelicOffers, this, run.Player);
        }
    }
    public void OnRestChoice(RestChoice choice)
    {
        run.ApplyRestChoice(choice);
        restPanel.SetActive(false);
        StartCoroutine(AdvanceRunNextFrame());
    }
    private System.Collections.IEnumerator AdvanceRunNextFrame()
    {
        yield return null;
        AdvanceRun();
    }

}