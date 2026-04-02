using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text nameText;
    public TMP_Text hpText;

    public int maxHP = 10;
    private int currentHP;

    public int attack;

    private BattleSystem battleSystem;

    public void Setup(string enemyName, int hp, BattleSystem system)
    {
        nameText.text = enemyName;

        maxHP = hp;
        currentHP = hp;

        battleSystem = system;

        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP < 0)
            currentHP = 0;

        UpdateUI();

        if (currentHP <= 0)
        {
            Debug.Log("Gegner besiegt!");
            Destroy(gameObject);
        }
    }

    public void ReduceAttack(int amount)
    {
        attack -= amount;

        if (attack < 0)
            attack = 0;

        Debug.Log("Gegner Angriff reduziert: " + attack);
    }
    public int GetCurrentHP()
    {
        return currentHP;
    }

    public void Die()
    {
        Destroy(gameObject);
    }


    void UpdateUI()
    {
        hpText.text = "HP: " + currentHP + " / " + maxHP;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        battleSystem.OnEnemyClicked(this);
    }
}
