using UnityEngine;

[System.Serializable]
public class Ability
{
    public string name;

    // Kosten
    public int energyCost;
    public int resourceCost;
    public int hpCost;

    // Passive Werte
    public int bonusRedDice;
    public int bonusGreenDice;
    public int bonusBlueDice;

    public int rerolls;

    // Aktiv-Effekt (einfach erstmal)
    public System.Action<BattleSystem, Hero> effect;
}
