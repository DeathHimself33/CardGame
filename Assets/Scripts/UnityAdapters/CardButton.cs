using UnityEngine;
using TMPro;
public class CardButton : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text costText;

    private int handIndex;
    private CombatView combatView;

    public void Setup(int index, string name, int cost, CombatView view)
    {
        handIndex = index;
        combatView = view;
        nameText.text = name;
        costText.text = cost.ToString();
    }

    public void OnClick()
    {
        Debug.Log($"CardButton.OnClick handIndex={handIndex} name={nameText.text}");
        combatView.OnCardClicked(handIndex);
    }
}
