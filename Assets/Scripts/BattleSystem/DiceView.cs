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
    private Color originalBackgroundColor;

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
            case DiceColor.Red: originalBackgroundColor = Color.red; break;
            case DiceColor.Green: originalBackgroundColor = Color.green; break;
            case DiceColor.Blue: originalBackgroundColor = Color.blue; break;
        }

        background.color = originalBackgroundColor;
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

    public void Highlight(DiceSymbol chosenSymbol)
    {
        if (dice.currentSymbol == chosenSymbol)
        {
            // Hervorheben → leicht größer, volle Farbe
            background.color = originalBackgroundColor;
            icon.color = Color.white;
            transform.localScale = Vector3.one * 1.2f;
        }
        else
        {
            // Abdunkeln → aber Farbe bleibt sichtbar!
            background.color = originalBackgroundColor * 0.4f;
            icon.color = new Color(1f, 1f, 1f, 0.5f); // halbtransparent
            transform.localScale = Vector3.one;
        }
    }
}
