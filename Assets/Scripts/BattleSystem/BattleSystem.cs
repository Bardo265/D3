using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{

    public List<Dice> diceList = new List<Dice>();

    public int redDice = 3;
    public int greenDice = 2;
    public int blueDice = 2;

    private HeroClass currentSelectedClass;

    private int attack;
    private int defense;
    private int energy;

    public GameObject endTurnButton;
    public GameObject abilityButton;
    public GameObject choiceButtons;

    public GameObject classSelectionPanel;
    public GameObject abilitySelectionPanel;
    public GameObject enemyPanel;
    public GameObject startFightButton;
    public GameObject statsPanel;

    public GameObject dicePrefab;
    public Transform diceParent;

    public GameObject dicePanel;
    public GameObject combatPanel;

    public Transform redParent;
    public Transform greenParent;
    public Transform blueParent;

    private List<DiceView> diceViews = new List<DiceView>();


    public TMP_Text attackText;
    public TMP_Text defenseText;
    public TMP_Text energyText;

    public GameObject enemyPrefab;
    public Transform enemyParent;

    public GameObject endRoundButton;

    private Enemy currentEnemy;
    public TMP_Dropdown classDropdown;

    private int currentHeroIndex = 0;
    private int totalHeroes = 3;

    public List<Hero> heroes = new List<Hero>();


    // 👉 NEU:
    public int currentSelectionHeroIndex = 0;
    public TMP_Text heroHPText;

    private bool isEnemyPhase = false;
    private int pendingDamage = 0;

    public TMP_Text enemyDamageText;
    public GameObject enemyPhasePanel;

    public GameObject heroPanel;

    public TMP_Text[] heroHPTexts;
    public TMP_Text[] heroResourceTexts;
    public Button[] abilityButtons;


    public TMP_Text[] abilityTexts; // 3 Texte
    private List<Ability> selectedAbilities = new List<Ability>();

    public UnityEngine.UI.Image[] abilityButtonImages;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;

    private int selectingHeroIndex = 0;



    void Start()
    {
        SetState("ClassSelection");
        currentSelectionHeroIndex = 0;
        heroes.Clear();
    }

    public void StartGameAfterSelection()
    {
        Debug.Log("Spiel startet nach Klassenauswahl!");

        CreateDice();
        StartHeroTurn();
    }


    public void OnClassDropdownChanged(int index)
    {
        currentSelectedClass = (HeroClass)index;
    }


    public Enemy GetCurrentEnemy()
    {
        return currentEnemy;
    }

    public void OnClassSelected(int index)
    {
        Hero hero = new Hero();

        hero.heroClass = (HeroClass)index;

        SetupHeroByClass(hero);
        SetupAbilities(hero);

        heroes.Add(hero);

        currentSelectionHeroIndex++;

        Debug.Log("Held erstellt: " + hero.heroClass);

        // 👉 Nächster Schritt
        SetState("AbilitySelection");
    }

    void CreateDice()
    {
        diceList.Clear();

        for (int i = 0; i < redDice; i++)
            diceList.Add(new Dice { color = DiceColor.Red });

        for (int i = 0; i < greenDice; i++)
            diceList.Add(new Dice { color = DiceColor.Green });

        for (int i = 0; i < blueDice; i++)
            diceList.Add(new Dice { color = DiceColor.Blue });
    }

    public void RollAllDice()
    {
        Debug.Log("---- Würfelwurf ----");

        foreach (var dice in diceList)
        {
            dice.Roll();
            Debug.Log(dice.color + " → " + dice.currentSymbol);
        }

        DisplayDice();

        choiceButtons.SetActive(true); // 👈 hier
    }

    public void ChooseA()
    {
        EvaluateRoll(DiceSymbol.A);
    }

    public void ChooseB()
    {
        EvaluateRoll(DiceSymbol.B);
    }

    public void ChooseC()
    {
        EvaluateRoll(DiceSymbol.C);
    }

    public void SetupHeroByClass(Hero hero)
    {
        switch (hero.heroClass)
        {
            case HeroClass.Krieger:
                hero.redDice = 4;
                hero.greenDice = 2;
                hero.blueDice = 1;

                // keine Resource
                hero.maxResource = 0;
                hero.resource = 0;
                break;

            case HeroClass.Magier:
                hero.redDice = 1;
                hero.greenDice = 2;
                hero.blueDice = 4;

                // Mana (einmalig, keine Regeneration)
                hero.maxResource = 10;
                hero.resource = 10;
                break;

            case HeroClass.Moench:
                hero.redDice = 2;
                hero.greenDice = 3;
                hero.blueDice = 2;

                // Heiligkeit
                hero.maxResource = 10;
                hero.resource = 5; // Startwert

                break;

            case HeroClass.Assassine:
                hero.redDice = 3;
                hero.greenDice = 1;
                hero.blueDice = 3;

                // Machtpunkte (reset jede Runde)
                hero.maxResource = 5;
                hero.resource = 5;
                break;

            case HeroClass.Jaeger:
                hero.redDice = 3;
                hero.greenDice = 2;
                hero.blueDice = 2;

                // Munition (bleibt erhalten)
                hero.maxResource = 6;
                hero.resource = 6;
                break;

            case HeroClass.Lichtbringer:
                hero.redDice = 1;
                hero.greenDice = 4;
                hero.blueDice = 2;

                // Lichtpunkte (werden erzeugt/verbraucht)
                hero.maxResource = 5;
                hero.resource = 0;
                break;
        }
    }

    public void SetupAbilities(Hero hero)
    {
        hero.allAbilities.Clear();

        switch (hero.heroClass)
        {
            case HeroClass.Krieger:

                hero.allAbilities.Clear();

                // 🛡️ Schild
                hero.allAbilities.Add(new Ability
                {
                    name = "Schild",

                    energyCost = 1,

                    bonusGreenDice = 3, // passiv

                    effect = (bs, h) =>
                    {
                        bs.AddDefense(2);
                        Debug.Log("Schild: +2 Verteidigung");
                    }
                });

                // 🎓 Lehrstunde
                hero.allAbilities.Add(new Ability
                {
                    name = "Lehrstunde",

                    energyCost = 1,

                    bonusBlueDice = 3, // passiv

                    effect = (bs, h) =>
                    {
                        bs.AddAttack(3);
                        Debug.Log("Lehrstunde: +3 Angriff");
                    }
                });

                // ⚔️ Gute Technik
                hero.allAbilities.Add(new Ability
                {
                    name = "Gute Technik",

                    energyCost = 1,

                    bonusRedDice = 3, // passiv

                    effect = (bs, h) =>
                    {
                        int removable = Mathf.Min(5, bs.GetAttack());

                        if (removable <= 0)
                        {
                            Debug.Log("Keine Angriffspunkte zum Umwandeln!");
                            return;
                        }

                        // 💀 1 Leben als Kosten
                        h.currentHP -= 1;

                        if (h.currentHP < 0)
                            h.currentHP = 0;

                        bs.AddAttack(-removable);
                        bs.AddDefense(removable * 2);

                        Debug.Log("Gute Technik: -" + removable + " Angriff, +" + (removable * 2) + " Verteidigung, -1 HP");

                        bs.UpdateHeroUI(); // 👉 wichtig für Anzeige
                    }
                });

                break;

            case HeroClass.Magier:

                hero.allAbilities.Clear();

                // 🔵 Energiefluss
                hero.allAbilities.Add(new Ability
                {
                    name = "Energiefluss",

                    energyCost = 2,

                    effect = (bs, h) =>
                    {
                        h.resource += 1;
                        h.resource = Mathf.Clamp(h.resource, 0, h.maxResource);

                        Debug.Log("Energiefluss: +1 Mana");

                        bs.UpdateHeroUI();
                    }
                });

                // 🔥 Feuerstrahl
                hero.allAbilities.Add(new Ability
                {
                    name = "Feuerstrahl",

                    energyCost = 1,
                    resourceCost = 1,

                    bonusRedDice = 3,

                    effect = (bs, h) =>
                    {
                        bs.AddAttack(4);
                        Debug.Log("Feuerstrahl: +4 Angriff");
                    }
                });

                // ❄️ Eisrüstung
                hero.allAbilities.Add(new Ability
                {
                    name = "Eisrüstung",

                    energyCost = 0,
                    resourceCost = 1,

                    bonusGreenDice = 2,
                    bonusBlueDice = 1,

                    effect = (bs, h) =>
                    {
                        bs.AddDefense(3);
                        Debug.Log("Eisrüstung: +3 Verteidigung");
                    }
                });

                break;

            case HeroClass.Moench:

                hero.allAbilities.Clear();

                // ☀️ Sonnenschlag
                hero.allAbilities.Add(new Ability
                {
                    name = "Sonnenschlag",

                    energyCost = 1,
                    resourceCost = 1,

                    bonusBlueDice = 2,

                    effect = (bs, h) =>
                    {
                        bs.AddAttack(3);
                        Debug.Log("Sonnenschlag: +3 Angriff");
                    }
                });

                // ✨ Gott sei mit dir
                hero.allAbilities.Add(new Ability
                {
                    name = "Gott sei mit dir",

                    energyCost = 0,
                    resourceCost = 1,

                    bonusGreenDice = 1,
                    bonusBlueDice = 1,

                    effect = (bs, h) =>
                    {
                        // 👉 aktuellen Helden heilen (einfachste Version)
                        h.currentHP += 3;
                        h.currentHP = Mathf.Clamp(h.currentHP, 0, h.maxHP);

                        Debug.Log("Heilung: +3 HP");

                        bs.UpdateHeroUI();
                    }
                });

                // 👊 Faustkampf
                hero.allAbilities.Add(new Ability
                {
                    name = "Faustkampf",

                    energyCost = 2,
                    resourceCost = 3,

                    // ⚠️ dynamisch → kein fixer Dice Bonus

                    effect = (bs, h) =>
                    {
                        int enemyCount = bs.GetEnemyCount(); // 👉 brauchen wir gleich

                        int attackGain = enemyCount * 2;
                        int defenseGain = enemyCount * 1;

                        bs.AddAttack(attackGain);
                        bs.AddDefense(defenseGain);

                        Debug.Log("Faustkampf: +" + attackGain + " Angriff, +" + defenseGain + " Schild");
                    }
                });

                break;
            case HeroClass.Assassine:

                hero.allAbilities.Clear();

                // 🗡️ Messerwurf
                hero.allAbilities.Add(new Ability
                {
                    name = "Messerwurf",

                    energyCost = 1,

                    bonusRedDice = 2,
                    bonusBlueDice = 1,

                    effect = (bs, h) =>
                    {
                        bs.AddAttack(2);
                        Debug.Log("Messerwurf: +2 Angriff");
                    }
                });

                // 💀 Todesstoß
                hero.allAbilities.Add(new Ability
                {
                    name = "Todesstoß",

                    energyCost = 2,
                    resourceCost = 3,

                    bonusRedDice = 2,

                    effect = (bs, h) =>
                    {
                        Enemy enemy = bs.GetCurrentEnemy();

                        if (enemy == null)
                        {
                            Debug.Log("Kein Gegner vorhanden!");
                            return;
                        }

                        if (enemy.GetCurrentHP() <= 7)
                        {
                            enemy.Die(); // 👉 sofort töten
                            Debug.Log("Todesstoß: Gegner eliminiert!");
                        }
                        else
                        {
                            Debug.Log("Gegner hat zu viel Leben für Todesstoß!");
                        }
                    }
                });

                // 🚪 Erstes Tor
                hero.allAbilities.Add(new Ability
                {
                    name = "Erstes Tor",

                    energyCost = 1,
                    resourceCost = 2,

                    bonusRedDice = 1,
                    bonusGreenDice = 1,
                    bonusBlueDice = 1,

                    effect = (bs, h) =>
                    {
                        bs.AddAttack(3);
                        bs.AddDefense(1);

                        // 👇 wichtig für spätere Combo!
                        h.usedFirstGateThisTurn = true;

                        Debug.Log("Erstes Tor aktiviert!");
                    }
                });

                break;
            case HeroClass.Jaeger:

                hero.allAbilities.Clear();

                // 🎯 Schnellfeuer
                hero.allAbilities.Add(new Ability
                {
                    name = "Schnellfeuer",

                    energyCost = 1,
                    resourceCost = 1, // Munition

                    bonusRedDice = 2,

                    effect = (bs, h) =>
                    {
                        bs.AddAttack(3);
                        Debug.Log("Schnellfeuer: +3 Angriff");
                    }
                });

                // 🧘 Konzentration
                hero.allAbilities.Add(new Ability
                {
                    name = "Konzentration",

                    energyCost = 2,

                    bonusBlueDice = 3,

                    effect = (bs, h) =>
                    {
                        bs.ReducePendingDamage(2);

                        Debug.Log("Konzentration: Gegner-Angriff -2");
                    }
                });

                // 🏹 Drei Pfeile
                hero.allAbilities.Add(new Ability
                {
                    name = "Drei Pfeile",

                    energyCost = 1,
                    resourceCost = 2,

                    bonusGreenDice = 1,
                    bonusRedDice = 1, // pro Gegner wird später erweitert

                    effect = (bs, h) =>
                    {
                        int enemyCount = bs.GetEnemyCount();

                        int totalDamage = enemyCount * 2;

                        bs.AddAttack(totalDamage);

                        Debug.Log("Drei Pfeile: +" + totalDamage + " Angriff (aufgeteilt auf Gegner)");
                    }
                });

                break;
            case HeroClass.Lichtbringer:

                hero.allAbilities.Clear();

                // 💡 Lichtstrahl
                hero.allAbilities.Add(new Ability
                {
                    name = "Lichtstrahl",

                    energyCost = 1,

                    bonusBlueDice = 3,

                    effect = (bs, h) =>
                    {
                        h.resource += 1; // Lichtpunkt erhalten

                        int enemies = bs.GetEnemyCount();

                        int shieldGain = 4 - enemies;
                        if (shieldGain < 0) shieldGain = 0;

                        bs.AddDefense(shieldGain);

                        Debug.Log("Lichtstrahl: +" + shieldGain + " Schild, +1 Lichtpunkt");

                        bs.UpdateHeroUI();
                    }
                });

                // 🌟 Leuchten
                hero.allAbilities.Add(new Ability
                {
                    name = "Leuchten",

                    energyCost = 2,

                    bonusRedDice = 3,

                    effect = (bs, h) =>
                    {
                        if (h.resource <= 0)
                        {
                            Debug.Log("Nicht genug Lichtpunkte!");
                            return;
                        }

                        h.resource -= 1;

                        h.currentHP = h.maxHP;

                        Debug.Log("Leuchten: Held vollständig geheilt!");

                        bs.UpdateHeroUI();
                    }
                });

                // ⚡ Blendendes Licht
                hero.allAbilities.Add(new Ability
                {
                    name = "Blendendes Licht",

                    energyCost = 1,

                    bonusBlueDice = 3,

                    effect = (bs, h) =>
                    {
                        if (h.resource < 2)
                        {
                            Debug.Log("Nicht genug Lichtpunkte!");
                            return;
                        }

                        h.resource -= 2;

                        bs.AddAttack(2);
                        bs.AddDefense(2);

                        Debug.Log("Blendendes Licht: +2 Angriff, +2 Schild");

                        bs.UpdateHeroUI();
                    }
                });

                break;
        }

        // 👉 Standard: erste 2 ausgerüstet
        hero.equippedAbilities = hero.allAbilities.Take(2).ToList();
    }

    void ShowAbilitySelection()
    {

        classSelectionPanel.SetActive(false);
        abilitySelectionPanel.SetActive(true);

        UpdateAbilityButtons();
        Hero hero = heroes[currentSelectionHeroIndex];

        Debug.Log("Wähle Fähigkeiten für: " + hero.name);

        for (int i = 0; i < hero.allAbilities.Count && i < abilityTexts.Length; i++)
        {
            abilityTexts[i].text = hero.allAbilities[i].name;
        }

        selectedAbilities.Clear();

        // Farben resetten
        for (int i = 0; i < abilityButtonImages.Length; i++)
        {
            abilityButtonImages[i].color = normalColor;
        }
    }

    public void SelectAbility(int index)
    {
        Hero hero = heroes[currentSelectionHeroIndex];

        if (index < 0 || index >= hero.allAbilities.Count)
            return;

        Ability ability = hero.allAbilities[index];

        if (!selectedAbilities.Contains(ability))
        {
            if (selectedAbilities.Count >= 2)
                return;

            selectedAbilities.Add(ability);
        }
        else
        {
            selectedAbilities.Remove(ability);
        }

        // 👉 WICHTIG: UI sofort aktualisieren
        UpdateAbilityButtons();
    }

    public void ConfirmAbilities()
    {
        Hero hero = heroes[currentSelectionHeroIndex];

        hero.equippedAbilities = new List<Ability>(selectedAbilities);

        selectedAbilities.Clear();

        currentSelectionHeroIndex++;

        if (currentSelectionHeroIndex < 3)
        {
            SetState("ClassSelection"); // nächster Held
        }
        else
        {
            currentHeroIndex = 0; // 🔥 GANZ WICHTIG
            StartHeroTurn();
        }
    }



    public void EvaluateRoll(DiceSymbol chosenSymbol)
    {
        

        foreach (var dice in diceList)
        {
            if (dice.currentSymbol == chosenSymbol)
            {
                switch (dice.color)
                {
                    case DiceColor.Red: attack++; break;
                    case DiceColor.Green: defense++; break;
                    case DiceColor.Blue: energy++; break;
                }
            }
        }

        // Highlight
        foreach (var view in diceViews)
        {
            view.Highlight(chosenSymbol);
        }

        choiceButtons.SetActive(false);

        StartCoroutine(SwitchToCombatAfterDelay(1f));
    }

    void DisplayDice()
    {
        // Alte löschen
        ClearContainer(redParent);
        ClearContainer(greenParent);
        ClearContainer(blueParent);

        diceViews.Clear();

        foreach (var dice in diceList)
        {
            Transform parent = GetParentForColor(dice.color);

            GameObject obj = Instantiate(dicePrefab, parent);
            DiceView view = obj.GetComponent<DiceView>();

            view.SetDice(dice, this);
            diceViews.Add(view);
        }
    }

    void ClearContainer(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    Transform GetParentForColor(DiceColor color)
    {
        switch (color)
        {
            case DiceColor.Red:
                return redParent;

            case DiceColor.Green:
                return greenParent;

            case DiceColor.Blue:
                return blueParent;
        }

        return null;
    }


    public void RerollDice(Dice dice)
    {
        dice.Roll();
        Debug.Log("Neu gewürfelt: " + dice.color + " → " + dice.currentSymbol);
    }

    IEnumerator SwitchToCombatAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        attackText.text = "Angriff: " + attack;
        defenseText.text = "Verteidigung: " + defense;
        energyText.text = "Energie: " + energy;
        SetState("Combat");
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyPrefab fehlt im Inspector!");
            return;
        }

        if (enemyParent == null)
        {
            Debug.LogError("EnemyParent fehlt im Inspector!");
            return;
        }

        GameObject obj = Instantiate(enemyPrefab, enemyParent);
        currentEnemy = obj.GetComponent<Enemy>();

        currentEnemy.Setup("Goblin", 20, this);
    }

    public void OnEnemyClicked(Enemy enemy)
    {
        if (attack <= 0)
        {
            Debug.Log("Keine Angriffspunkte mehr!");
            return;
        }

        enemy.TakeDamage(1);
        attack--;

        UpdatePlayerUI();

        if (currentEnemy == null)
        {
            Debug.Log("Kampf gewonnen!");
        }
    }

    void UpdatePlayerUI()
    {
        attackText.text = "Angriff: " + attack;
        defenseText.text = "Verteidigung: " + defense;
        energyText.text = "Energie: " + energy;
    }


    public void UseAbility(int index)
    {
        Hero hero = GetCurrentHero();

        if (index >= hero.equippedAbilities.Count)
            return;

        Ability ability = hero.equippedAbilities[index];

        // Kosten prüfen
        if (energy < ability.energyCost)
        {
            Debug.Log("Nicht genug Energie!");
            return;
        }

        if (hero.resource < ability.resourceCost)
        {
            Debug.Log("Nicht genug Resource!");
            return;
        }

        // Kosten abziehen
        energy -= ability.energyCost;
        hero.resource -= ability.resourceCost;

        // Effekt ausführen
        ability.effect?.Invoke(this, hero);

        UpdatePlayerUI();
        UpdateHeroUI();
    }


    void StartHeroTurn()
    {
        Hero hero = GetCurrentHero();

        // ✅ 1. RESET (GANZ WICHTIG!)
        redDice = 0;
        greenDice = 0;
        blueDice = 0;

        // ✅ 2. BASIS vom Helden
        redDice = hero.redDice;
        greenDice = hero.greenDice;
        blueDice = hero.blueDice;

        // ✅ 3. Klassenboni
        if (hero.heroClass == HeroClass.Magier)
        {
            if (hero.resource == 0)
            {
                blueDice += 1;
                Debug.Log("Magier Bonuswürfel!");
            }
        }

        if (hero.heroClass == HeroClass.Moench)
        {
            if (hero.resource >= 4 && hero.resource <= 6)
            {
                greenDice += 1;
            }
        }

        if (hero.heroClass == HeroClass.Assassine)
        {
            hero.resource = hero.maxResource;
        }

        // ✅ 4. Fähigkeiten
        foreach (var ability in hero.equippedAbilities)
        {
            redDice += ability.bonusRedDice;
            greenDice += ability.bonusGreenDice;
            blueDice += ability.bonusBlueDice;
        }

        // ✅ 5. Ressourcen-Logik (unabhängig)
        if (hero.heroClass == HeroClass.Moench)
        {
            int gain = Random.Range(1, 4);
            hero.resource += gain;
            hero.resource = Mathf.Clamp(hero.resource, 0, hero.maxResource);
        }

        hero.usedFirstGateThisTurn = false;
        energy = 0;

        Debug.Log($"FINAL DICE: R={redDice}, G={greenDice}, B={blueDice}");

        // 👉 STATE STARTET DICE
        SetState("Dice");
    }
    public int GetEnemyCount()
    {
        // aktuell nur 1 Gegner
        return currentEnemy != null ? 1 : 0;
    }

    public int GetAttack()
    {
        return attack;
    }

    public void AddAttack(int amount)
    {
        attack += amount;
        UpdatePlayerUI();
    }

    public void AddDefense(int amount)
    {
        defense += amount;
        UpdatePlayerUI();
    }


    public void EndTurn()
    {
        currentHeroIndex++;

        if (currentHeroIndex >= totalHeroes)
        {
            StartEnemyPhase();
        }
        else
        {
            StartHeroTurn();
        }
    }

    public int GetPendingDamage()
    {
        return pendingDamage;
    }

    public void ReducePendingDamage(int amount)
    {
        pendingDamage -= amount;

        if (pendingDamage < 0)
            pendingDamage = 0;

        UpdateEnemyDamageUI();

        Debug.Log("Schaden reduziert auf: " + pendingDamage);
    }


    void StartEnemyPhase()
    {
        isEnemyPhase = true;
        SetState("Enemy");

        if (currentEnemy == null)
        {
            SpawnEnemy();
        }

        pendingDamage = Random.Range(7, 14);

        UpdateEnemyDamageUI();

        enemyPhasePanel.SetActive(true);
        endTurnButton.SetActive(false);
    }

    public void EndRound()
    {
        Debug.Log("Runde beendet");

        StartNewRound();
    }

    void StartNewRound()
    {
        Debug.Log("Neue Runde beginnt!");

        currentHeroIndex = 0;
        SetState("Dice");

        StartHeroTurn();
    }

    

    Hero GetCurrentHero()
    {
        return heroes[currentHeroIndex];
    }

    void UpdateHeroUI()
    {
        for (int i = 0; i < heroes.Count; i++)
        {
            Hero hero = heroes[i];

            // HP anzeigen
            if (heroHPTexts[i] != null)
            {
                heroHPTexts[i].text =
                    hero.name + "\nHP: " + hero.currentHP + " / " + hero.maxHP;
            }

            // Resource anzeigen
            if (heroResourceTexts[i] != null)
            {
                string resName = GetResourceName(hero);

                if (resName == "")
                {
                    heroResourceTexts[i].text = ""; // Krieger
                }
                else
                {
                    heroResourceTexts[i].text =
                        resName + ": " + hero.resource + " / " + hero.maxResource;
                }
            }
        }
    }

    void DamageHero(int damage)
    {
        Hero hero = GetCurrentHero();

        hero.currentHP -= damage;

        if (hero.currentHP <= 0)
        {
            Debug.Log(hero.name + " ist besiegt!");
            hero.currentHP = 0;
        }

        UpdateHeroUI();
    }

    public void UseShield()
    {
        if (!isEnemyPhase) return;

        if (pendingDamage <= 0)
        {
            Debug.Log("Kein Schaden mehr übrig – Schild nicht nötig.");
            return;
        }

        if (defense <= 0)
        {
            Debug.Log("Kein Schild mehr übrig!");
            return;
        }

        defense--;          // 👉 1 Schild verbrauchen
        pendingDamage--;    // 👉 1 Schaden blocken

        Debug.Log("1 Schaden mit Schild geblockt!");
        Debug.Log("Restschaden: " + pendingDamage);

        UpdatePlayerUI();
        UpdateEnemyDamageUI();

        // Wenn kein Schaden mehr übrig → Phase beenden
        if (pendingDamage <= 0)
        {
            endRoundButton.SetActive(true);
        }
    }

    public void HitHero(int heroIndex)
    {
        if (!isEnemyPhase) return;

        if (pendingDamage <= 0)
        {
            Debug.Log("Kein Schaden mehr übrig – Schild nicht nötig.");
            return;
        }

        Hero hero = heroes[heroIndex];

        hero.currentHP -= 1;
        pendingDamage -= 1;

        Debug.Log(hero.name + " nimmt 1 Schaden");

        UpdateHeroUI();

        UpdateEnemyDamageUI();

        if (pendingDamage <= 0)
        {
            endRoundButton.SetActive(true);
        }

    }

    void EndEnemyPhase()
    {
        isEnemyPhase = false;

        Debug.Log("👉 Gegnerphase beendet");

        enemyPhasePanel.SetActive(false);

        if (currentEnemy != null)
        {
            Destroy(currentEnemy.gameObject);
            currentEnemy = null;
        }

        currentHeroIndex = 0;

        StartHeroTurn();
    }


    void UpdateEnemyDamageUI()
    {
        if (enemyDamageText != null)
        {
            enemyDamageText.text = "Schaden: " + pendingDamage;
        }
    }

    string GetResourceName(Hero hero)
    {
        switch (hero.heroClass)
        {
            case HeroClass.Magier: return "Mana";
            case HeroClass.Moench: return "Heiligkeit";
            case HeroClass.Assassine: return "Macht";
            case HeroClass.Jaeger: return "Munition";
            case HeroClass.Lichtbringer: return "Licht";
            default: return ""; // Krieger hat keine
        }
    }


    public void SetState(string state)
    {
        // ALLES aus
        abilitySelectionPanel.SetActive(false);
        dicePanel.SetActive(false);
        combatPanel.SetActive(false);
        enemyPanel.SetActive(false);
        enemyPhasePanel.SetActive(false);
        heroPanel.SetActive(false);
        classSelectionPanel.SetActive(false);
        statsPanel.SetActive(false);

        abilityButton.SetActive(false);
        endTurnButton.SetActive(false);
        endRoundButton.SetActive(false);
        choiceButtons.SetActive(false);

        switch (state)
        {
            case "ClassSelection":
                classSelectionPanel.SetActive(true);
                break;

            case "AbilitySelection":
                abilitySelectionPanel.SetActive(true);
                break;

            case "Dice":
                dicePanel.SetActive(true);

                CreateDice();   // 👉 Würfel erzeugen
                RollAllDice();  // 👉 Würfel würfeln

                break;

            case "Combat":
                combatPanel.SetActive(true);
                abilityButton.SetActive(true);
                endTurnButton.SetActive(true);
                heroPanel.SetActive(true);
                statsPanel.SetActive(true);

                UpdateHeroUI(); // 👈 DAS FEHLT
                break;

            case "Enemy":
                enemyPanel.SetActive(true);
                enemyPhasePanel.SetActive(true);
                heroPanel.SetActive(true);
                statsPanel.SetActive(true);

                endRoundButton.SetActive(false); // 👈 erstmal AUS (korrekt)
                break;
        }
    }

    void ShowClassSelection()
    {
        classSelectionPanel.SetActive(true);
        abilitySelectionPanel.SetActive(false);
    }

    void ShowStartFightButton()
    {
        startFightButton.SetActive(true);
    }


    void HighlightButton(int index, bool active)
    {
        if (index < 0 || index >= abilityButtons.Length) return;

        var colors = abilityButtons[index].colors;

        if (active)
        {
            colors.normalColor = Color.green;
        }
        else
        {
            colors.normalColor = Color.white;
        }

        abilityButtons[index].colors = colors;
    }

    void UpdateAbilityButtons()
    {
        Hero hero = heroes[currentSelectionHeroIndex];

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            if (i < hero.allAbilities.Count)
            {
                bool selected = selectedAbilities.Contains(hero.allAbilities[i]);

                var colors = abilityButtons[i].colors;
                colors.normalColor = selected ? Color.green : Color.white;
                abilityButtons[i].colors = colors;

                abilityButtons[i].gameObject.SetActive(true);
            }
            else
            {
                abilityButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ConfirmClassSelection()
    {
        // 👉 Falls Held noch nicht existiert → erstellen
        if (heroes.Count <= currentSelectionHeroIndex)
        {
            heroes.Add(new Hero());
        }

        Hero hero = heroes[currentSelectionHeroIndex];

        // 👉 Klasse setzen (direkt vom Dropdown!)
        hero.heroClass = (HeroClass)classDropdown.value;

        // 👉 Name setzen
        hero.name = hero.heroClass.ToString();

        // 👉 HP je nach Klasse
        switch (hero.heroClass)
        {
            case HeroClass.Krieger: hero.maxHP = 25; break;
            case HeroClass.Magier: hero.maxHP = 18; break;
            case HeroClass.Moench: hero.maxHP = 22; break;
            case HeroClass.Assassine: hero.maxHP = 20; break;
            case HeroClass.Jaeger: hero.maxHP = 21; break;
            case HeroClass.Lichtbringer: hero.maxHP = 24; break;
        }

        hero.currentHP = hero.maxHP;

        // 👉 Setup
        SetupHeroByClass(hero);
        SetupAbilities(hero);

        Debug.Log("Held " + currentSelectionHeroIndex + " = " + hero.heroClass);

        SetState("AbilitySelection");
        ShowAbilitySelection(); // 👈 DAS FEHLT
    }


}
