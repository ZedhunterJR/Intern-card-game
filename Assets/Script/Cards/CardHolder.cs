using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class CardHolder : MonoBehaviour
{
    #region Register variable
    [Header("Card Dragging")]
    private Vector3 offset;
    [SerializeField] List<ButtonUI> selectedCards = new();
    [SerializeField] Vector2 scaleWhenDragging = new Vector2(1.25f, 1.25f);
    private float rotationAmount = 20f;
    private float rotationSpeed = 20f;
    private Vector2 rotationDelta;
    [SerializeField] ButtonUI draggingCard;

    [Header("Hover Parameters")]
    [SerializeField] Vector2 scaleWhenHover = new Vector2(1.15f, 1.15f);
    [SerializeField] float hoverPunchAngle = 5f;
    [SerializeField] float hoverTransition = 0.15f;
    [SerializeField] ButtonUI hoverCard;

    [SerializeField] private GameObject cardSlot;
    [SerializeField] private Transform cardHolder;

    [Header("Spawn Setting")]
    [SerializeField] private int cardsToSpawn = 7;
    public List<ButtonUI> cards;
    #endregion

    #region Unity Methods

    private void Start()
    {
        for (int i = 0; i <= cardsToSpawn; i++)
        {
            var card = Instantiate(cardSlot, cardHolder);
            cards.Add(card.GetComponentInChildren<ButtonUI>());
        }

        int cardCount = 0;

        foreach (var card in cards)
        {
            AddCardEvent(card.gameObject);

            card.name = cardCount.ToString();
            cardCount++;
        }
    }

    private void Update()
    {
        if (draggingCard != null)
        {
            Vector2 targetPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            Vector2 direction = (targetPostion - (Vector2)draggingCard.transform.position).normalized;
            draggingCard.transform.position = Vector3.Lerp(draggingCard.transform.position, targetPostion, 0.1f);
            draggingCard.transform.position = ClampScreen(draggingCard.transform.position);

            float targetZRotation = -direction.x * rotationAmount;

            targetZRotation = Mathf.Clamp(targetZRotation, -60f, 60f);
            float smoothedZ = Mathf.LerpAngle(draggingCard.transform.eulerAngles.z, targetZRotation, rotationSpeed * Time.deltaTime);
            draggingCard.transform.rotation = Quaternion.Euler(0f, 0f, smoothedZ);

            SwapCheck();
        }

        ApplyCardIdleTilt();
    }
    #endregion

    #region Methods
    void AddCardEvent(GameObject gameObject)
    {
        var buttonUI = gameObject.GetComponent<ButtonUI>();
        var cardRect = gameObject.GetComponent<RectTransform>();
        var cardCanvas = gameObject.GetComponent<Canvas>();
        buttonUI.MouseHoverEnter = () =>
        {
            hoverCard = buttonUI;

            if (draggingCard == null)
            {
                cardRect.DOScale(scaleWhenHover, 0.1f).SetEase(Ease.OutBack);
                DOTween.Kill(2, true);
                cardRect.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
            }
        };
        buttonUI.MouseHoverExit = () =>
        {
            hoverCard = null;
            if (draggingCard != buttonUI)
                cardRect.DOScale(Vector2.one, 0.1f).SetEase(Ease.OutBack);
        };
        buttonUI.MouseDragBegin = () =>
        {
            draggingCard = buttonUI;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = mousePosition - (Vector2)gameObject.transform.position;
            cardRect.DOScale(scaleWhenDragging * 1.1f, 0.1f).SetEase(Ease.OutBack); 
            // Không hiểu sáo đoạn này scaleWhenDragging = Vector2(1.25f, 1.25f) nhưng khi game chạy lại là Vector2(1.05f, 1.05f) nên phải * 1.2f

            // Change Card To First Layer
            cardCanvas.overrideSorting = true;
            cardCanvas.sortingLayerName = "Interact";
            cardCanvas.sortingOrder = 1;

        };
        buttonUI.MouseDrag = () =>
        {
        };
        buttonUI.MouseDragEnd = () =>
        {
            draggingCard = null;
            cardRect.DOScale(Vector2.one, 0.1f);
            cardRect.DOAnchorPos(Vector2.zero, 0.3f).SetUpdate(false);
            cardRect.DORotate(Vector2.zero, 0.3f).SetUpdate(false);

            selectedCards.Remove(buttonUI);
            cardCanvas.overrideSorting = false;
        };
        buttonUI.ClickFunc = () =>
        {
            if (selectedCards.Contains(buttonUI))
            {
                selectedCards.Remove(buttonUI);
                cardRect.DOAnchorPos(Vector2.zero, 0.3f / 2).SetUpdate(false);
            }
            else if (selectedCards.Count < 5)
            {
                selectedCards.Add(buttonUI);
                cardRect.DOAnchorPosY((transform.position.y + 50f), 0.3f).SetUpdate(false);
            }
        };
    }

    Vector3 ClampScreen(Vector3 position)
    {
        Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
        Vector2 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Vector2 clampPosition = position;
        if (position.x > topRight.x) clampPosition.x = topRight.x;
        if (position.x < bottomLeft.x) clampPosition.x = bottomLeft.x;
        if (position.y > topRight.y) clampPosition.y = topRight.y;
        if (position.y < bottomLeft.y) clampPosition.y = bottomLeft.y;

        return clampPosition;
    }

    void SwapCheck()
    {
        for (int i = 0; i < cards.Count; i++)
        {

            if (draggingCard.transform.position.x > cards[i].transform.position.x)
            {
                if (ParentIndex(draggingCard) < ParentIndex(cards[i]))
                {
                    Swap(i);
                    break;
                }
            }

            if (draggingCard.transform.position.x < cards[i].transform.position.x)
            {
                if (ParentIndex(draggingCard) > ParentIndex(cards[i]))
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    static int ParentIndex(ButtonUI card)
    {
        return card.transform.parent.GetSiblingIndex();
    }

    void Swap(int index)
    {
        Transform focusedParent = draggingCard.transform.parent;
        Transform crossedParent = cards[index].transform.parent;

        cards[index].transform.SetParent(focusedParent);
        DOTween.Kill(2, true);
        if (selectedCards.Contains(cards[index]))
        {
            cards[index].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 50f), 0.1f).SetUpdate(false);
            cards[index].GetComponent<RectTransform>().DOPunchRotation(Vector3.forward * 15, hoverTransition, 5, 1).SetId(3);
        }
        else
        {
            cards[index].GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.1f).SetId(3);
            cards[index].GetComponent<RectTransform>().DOPunchRotation(Vector3.forward * 15, hoverTransition, 5, 1).SetId(3);
        }

        //cards[index].transform.localPosition = cards[index].selected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;
        draggingCard.transform.SetParent(crossedParent);

        bool swapIsRight = ParentIndex(cards[index]) > ParentIndex(draggingCard);
        //cards[index].cardVisual.Swap(swapIsRight ? -1 : 1);

        //Updated Visual Indexes
        foreach (ButtonUI card in cards)
        {
            //card.cardVisual.UpdateIndex(transform.childCount);
        }
    }

    void ApplyCardIdleTilt()
    {
        float frequency = 1.5f;
        float amplitude = 2.5f;

        for (int i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            if (card == draggingCard) continue;

            float timeOffset = Time.time * frequency + i * 0.5f;

            float tiltX = Mathf.Sin(timeOffset) * amplitude * 1.5f;
            float tiltY = Mathf.Cos(timeOffset) * amplitude * 1.5f;
            float tiltZ = Mathf.Sin(timeOffset + 1f) * amplitude * 1.5f;

            Quaternion targetRotation = Quaternion.Euler(tiltX, tiltY, tiltZ);
            card.transform.rotation = Quaternion.Lerp(card.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    #endregion
}
