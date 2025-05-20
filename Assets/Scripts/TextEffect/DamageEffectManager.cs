using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DamageEffectManager : MonoBehaviour
{
    public static DamageEffectManager Instance { get; private set; }

    [SerializeField] private GameObject textPrefab; // UI 데미지 텍스트 프리팹
    [SerializeField] private Canvas uiCanvas; // UI 캔버스 참조

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 캔버스가 없으면 자동으로 찾기
        if (uiCanvas == null)
        {
            uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas == null)
            {
                Debug.LogError("UI 캔버스를 찾을 수 없습니다. 씬에 캔버스가 있는지 확인하세요.");
            }
        }
    }

    public void ShowDamageText(Vector3 position, string text, Color color, bool isCritical = false, bool isStatusEffect = false)
    {
        if (textPrefab == null || uiCanvas == null) return;

        // 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);

        // UI가 카메라 뒤에 있는 경우 표시하지 않음
        if (screenPos.z < 0) return;

        // 데미지 텍스트 UI 생성
        GameObject damageText = Instantiate(textPrefab, uiCanvas.transform);

        // 스크린 위치 설정
        RectTransform rectTransform = damageText.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.position = screenPos;
        }

        // 텍스트 컴포넌트 설정
        TextMeshProUGUI tmp = damageText.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            // 텍스트 설정
            tmp.text = text;

            // 색상 설정
            tmp.color = color;

            // 아웃라인 색상 설정
            tmp.outlineColor = new Color(
                Mathf.Clamp01(color.r - 0.3f),
                Mathf.Clamp01(color.g - 0.3f),
                Mathf.Clamp01(color.b - 0.3f),
                color.a
            );

            // 크기 설정
            float scale = 1.0f;

            // 텍스트가 숫자인 경우 값에 따라 크기 조정
            int numericValue;
            if (int.TryParse(text.Replace("+", "").Replace("CRIT!", "").Replace("HEAL CRIT!", ""), out numericValue))
            {
                scale = Mathf.Clamp(numericValue / 15f, 0.8f, 2.5f);
            }

            // 크리티컬이면 크기 증가
            if (isCritical) scale *= 1.4f;

            // 상태 효과는 약간 작게
            if (isStatusEffect) scale *= 0.8f;

            damageText.transform.localScale = new Vector3(scale, scale, scale);
        }

        // 효과 설정
        DamageTextEffect effect = damageText.AddComponent<DamageTextEffect>();
        if (effect != null)
        {
            effect.Initialize(isCritical, isStatusEffect);

            // 상태 효과는 떨어지지 않고 위로 올라감
            if (isStatusEffect)
            {
                effect.SetVerticalMovement();
            }
        }
    }

    // 데미지 함수
    public void ShowDamage(Vector3 position, int amount, bool isCritical = false)
    {
        string text = amount.ToString();
        Color color = isCritical ? new Color(1.0f, 0.8f, 0.0f) : new Color(1.0f, 0.3f, 0.3f);

        if (isCritical)
        {
            text = "CRIT!\n" + text;
        }

        ShowDamageText(position, text, color, isCritical);
    }

    // 힐링 함수
    public void ShowHeal(Vector3 position, int amount, bool isCritical = false)
    {
        string text = "+" + amount.ToString();
        Color color = isCritical ? new Color(0.4f, 1.0f, 0.4f) : new Color(0.3f, 0.9f, 0.3f);

        if (isCritical)
        {
            text = "HEAL CRIT!\n" + text;
        }

        ShowDamageText(position, text, color, isCritical);
    }

    // 미스 함수
    public void ShowMiss(Vector3 position)
    {
        ShowDamageText(position, "MISS", Color.gray, false);
    }

    // 상태 효과 함수
    public void ShowStatusEffect(Vector3 position, string effectName)
    {
        Color color;

        // 상태 효과에 따른 색상 설정
        switch (effectName.ToLower())
        {
            case "poison":
                color = new Color(0.5f, 0.1f, 0.5f); // 보라색
                break;
            case "burn":
                color = new Color(1f, 0.4f, 0f); // 주황색
                break;
            case "freeze":
                color = new Color(0.5f, 0.8f, 1f); // 하늘색
                break;
            case "stun":
                color = new Color(1f, 1f, 0f); // 노란색
                break;
            default:
                color = new Color(1f, 1f, 1f); // 기본 흰색
                break;
        }

        ShowDamageText(position, effectName.ToUpper(), color, false, true);
    }
}