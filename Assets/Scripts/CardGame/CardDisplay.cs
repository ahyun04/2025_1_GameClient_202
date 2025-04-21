using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    public int cardIndex;

    //3D ��Ʈ ���
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
            Debug.Log($"������ �����մϴ� !(�ʿ� : {cardData.manaCost}, ���� : {playerStats?.currenMana ?? 0})");
            transform.position = originalPosition;
            return;
        }


        isDragging = false;
        //����ĳ��Ʈ
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
                    Debug.Log($"{cardData.cardName} ī��� ������ {cardData.effectAmount} �������� �������ϴ�");
                    cardUsed = true;
                }
                else
                {
                    Debug.Log("�� ī��� ������ ����� �� �����ϴ�");
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
                    Debug.Log($"{cardData.cardName} ī��� �÷��̾��� ü���� {cardData.effectAmount} ȸ���߽��ϴ�");
                    cardUsed = true;
                }
                else
                {
                    Debug.Log("�� ī��� �÷��̾�� ����� �� �����ϴ�");
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

        //���� ī�� ���� ��ó�� ����ߴ��� �˻�

        if(!cardUsed)
        {
            transform.position = originalPosition;
            //���� ������
            cardManager.ArrangeHand();
        }
        else
        {
            //ī�带 ����ߴٸ� ���� ī�� ���̷� �̵�
            if(cardManager != null)
                cardManager.DiscardCard(cardIndex);

            //ī�� ���� ���� �Ҹ�
            playerStats.UseMana(cardData.manaCost);
            Debug.Log($"������ {cardData.manaCost} ��� �߽��ϴ� (���� ���� : {playerStats.currenMana})");
        }
    }
}
