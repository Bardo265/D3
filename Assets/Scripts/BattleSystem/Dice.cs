using UnityEngine;

public enum DiceColor
{
    Red,
    Green,
    Blue
}

public enum DiceSymbol
{
    A, // Sonne
    B, // Mond
    C  // Stern
}

[System.Serializable]
public class Dice
{
    public DiceColor color;
    public DiceSymbol currentSymbol;

    public void Roll()
    {
        int value = Random.Range(0, 3);
        currentSymbol = (DiceSymbol)value;
    }
}
