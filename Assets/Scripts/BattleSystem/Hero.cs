using System.Collections.Generic;

[System.Serializable]
public class Hero
{
    public string name;

    public int maxHP;
    public int currentHP;

    public HeroClass heroClass;

    // 🎲 Würfel
    public int redDice;
    public int greenDice;
    public int blueDice;

    public int resource;
    public int maxResource;

    public bool usedFirstGateThisTurn = false;

    public List<Ability> allAbilities = new List<Ability>();   // 3 Stück
    public List<Ability> equippedAbilities = new List<Ability>(); // 2 aktiv
}