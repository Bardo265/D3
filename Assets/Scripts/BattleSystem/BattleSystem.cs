using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{

    public List<Dice> diceList = new List<Dice>();

    public int redDice = 3;
    public int greenDice = 2;
    public int blueDice = 2;

    public GameObject choiceButtons;

    public GameObject dicePrefab;
    public Transform diceParent;

    public Transform redParent;
    public Transform greenParent;
    public Transform blueParent;

    private List<DiceView> diceViews = new List<DiceView>();


    void Start()
    {

        choiceButtons.SetActive(false);

        CreateDice();
        RollAllDice();
        DisplayDice();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EvaluateRoll(DiceSymbol.A);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EvaluateRoll(DiceSymbol.B);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EvaluateRoll(DiceSymbol.C);
        }
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

        choiceButtons.SetActive(true);
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
        int attack = 0;
        int defense = 0;
        int energy = 0;

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

        Debug.Log("Gewählt: " + chosenSymbol);
        Debug.Log("Angriff: " + attack);
        Debug.Log("Verteidigung: " + defense);
        Debug.Log("Energie: " + energy);

        choiceButtons.SetActive(false);
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

}
