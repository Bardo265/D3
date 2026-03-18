using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DiceView : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Referenzen")]
    public Image background;
    public Image icon;

    [Header("Symbol Sprites")]
    public Sprite sunSprite;
    public Sprite moonSprite;
    public Sprite starSprite;

    private Dice dice;
    private BattleSystem battleSystem;

    public void SetDice(Dice dice, BattleSystem system)
    {
        this.dice = dice;
        this.battleSystem = system;

        UpdateView();
    }

    public void UpdateView()
    {
        SetColor(dice.color);
        SetSymbol(dice.currentSymbol);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Würfel geklickt: " + dice.color);

        battleSystem.RerollDice(dice);
        UpdateView();
    }

    void SetColor(DiceColor color)
    {
        switch (color)
        {
            case DiceColor.Red: background.color = Color.red; break;
            case DiceColor.Green: background.color = Color.green; break;
            case DiceColor.Blue: background.color = Color.blue; break;
        }
    }

    void SetSymbol(DiceSymbol symbol)
    {
        switch (symbol)
        {
            case DiceSymbol.A: icon.sprite = sunSprite; break;
            case DiceSymbol.B: icon.sprite = moonSprite; break;
            case DiceSymbol.C: icon.sprite = starSprite; break;
        }
    }
}
