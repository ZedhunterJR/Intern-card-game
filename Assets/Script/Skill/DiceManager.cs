using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.Serialization.Formatters;
using UnityEngine.UI;

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
    [SerializeField] List<Dice> selectedDice;

    // var for dragging 
    [SerializeField] ButtonUI draggingDice;
    public ButtonUI DraggingDice => draggingDice;
    Vector3 offset;
    Vector3 previousPosition;
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            RerollDices(selectedDice);
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
        buttonUI.ClickFunc = () =>
        {
            if (draggingDice == null)
            {
                if (selectedDice.Contains(dice))
                {
                    selectedDice.Remove(dice);
                    diceOutline.enabled = false;
                }
                else
                {
                    selectedDice.Add(dice);
                    diceOutline.enabled = true;
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
            previousPosition = draggingDice.transform.position;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = mousePosition - (Vector2)dice.transform.position;

            diceCanvas.overrideSorting = true;
            diceCanvas.sortingLayerName = "Interact";
            diceCanvas.sortingOrder = 2;
        };
        buttonUI.MouseDragEnd = () =>
        {
            draggingDice = null;
            diceRect.DOMove(previousPosition, 0.3f).SetUpdate(false);

            diceCanvas.overrideSorting = false;
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
