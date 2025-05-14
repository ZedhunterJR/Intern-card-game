using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class CardHolder : Singleton<CardHolder>
{
    #region Register variable
    [Header("Card Dragging")]
    private Vector3 offset;
    //[SerializeField] List<ButtonUI> selectedCards = new();
    [SerializeField] Vector2 scaleWhenDragging = new Vector2(1.25f, 1.25f);
    private float rotationAmount = 20f;
    private float rotationSpeed = 20f;
    private Vector2 rotationDelta;
    [SerializeField] GameObject draggingCard;

    [Header("Hover Parameters")]
    [SerializeField] Vector2 scaleWhenHover = new Vector2(1.15f, 1.15f);
    [SerializeField] float hoverPunchAngle = 5f;
    [SerializeField] float hoverTransition = 0.15f;
    [SerializeField] GameObject hoverCard;

    [SerializeField] private GameObject cardSlot;
    [SerializeField] private Transform[] cardHolders;

    [Header("Spawn Setting")]
    //[SerializeField] private int cardsToSpawn = 7;
    public List<GameObject> cards;

    #endregion

    #region Unity Methods

    private void Start()
    {
        for (int i = 0; i < cardHolders.Length; i++)
        {
            var card = Instantiate(cardSlot, cardHolders[i]);
            cards.Add(card);
        }

        int cardCount = 0;

        foreach (var card in cards)
        {
            AddCardEvent(card);

            card.name = cardCount.ToString();
            cardCount++;
        }

        GameManager.Instance.TestBasicDeck(cards);
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

        //ApplyCardIdleTilt();
    }
    #endregion

    #region Methods
    void AddCardEvent(GameObject obj)
    {
        var buttonUI = obj.GetComponent<ButtonUI>();
        var cardRect = obj.GetComponent<RectTransform>();
        var cardCanvas = obj.GetComponent<Canvas>();
        var cardSkill = obj.GetComponent<Skill>();
        cardCanvas.overrideSorting = true;
        cardCanvas.sortingLayerName = "Interact";
        cardCanvas.sortingOrder = 0;
        buttonUI.MouseHoverEnter = () =>
        {
            if (draggingCard != null)
                return;

            hoverCard = obj;
            cardRect.DOScale(scaleWhenHover, 0.1f).SetEase(Ease.OutBack);
            DOTween.Kill(2, true);
            cardRect.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
            cardRect.DOAnchorPosY((transform.position.y + 10f), 0.3f).SetUpdate(false).SetId(2);

            cardCanvas.overrideSorting = true;
            cardCanvas.sortingLayerName = "Interact";
            cardCanvas.sortingOrder = 1;
            cardSkill.UpdatePanelInfor();
            cardSkill.informationPanel.SetActive(true);
        };
        buttonUI.MouseHoverExit = () =>
        {
            if (draggingCard != null)
                return;

            hoverCard = null;
            cardRect.DOScale(Vector2.one, 0.1f).SetEase(Ease.OutBack);
            cardRect.DOAnchorPos(Vector2.zero, 0.3f).SetUpdate(false);
            cardCanvas.overrideSorting = false;
            cardSkill.informationPanel.SetActive(false);
        };
        buttonUI.MouseDragBegin = () =>
        {
            draggingCard = obj;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = mousePosition - (Vector2)obj.transform.position;
            cardRect.DOScale(scaleWhenDragging * 1.1f, 0.1f).SetEase(Ease.OutBack);
            // Không hiểu sáo đoạn này scaleWhenDragging = Vector2(1.25f, 1.25f) nhưng khi game chạy lại là Vector2(1.05f, 1.05f) nên phải * 1.2f

            // Change Card To First Layer
            cardCanvas.overrideSorting = true;
            cardCanvas.sortingLayerName = "Interact";
            cardCanvas.sortingOrder = 1;
        };
        buttonUI.MouseDragEnd = () =>
        {
            //print("what");
            draggingCard = null;
            cardRect.DOScale(Vector2.one, 0.1f);
            cardRect.DOAnchorPos(Vector2.zero, 0.3f).SetUpdate(false);
            cardRect.DORotate(Vector2.zero, 0.3f).SetUpdate(false);

            //selectedCards.Remove(buttonUI);
            cardCanvas.overrideSorting = false;
        };
        buttonUI.MouseDrag = () =>
        {
        };
        buttonUI.ClickFunc = () =>
        {
            /*if (selectedCards.Contains(buttonUI))
            {
                selectedCards.Remove(buttonUI);
                cardRect.DOAnchorPos(Vector2.zero, 0.3f / 2).SetUpdate(false);
            }
            else if (selectedCards.Count < 5)
            {
                selectedCards.Add(buttonUI);
                cardRect.DOAnchorPosY((transform.position.y + 50f), 0.3f).SetUpdate(false);
            }*/
        };
        /*
        buttonUI.MouseDrop = () =>
        {
            if (draggingCard != null)
            {
                Debug.Log($"Thả card [{draggingCard.name}] vào [{obj.name}]");
            }
            else if (diceManager.DraggingDice != null)
            {
                var droppedDice = diceManager.DraggingDice;

                Debug.Log($"Thả dice [{droppedDice.name}] vào card [{obj.name}]");

                // fit dice to card 
                var diceplaceTransform = obj.transform.Find("diceplace");
                droppedDice.transform.SetParent(diceplaceTransform);
                droppedDice.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.1f);
                var diceTransfrom = droppedDice.GetComponent<RectTransform>();
                DOTween.To(() => diceTransfrom.offsetMin, x => diceTransfrom.offsetMin = x, Vector2.zero, 0.1f);
                DOTween.To(() => diceTransfrom.offsetMax, x => diceTransfrom.offsetMax = x, Vector2.zero, 0.3f);

                var droppedDiceCanvas = droppedDice.GetComponent<Canvas>();
                droppedDiceCanvas.overrideSorting = false;

                // Disable outline components
                var diceOutline = droppedDice.GetComponent<Outline>();
                diceOutline.enabled = false;

                // Remove frome SelectedDiceList 
                var diceComp = droppedDice.GetComponent<Dice>();
                if (diceManager.SelectedDice.Contains(diceComp))
                    diceManager.SelectedDice.Remove(diceComp);

                var diceGraphicRaycaster = diceComp.GetComponent<GraphicRaycaster>();
                diceGraphicRaycaster.enabled = true;
            }
        };*/
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

    static int ParentIndex(GameObject card)
    {
        return card.transform.parent.GetSiblingIndex();
    }

    void Swap(int index)
    {
        Transform focusedParent = draggingCard.transform.parent;
        Transform crossedParent = cards[index].transform.parent;

        // Swap transforms in hierarchy
        cards[index].transform.SetParent(focusedParent);
        draggingCard.transform.SetParent(crossedParent);

        // Animate the swapped card
        DOTween.Kill(3, true);
        var rt = cards[index].GetComponent<RectTransform>();
        rt.DOAnchorPos(Vector2.zero, 0.1f).SetId(3);
        rt.DOPunchRotation(Vector3.forward * 15, hoverTransition, 5, 1).SetId(3);

        // Swap in the list
        int dragIndex = cards.IndexOf(draggingCard);
        (cards[dragIndex], cards[index]) = (cards[index], cards[dragIndex]);

        // Optional: reassign layout/visual order
        bool swapIsRight = ParentIndex(cards[index]) > ParentIndex(draggingCard);
        //cards[index].cardVisual.Swap(swapIsRight ? -1 : 1);

        foreach (var card in cards)
        {
            // Update visual index or position based on new order
            //card.cardVisual.UpdateIndex(...);
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
