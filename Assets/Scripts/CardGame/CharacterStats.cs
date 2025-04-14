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
    public TextMeshPro healthText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currenHealth -= damage;
    }

    public void Heal(int amount)
    {
        currenHealth += amount;
    }
}
