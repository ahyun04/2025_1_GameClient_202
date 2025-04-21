using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public string characterName;
    public int maxHealth = 100;
    public int currenHealth;

    public Slider healthBar;
    public TMP_Text healthText;

    public int maxMana = 10;
    public int currenMana = 10;
    public Slider manaBar;
    public TMP_Text manaText;

    // Start is called before the first frame update
    void Start()
    {
        currenMana = maxMana;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currenHealth -= damage;
    }

    public void Heal(int amount)
    {
        currenHealth += amount;
    }

    public void UseMana(int amount)
    {
        currenMana -= amount;
        if(currenMana < 0)
        {
            currenMana = 0;
        }
        UpdateUI();
    }

    public void GainMana(int amount)
    {
        currenMana += amount;
        if(currenMana > maxMana)
        {
            currenMana = maxMana;
        }
        UpdateUI();
    }
    private void UpdateUI()
    {
        if(healthBar != null)
        {
            healthBar.value = (float)currenHealth / maxHealth;
        }
        if(healthText != null)
        {
            healthText.text = $"{currenHealth} / {maxHealth}";
        }
        if(manaBar != null)
        {
            manaBar.value = (float)currenMana / maxMana;
        }
        if(manaText != null)
        {
            manaText.text = $"{currenMana} / {maxMana}";
        }
    }
}
