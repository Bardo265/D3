using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{

    public List<Dice> diceList = new List<Dice>();

    public int redDice = 3;
    public int greenDice = 2;
    public int blueDice = 2;

    public GameObject dicePrefab;
    public Transform diceParent;

    private List<DiceView> diceViews = new List<DiceView>();


    void Start()
    {
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
    }




    public void EvaluateRoll(DiceSymbol chosenSymbol)
    {
        int attack = 0;
        int defense = 0;
        int energy = 0;

        Debug.Log("Gewählt: " + chosenSymbol);
        Debug.Log("---- Auswertung ----");

        foreach (var dice in diceList)
        {
            if (dice.currentSymbol == chosenSymbol)
            {
                switch (dice.color)
                {
                    case DiceColor.Red:
                        attack++;
                        break;

                    case DiceColor.Green:
                        defense++;
                        break;

                    case DiceColor.Blue:
                        energy++;
                        break;
                }
            }
        }

        Debug.Log("Angriff: " + attack);
        Debug.Log("Verteidigung: " + defense);
        Debug.Log("Energie: " + energy);
    }

    void DisplayDice()
    {
        foreach (Transform child in diceParent)
            Destroy(child.gameObject);

        diceViews.Clear();

        foreach (var dice in diceList)
        {
            GameObject obj = Instantiate(dicePrefab, diceParent);
            DiceView view = obj.GetComponent<DiceView>();
            view.SetDice(dice);
            diceViews.Add(view);

            Debug.Log("Würfel erzeugt: " + dice.color + " " + dice.currentSymbol);
        }
    }

}
