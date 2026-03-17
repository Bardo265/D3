using UnityEngine;
using UnityEngine.UI;
public class DiceView : MonoBehaviour
{
    [Header("UI Referenzen")]
    public Image background; // Hintergrund (Farbe)
    public Image icon;       // Symbol (Sonne/Mond/Stern)

    [Header("Symbol Sprites")]
    public Sprite sunSprite;
    public Sprite moonSprite;
    public Sprite starSprite;

    // Wird vom BattleSystem aufgerufen
    public void SetDice(Dice dice)
    {
        SetColor(dice.color);
        SetSymbol(dice.currentSymbol);

        Debug.Log("SetDice aufgerufen: " + dice.color + " " + dice.currentSymbol +
                  " → Background = " + (background.sprite != null) +
                  " Icon = " + (icon.sprite != null));
    }

    void SetColor(DiceColor color)
    {
        switch (color)
        {
            case DiceColor.Red:
                background.color = Color.red;
                break;

            case DiceColor.Green:
                background.color = Color.green;
                break;

            case DiceColor.Blue:
                background.color = Color.blue;
                break;
        }
    }

    void SetSymbol(DiceSymbol symbol)
    {
        switch (symbol)
        {
            case DiceSymbol.A:
                icon.sprite = sunSprite;
                break;

            case DiceSymbol.B:
                icon.sprite = moonSprite;
                break;

            case DiceSymbol.C:
                icon.sprite = starSprite;
                break;
        }
    }
}
