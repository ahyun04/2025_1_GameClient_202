using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    public int cardIndex;

    //3D 가트 요소
    public MeshRenderer cardRenderer;
    public TextMeshPro nameText;
    public TextMeshPro costText;
    public TextMeshPro attackText;
    public TextMeshPro descriptionText;

    public bool isDragging = false;
    private Vector3 originalPosition;
    private CardManager cardManager;
    public LayerMask enermLayer;
    public LayerMask playerLayer;

    void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        enermLayer = LayerMask.GetMask("Enemy");

        cardManager = FindAnyObjectByType<CardManager>();

        SetupCard(cardData);
    }

    public void SetupCard(CardData data)
    {
        cardData = data;

        if(nameText != null )
        {
            nameText.text = data.cardName;
        }
        if(costText != null )
        {
            costText.text = data.manaCost.ToString();
        }
        if( attackText != null )
        {
            attackText.text = data.effectAmount.ToString();
        }
        if(descriptionText != null)
        {
            descriptionText.text = data.description;
        }

        if(cardRenderer != null && data.artwork != null)
        {
            Material cardMaterial = cardRenderer.material;
            cardMaterial.mainTexture = data.artwork.texture;
        }
    }

    private void OnMouseDown()
    {
        originalPosition = transform.position;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if(isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        }
    }

    private void OnMouseUp()
    {
        CharacterStats playerStats = FindAnyObjectByType<CharacterStats>();
        if(playerStats != null || playerStats.currenMana < cardData.manaCost)
        {
            Debug.Log($"마나가 부족합니다 !(필요 : {cardData.manaCost}, 현재 : {playerStats?.currenMana ?? 0})");
            transform.position = originalPosition;
            return;
        }


        isDragging = false;
        //레이캐스트
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   

        bool cardUsed = false;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enermLayer))
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
                else
                {
                    Debug.Log("이 카드는 적에게 사용할 수 없습니다");
                }
            }
        }
        else if(Physics.Raycast(ray,out hit, Mathf.Infinity,playerLayer))
        {
            //CharacterStats playerStats = hit.collider.GetComponent<CharacterStats>();

            if(playerStats != null)
            {
                if(cardData.cardType == CardData.CardType.Heal)
                {
                    playerStats.Heal(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} 카드로 플레이어의 체력을 {cardData.effectAmount} 회복했습니다");
                    cardUsed = true;
                }
                else
                {
                    Debug.Log("이 카드는 플레이어에게 사용할 수 없습니다");
                }
            }
        }
        else if( cardManager != null )
        {
            float disToDiscard = Vector3.Distance(transform.position, cardManager.discardPosition.position);
            if( disToDiscard < 2.0f )
            {
                cardManager.DiscardCard(cardIndex);
                return;
            }
        }

        //버린 카드 더미 근처에 드롭했는지 검사

        if(!cardUsed)
        {
            transform.position = originalPosition;
            //손패 재정렬
            cardManager.ArrangeHand();
        }
        else
        {
            //카드를 사용했다면 버린 카드 더미로 이동
            if(cardManager != null)
                cardManager.DiscardCard(cardIndex);

            //카드 사용시 마나 소모
            playerStats.UseMana(cardData.manaCost);
            Debug.Log($"마나를 {cardData.manaCost} 사용 했습니다 (남은 마나 : {playerStats.currenMana})");
        }
    }
}
