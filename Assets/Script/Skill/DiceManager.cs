using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.Serialization.Formatters;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class DiceManager : Singleton<DiceManager>
{
    #region Register variable
    [SerializeField] int baseDiceNum = 5;
    private int currentDiceNum;

    [SerializeField] Transform[] diceHolders;
    [SerializeField] GameObject dicePrefabs;
    //[SerializeField] Transform diceHolder;
    public List<Dice> diceList;
    public List<ButtonUI> diceFace;

    // var for reroll
    [Header("Reroll Parameters")]
    [SerializeField] List<Dice> selectedDice;
    public List<Dice> SelectedDice;
    [SerializeField] bool isRolling = false;
    [SerializeField] RectTransform diceHolderLid;
    [SerializeField] RectTransform diceHolderContain;

    // var for dragging 
    [Header("Dragging Parameters")]
    [SerializeField] ButtonUI draggingDice;
    public ButtonUI DraggingDice => draggingDice;
    Vector3 offset;

    private Dictionary<Dice, Skill> skillDicePair = new();

    #endregion

    #region Unity Methods
    private void Awake()
    {
        currentDiceNum = baseDiceNum;
    }

    private void Start()
    {
        for (int i = 0; i < baseDiceNum; i++)
        {
            Dice dice = Instantiate(dicePrefabs, diceHolders[i]).GetComponent<Dice>();
            diceList.Add(dice);
        }

        int diceCount = 0;

        foreach (var dice in diceList)
        {
            AddDiceEvent(dice);
            dice.OnStart();
            dice.gameObject.name = $"Dice {diceCount}";
            diceCount++;
        }
    }

    private void Update()
    {
        if (draggingDice != null)
        {
            Vector2 targetPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            Vector2 direction = (targetPostion - (Vector2)draggingDice.transform.position).normalized;
            draggingDice.transform.position = Vector3.Lerp(draggingDice.transform.position, targetPostion, 0.1f);
            draggingDice.transform.position = ClampScreen(draggingDice.transform.position);
        }

    }
    #endregion

    #region Methods
    void AddDiceEvent(Dice dice)
    {
        var buttonUI = dice.GetComponent<ButtonUI>();
        var diceRect = dice.GetComponent<RectTransform>();
        var diceCanvas = dice.GetComponent<Canvas>();
        var diceOutline = dice.GetComponent<Outline>();
        var diceGraphic = dice.GetComponent<GraphicRaycaster>();
        buttonUI.ClickFunc = () =>
        {
            if (draggingDice == null)
            {
                if (selectedDice.Contains(dice) && GameManager.Instance.GameStatus == GameStatus.Battle)
                {
                    selectedDice.Remove(dice);
                    diceOutline.enabled = false;
                }
                else
                {
                    if (GameManager.Instance.GameStatus == GameStatus.Battle)
                    {
                        selectedDice.Add(dice);
                        diceOutline.enabled = true;
                    }
                }
            }
        };
        buttonUI.MouseHoverEnter = () =>
        {

        };
        buttonUI.MouseHoverExit = () =>
        {

        };
        buttonUI.MouseDragBegin = () =>
        {
            draggingDice = buttonUI;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = mousePosition - (Vector2)dice.transform.position;
            diceGraphic.enabled = false;

            diceCanvas.overrideSorting = true;
            diceCanvas.sortingLayerName = "Interact";
            diceCanvas.sortingOrder = 2;
        };
        buttonUI.MouseDragEnd = () =>
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Skill found = null;
            foreach (RaycastResult result in results)
            {
                Skill card = result.gameObject.GetComponent<Skill>();
                if (card != null)
                {
                    found = card;
                    break;
                }
            }

            if (found == null)
            {
                ReturnDice(dice);
                diceCanvas.overrideSorting = false;
            }
            else
            {
                if (skillDicePair.TryGetValue(dice, out Skill s))
                {
                    s.diceFace = null;
                    skillDicePair.Remove(dice);
                }
                if (found.diceFace != null)
                {
                    ReturnDice(found.diceFace);
                }

                // Set skill dice face 
                found.AddDice(dice);
                skillDicePair[dice] = found;

                if (selectedDice.Contains(draggingDice.GetComponent<Dice>()))
                {
                    selectedDice.Remove(draggingDice.GetComponent<Dice>());
                }

                draggingDice.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.1f);

                var droppedDiceCanvas = draggingDice.GetComponent<Canvas>();
                droppedDiceCanvas.overrideSorting = false;

                // Disable outline components
                var diceOutline = draggingDice.GetComponent<Outline>();
                diceOutline.enabled = false;

                // Remove frome SelectedDiceList 
                var diceComp = draggingDice.GetComponent<Dice>();
                if (SelectedDice.Contains(diceComp))
                    SelectedDice.Remove(diceComp);

                var diceGraphicRaycaster = diceComp.GetComponent<GraphicRaycaster>();
                diceGraphicRaycaster.enabled = true;
            }

            draggingDice = null;
            diceGraphic.enabled = true;
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
    public void ReturnDice(Dice dice)
    {
        Transform toReturn = null;
        for (int i = 0; i < diceHolders.Length; i++)
        {
            if (diceHolders[i].childCount == 0)
            {
                toReturn = diceHolders[i];
                break;
            }
        }
        if (toReturn == null) return;

        if (skillDicePair.TryGetValue(dice, out Skill s))
        {
            s.diceFace = null;
            skillDicePair.Remove(dice);
        }
        dice.transform.SetParent(toReturn);
        dice.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.2f);
    }

    public void RerollAction()
    {
        if (!isRolling && selectedDice.Count != 0 && GameManager.Instance.GameStatus == GameStatus.Battle)
        {
            StartCoroutine(RerollAnim());
        }
        else
        {
            DOTween.Kill(2, true);
            diceHolderContain.DOPunchRotation(Vector3.forward * 5f, 0.15f, 20, 1).SetId(2);
        }
    }

    IEnumerator RerollAnim()
    {
        isRolling = true;
        yield return diceHolderLid.DOAnchorPos(new Vector2(0, 0), 1.2f)
          .SetEase(Ease.OutCubic)
          .WaitForCompletion();

        float[] shakeHeights = { 15f, 5f, 12f, 5f };
        float[] shakeDurations = { 0.1f, 0.1f, 0.08f, 0.08f };
        Sequence shakeSeq = DOTween.Sequence();
        for (int i = 0; i < shakeHeights.Length; i++)
        {
            float targetY = shakeHeights[i];
            float duration = shakeDurations[i];

            //shakeSeq.Append(diceHolderLid.DOAnchorPosY(targetY, duration));
            shakeSeq.Append(diceHolderContain.DOAnchorPosY(targetY, duration));
        }
        RerollDices(selectedDice);
        yield return shakeSeq.WaitForCompletion();

        yield return diceHolderLid.DOAnchorPos(new Vector2(-200, 0), 0.8f)
        .SetEase(Ease.InCubic)
        .WaitForCompletion();

        isRolling = false;
    }

    public void RerollDices(List<Dice> diceList)
    {
        foreach (Dice dice in diceList)
        {
            dice.Reroll();
        }
    }

    public void UpdateDiceGraphic(Dice dice)
    {

    }

    #endregion
}