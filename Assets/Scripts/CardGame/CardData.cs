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

    public enum AdditionalEffectType
    {
        None,
        DrawCard,
        DiscardCard,
        GainMana,
        ReduceEnemyMana,
        ReduceCardCost,
    }

    //추가 효과 리스트
    public List<AdditionalEffect> additionalEffects = new List< AdditionalEffect>();
    
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
    public string GetAdditionalEfectsDescription()
    {
        if (additionalEffects.Count == 0)
            return "";

        string result = "\n";

        foreach(var effect in additionalEffects )
        {
            result += effect.GetDescription() + "\n";
        }
        return result;
    }
}
