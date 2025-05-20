using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DamageTextEffect : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 100f; // UI용으로 더 큰 값
    [SerializeField] private float lifeTime = 1.5f;

    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Color originalColor;
    private Vector2 moveDirection;
    private float timer = 0f;

    private bool isCritical = false;
    private bool isStatusEffect = false;
    private bool useGravity = true;
    private float verticalVelocity = 100f;

    public void Initialize(bool critical, bool statusEffect)
    {
        isCritical = critical;
        isStatusEffect = statusEffect;

        // 상태 효과는 중력 사용 안 함
        if (isStatusEffect)
        {
            useGravity = false;
        }

        Start(); // 초기화 즉시 시작
    }

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (textMesh != null)
        {
            originalColor = textMesh.color;

            // 랜덤 방향 설정 (UI 스페이스용)
            float randomX = Random.Range(-0.5f, 0.5f);
            float randomY = useGravity ? Random.Range(0.5f, 1.0f) : Random.Range(0.8f, 1.5f);
            moveDirection = new Vector2(randomX, randomY).normalized;

            // 랜덤 회전 (UI용)
            if (rectTransform != null)
            {
                rectTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
            }

            // 초기 수직 속도 (중력 사용 시)
            if (useGravity)
            {
                verticalVelocity = Random.Range(100f, 200f); // UI용으로 더 큰 값
            }

            // 펀치 스케일 효과
            StartCoroutine(PunchScale(isCritical ? 1.5f : 1.2f));

            // 크리티컬일 경우 추가 효과
            if (isCritical)
            {
                StartCoroutine(FlashText());
                StartCoroutine(CreateFlashEffect());
            }
        }
    }

    // 수직 이동 설정 (중력 사용하지 않고 위로 올라가는 효과)
    public void SetVerticalMovement()
    {
        useGravity = false;
    }

    private void Update()
    {
        if (rectTransform == null) return;

        // 이동 처리
        if (useGravity)
        {
            // 중력 효과 (포물선 이동) - UI용
            verticalVelocity -= 300f * Time.deltaTime; // UI용 중력값
            rectTransform.anchoredPosition += new Vector2(0, verticalVelocity * Time.deltaTime);
            rectTransform.anchoredPosition += new Vector2(moveDirection.x * moveSpeed * Time.deltaTime, 0);
        }
        else
        {
            // 직선 이동 - UI용
            rectTransform.anchoredPosition += (Vector2)(moveDirection * moveSpeed * Time.deltaTime);
        }

        // 페이드 아웃
        timer += Time.deltaTime;
        if (timer >= lifeTime * 0.5f)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, (timer - lifeTime * 0.5f) / (lifeTime * 0.5f));
            }
            else if (textMesh != null)
            {
                float alpha = Mathf.Lerp(originalColor.a, 0f, (timer - lifeTime * 0.5f) / (lifeTime * 0.5f));
                textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }

            // 속도 감소
            moveSpeed = Mathf.Lerp(moveSpeed, 20f, Time.deltaTime * 2f);

            // 완전히 투명해지면 파괴
            if ((canvasGroup != null && canvasGroup.alpha <= 0.05f) ||
                (textMesh != null && textMesh.color.a <= 0.05f))
            {
                Destroy(gameObject);
            }
        }
    }

    // 스케일 펀치 효과
    private IEnumerator PunchScale(float intensity)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * intensity;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    // 번쩍임 효과
    private IEnumerator FlashText()
    {
        if (textMesh == null) yield break;

        Color flashColor = Color.white;
        float flashDuration = 0.2f;

        // 원래 색상 저장
        Color startColor = textMesh.color;

        // 번쩍임 색상으로 변경
        textMesh.color = flashColor;

        // 대기
        yield return new WaitForSeconds(flashDuration);

        // 원래 색상으로 복원
        textMesh.color = startColor;
    }

    // 잔상 효과를 UI용 깜빡임 효과로 대체
    private IEnumerator CreateFlashEffect()
    {
        if (textMesh == null) yield break;

        float interval = 0.05f;
        int flashCount = 3;

        for (int i = 0; i < flashCount; i++)
        {
            // 알파값 조정으로 깜빡임 구현
            textMesh.alpha = 0.5f;
            yield return new WaitForSeconds(interval);
            textMesh.alpha = 1.0f;
            yield return new WaitForSeconds(interval);
        }
    }
}