using UnityEngine;
using TMPro;
using CardGame;

public class EnemyPanel : MonoBehaviour
{
    public TMP_Text hpText;
    public TMP_Text intentText;

    private int enemyIndex;
    private CombatView combatView;
    public void Setup(int index, Enemy enemy, CombatView view)
    {
        enemyIndex = index;
        combatView = view;

        hpText.text = $"{enemy.HP}/{enemy.MaxHP}";

        if(enemy.plannedAction != null)
        {
            intentText.text = $"{enemy.plannedAction.Intent} {enemy.plannedAction.Amount}";
        }
        else
        {
            intentText.text = "";
        }
    }
    public void OnClick()
    {
        combatView.OnEnemyClicked(enemyIndex);
    }
}
