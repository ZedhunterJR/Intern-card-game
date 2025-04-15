using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class CardHolder : MonoBehaviour
{
    #region Register variable
    Vector3 offset;
    [SerializeField] List<ButtonUI> selectedCards = new();

    [SerializeField] ButtonUI draggingCard;

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
            SwapCheck();
        }
    }
    #endregion

    #region Methods
    void AddCardEvent(GameObject gameObject)
    {
        var buttonUI = gameObject.GetComponent<ButtonUI>();
        var cardRect = gameObject.GetComponent<RectTransform>();
        var cardCanvas = gameObject.GetComponent<Canvas>();
        buttonUI.MouseDragBegin = () =>
        {
            draggingCard = buttonUI;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = mousePosition - (Vector2)gameObject.transform.position;
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

            cardRect.DOAnchorPos(Vector2.zero, .3f).SetUpdate(true);
            selectedCards.Remove(buttonUI);
            cardCanvas.overrideSorting = false;
        };
        buttonUI.ClickFunc = () =>
        {
            if (selectedCards.Contains(buttonUI))
            {
                selectedCards.Remove(buttonUI);
                cardRect.DOAnchorPos(Vector2.zero, 0.3f / 2).SetUpdate(true);
            }
            else if (selectedCards.Count < 5)
            {
                selectedCards.Add(buttonUI);
                cardRect.DOAnchorPosY((transform.position.y + 50f), 0.3f).SetUpdate(true);
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
        if (selectedCards.Contains(cards[index]))
        {
            cards[index].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0,50f), 0.01f).SetUpdate(true);
        }
        else
        {
            cards[index].GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.3f);
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
    #endregion
}
