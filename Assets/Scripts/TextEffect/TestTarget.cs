using UnityEngine;

public class TestTarget : MonoBehaviour
{
    [SerializeField] private int minDamage = 5;
    [SerializeField] private int maxDamage = 50;
    [SerializeField] private int minHeal = 10;
    [SerializeField] private int maxHeal = 60;
    [SerializeField] private float criticalChance = 0.2f; // 20% 크리티컬 확률
    [SerializeField] private float missChance = 0.1f; // 10% 미스 확률
    [SerializeField] private float statusEffectChance = 0.15f; // 15% 상태이상 확률

    // 상태이상 종류
    private string[] statusEffects = { "Poison", "Burn", "Freeze", "Stun", "Blind", "Silence" };

    private void OnMouseDown()
    {
        // 랜덤 값으로 결정
        float randomValue = Random.value;

        if (randomValue < missChance)
        {
            // 미스
            ShowMiss();
        }
        else if (randomValue < 0.5f) // 50% 확률로 데미지
        {
            // 데미지
            bool isCritical = Random.value < criticalChance;
            int damage = Random.Range(minDamage, maxDamage + 1);

            // 크리티컬이면 데미지 2배
            if (isCritical) damage *= 2;

            ShowDamage(damage, isCritical);

            // 상태이상 추가 확률
            if (Random.value < statusEffectChance)
            {
                string statusEffect = statusEffects[Random.Range(0, statusEffects.Length)];
                ShowStatusEffect(statusEffect);
            }
        }
        else // 나머지 50%는 힐링
        {
            // 힐링
            bool isCritical = Random.value < criticalChance;
            int heal = Random.Range(minHeal, maxHeal + 1);

            // 크리티컬이면 힐링 1.5배
            if (isCritical) heal = Mathf.RoundToInt(heal * 1.5f);

            ShowHeal(heal, isCritical);
        }
    }

    private void ShowDamage(int amount, bool isCritical)
    {
        if (DamageEffectManager.Instance != null)
        {
            Vector3 position = transform.position;
            // 랜덤 위치 오프셋 추가
            position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 1.5f), 0);

            DamageEffectManager.Instance.ShowDamage(position, amount, isCritical);
        }
    }

    private void ShowHeal(int amount, bool isCritical)
    {
        if (DamageEffectManager.Instance != null)
        {
            Vector3 position = transform.position;
            // 랜덤 위치 오프셋 추가
            position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 1.5f), 0);

            DamageEffectManager.Instance.ShowHeal(position, amount, isCritical);
        }
    }

    private void ShowMiss()
    {
        if (DamageEffectManager.Instance != null)
        {
            Vector3 position = transform.position;
            // 랜덤 위치 오프셋 추가
            position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 1.5f), 0);

            DamageEffectManager.Instance.ShowMiss(position);
        }
    }

    private void ShowStatusEffect(string effectName)
    {
        if (DamageEffectManager.Instance != null)
        {
            Vector3 position = transform.position;
            // 약간 위쪽에 표시
            position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1.0f, 1.5f), 0);

            DamageEffectManager.Instance.ShowStatusEffect(position, effectName);
        }
    }
}