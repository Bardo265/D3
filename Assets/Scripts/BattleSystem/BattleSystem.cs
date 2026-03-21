using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{

    public List<Dice> diceList = new List<Dice>();

    public int redDice = 3;
    public int greenDice = 2;
    public int blueDice = 2;

    private int attack;
    private int defense;
    private int energy;

    public GameObject endTurnButton;

    public GameObject choiceButtons;

    public GameObject dicePrefab;
    public Transform diceParent;

    public GameObject dicePanel;
    public GameObject combatPanel;
    public GameObject enemyPanel;

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

    private int currentHeroIndex = 0;
    private int totalHeroes = 3;

    public List<Hero> heroes = new List<Hero>();
    public TMP_Text heroHPText;

    private bool isEnemyPhase = false;
    private int pendingDamage = 0;

    public TMP_Text enemyDamageText;
    public GameObject enemyPhasePanel;

    public GameObject heroPanel;

    public TMP_Text[] heroHPTexts;


    void Start()
    {
        // UI initial ausblenden
        choiceButtons.SetActive(false);
        endRoundButton.SetActive(false);

        // Gegner erstmal verstecken
        enemyPanel.SetActive(false);

        CreateHeroes();

        // Würfel erzeugen
        CreateDice();

        // Erste Runde starten
        StartHeroTurn();
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

        dicePanel.SetActive(false);
        combatPanel.SetActive(true);

        enemyPanel.SetActive(false); // 👈 WICHTIG
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

    public void UseRandomAbility()
    {
        if (energy < 2)
        {
            Debug.Log("Nicht genug Energie!");
            return;
        }

        energy -= 2;

        int roll = Random.Range(0, 2); // 0 oder 1

        if (roll == 0)
        {
            attack += 3;
            Debug.Log("Fähigkeit: +3 Angriff!");
        }
        else
        {
            defense += 3;
            Debug.Log("Fähigkeit: +3 Verteidigung!");
        }

        UpdatePlayerUI();
    }


    void StartHeroTurn()
    {
        endRoundButton.SetActive(false);
        heroPanel.SetActive(false);

        // 👇 Gegner ausblenden
        enemyPanel.SetActive(false);

        energy = 0;

        if (currentHeroIndex == 0)
        {
            // Angriff & Defense bleiben bestehen
        }

        Debug.Log("Held " + (currentHeroIndex + 1) + " ist am Zug");

        dicePanel.SetActive(true);
        combatPanel.SetActive(false);

        RollAllDice();
        UpdateHeroUI();
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

    void StartEnemyPhase()
    {
        isEnemyPhase = true;
        heroPanel.SetActive(true);
        endTurnButton.SetActive(false);
        Debug.Log("👉 Gegnerphase beginnt");

        enemyPanel.SetActive(true);

        if (currentEnemy == null)
        {
            SpawnEnemy();
        }

        pendingDamage = Random.Range(3, 7);

        UpdateEnemyDamageUI();

        enemyPhasePanel.SetActive(true);
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
        enemyPhasePanel.SetActive(false);
        dicePanel.SetActive(true);
        combatPanel.SetActive(false);

        StartHeroTurn();
    }

    void CreateHeroes()
    {
        heroes.Clear();

        for (int i = 0; i < 3; i++)
        {
            Hero hero = new Hero();
            hero.name = "Held " + (i + 1);
            hero.maxHP = 20;
            hero.currentHP = 20;

            heroes.Add(hero);
        }
        UpdateHeroUI();
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

            if (heroHPTexts[i] != null)
            {
                heroHPTexts[i].text = hero.name + "\nHP: " + hero.currentHP + " / " + hero.maxHP;
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
            endTurnButton.SetActive(true);
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
            endTurnButton.SetActive(true);
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



}
