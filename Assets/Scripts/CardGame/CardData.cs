using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    public enum CardType
    {
        Attack,
        Heal,
        Buff,
        Utility,
    }

    public string cardName;
    public string description;
    public Sprite artwork;
    public int manaCost;
    public int effectAmount;
    public CardType cardType;

    public Color GetCardColor()
    {
        switch(cardType)
        {
            case CardType.Attack:
                return new Color(0.9f, 0.3f, 0.3f);
            case CardType.Heal:
                return new Color(0.3f, 0.9f, 0.3f);
            case CardType.Buff:
                return new Color(0.3f, 0.3f, 0.9f);
            case CardType.Utility:
                return new Color(0.9f, 0.9f, 0.3f);
            default:
                return Color.white;
        }
    }
}
