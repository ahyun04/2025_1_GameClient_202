using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;                       //카드 데이터
    public int cardIndex;                           //손패에서의 인덱스 (나중에 사용)

    //3D 카드 요소
    public MeshRenderer cardRenderer;               //카드 렌더러 (icon or 일러스트)
    public TextMeshPro nameText;                    //이름 텍스트
    public TextMeshPro costText;                    //비용 텍스트
    public TextMeshPro attackText;                  //공격력/효과 텍스트
    public TextMeshPro descriptionText;             //설명 텍스트 

    //카드 상태 
    public bool isDragging = false;
    private Vector3 originalPosition;       //드래그 전 원래 위치 

    //레이어 마스크 
    public LayerMask enemyLayer;                //적 레이어
    public LayerMask playerLayer;               //플레이어 레이어 

    private CardManager cardManager;                    //카드 매니저 참조 추가 

    // Start is called before the first frame update
    void Start()
    {
        //레이어 마스크 설정
        playerLayer = LayerMask.GetMask("Player");
        enemyLayer = LayerMask.GetMask("Enemy");

        cardManager = FindObjectOfType<CardManager>();

        SetupCard(cardData);
    }

    //카드 데이터 설정 
    public void SetupCard(CardData data)
    {
        cardData = data;

        //3D 텍스트 업데이트 
        if (nameText != null) nameText.text = data.cardName;
        if (costText != null) costText.text = data.manaCost.ToString();
        if (attackText != null) attackText.text = data.effectAmount.ToString();
        if (descriptionText != null) descriptionText.text = data.description;

        //카드 텍스쳐 설정
        if (cardRenderer != null && data.artwork != null)
        {
            Material cardMaterial = cardRenderer.material;
            cardMaterial.mainTexture = data.artwork.texture;
        }

        if(descriptionText != null)
        {
            descriptionText.text = data.description + data.GetAdditionalEfectsDescription();
        }
    }

    private void OnMouseDown()
    {
        //드레그 시작 시 원래 위치 저장
        originalPosition = transform.position;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            //마우스 위치로 카드 이동
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        if(cardManager != null)
        {
            float disToDiscard = Vector3.Distance(transform.position, cardManager.discardPosition.position);
            if(disToDiscard < 2.0f)
            {
                cardManager.DiscardCard(cardIndex);
                return;
            }
        }

        CharacterStats playerStats = null;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null )
        {
            playerStats = playerObj.GetComponent<CharacterStats>();
        }
        if(playerStats == null || playerStats.currentMana < cardData.manaCost)
        {
            Debug.Log($"마나가 부족합니다! (필요 : {cardData.manaCost}, 현재 : {playerStats?.currentMana ?? 0}");
            transform.position = originalPosition;
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool cardUsed = false;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
            CharacterStats enemyStats = hit.collider.GetComponent<CharacterStats>();
            if(enemyStats != null)
            {
                if(cardData.cardType == CardData.CardType.Attack)
                {
                    enemyStats.TakeDamage(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} 카드로 적에게 {cardData.effectAmount} 데미지를 입혔습니다");
                    cardUsed = true;
                }
            }
            else
            {
                Debug.Log("이 카드는 저게게 사용할 수 없습니다");
            }
            
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, playerLayer))
        {
            if (playerStats != null)
            {
                if(cardData.cardType == CardData.CardType.Heal)
                {
                    playerStats.Heal(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} 카드로 플레이어의 체력을 {cardData.effectAmount} 회복 했습니다");
                    cardUsed = true;
                }
            }
            else
            {
                Debug.Log("이 카드는 플레이어에게 사용 할 수 없습니다");
            }
        }
        if(!cardUsed)
        {
            transform.position = originalPosition;
            if (cardManager != null)
                cardManager.ArrangeHand();
            return;
        }
        playerStats.UseMana(cardData.manaCost);
        Debug.Log($"마나를 {cardData.manaCost} 사용 했습니다 (남은 마나 : {playerStats.currentMana})");

        if(cardData.additionalEffects != null && cardData.additionalEffects.Count > 0)
        {
            ProcessAdditionalEffectsAndDiscard();
        }
        else
        {
            if( cardManager != null )
                cardManager.DiscardCard(cardIndex);
        }
    }

    private void ProcessAdditionalEffectsAndDiscard()
    {
        CardData cardDataCopy = cardData;
        int cardIndexCopy = cardIndex;

        foreach(var effect in cardDataCopy.additionalEffects)
            {
            switch(effect.effectType)
            {
                case CardData.AdditionalEffectType.DrawCard:
                    for (int i = 0; i< effect.effectAmount; i++)
                    {
                        if(cardManager != null)
                        {
                            cardManager.DrawCard();
                        }
                    }
                    Debug.Log($"{effect.effectAmount} 장의 카드를 드로우 했습니다. ");
                    break;
                case CardData.AdditionalEffectType.DiscardCard:
                    for(int i = 0; i<effect.effectAmount; i++)
                    {
                        if(cardManager != null && cardManager.handCards.Count > 0)
                        {
                            int randomIndex = Random.Range(0, cardManager.handCards.Count);
                            Debug.Log($"랜덤 카드 버리기 : 선택된 인덱스 {randomIndex}, 현재 손패 크기 : {cardManager.handCards.Count}");

                            if(cardIndexCopy < cardManager.handCards.Count)
                            {
                                if(randomIndex != cardIndexCopy)
                                {
                                    cardManager.DiscardCard(randomIndex);

                                    if(randomIndex < cardIndexCopy)
                                    {
                                        cardIndexCopy--;
                                    }
                                }
                                else if(cardManager.handCards.Count > 1)
                                {
                                    int newIndex = (randomIndex + 1)% cardManager.handCards.Count;
                                    cardManager.DiscardCard(newIndex);

                                    if(randomIndex < cardIndexCopy)
                                    {
                                        cardIndexCopy--;
                                    }
                                }
                            }
                            else
                            {
                                cardManager.DiscardCard(randomIndex);
                            }
                        }
                    }
                    Debug.Log($"랜덤으로 {effect.effectAmount}장의 카드를 버렸습니다");
                    break;

                case CardData.AdditionalEffectType.GainMana:
                    GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                    if (playerObj != null)
                    {
                        CharacterStats playerStats = playerObj.GetComponent<CharacterStats>();
                        if (playerStats != null)
                        {
                            playerStats.GainMana(effect.effectAmount);
                            Debug.Log($"마나를 {effect.effectAmount} 획득했습니다! (현재 마나 : {playerStats.currentMana}");
                        }
                    }
                    break;
                case CardData.AdditionalEffectType.ReduceEnemyMana:
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ebentg");
                    foreach(var enemy in enemies)
                    {
                        CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
                        if(enemyStats != null)
                        {
                            enemyStats.UseMana(effect.effectAmount);
                            Debug.Log($"마나를 {enemyStats.characterName}의 마나를 {effect.effectAmount}감소 시켰습니다");
                        }
                    }
                    break;
                case CardData.AdditionalEffectType.ReduceCardCost:
                    for(int  i = 0; i < cardManager.handCards.Count; i++)
                    {
                        CardDisplay display = cardManager.cardObjects[i].GetComponent<CardDisplay>();
                        if(display != null && display != this)
                        {
                            TextMeshPro costText = display.costText;
                            if(costText != null)
                            {
                                int originalCost = display.cardData.manaCost;
                                int newCost = Mathf.Max(0, originalCost - effect.effectAmount);
                                costText.text = newCost.ToString();
                                costText.color = Color.green;
                            }
                        }
                    }
                    break;
            }
        }
        if(cardManager != null)
        {
            cardManager.DiscardCard(cardIndexCopy);
        }
    }
}
