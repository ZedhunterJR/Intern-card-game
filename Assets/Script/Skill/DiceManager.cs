using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.Serialization.Formatters;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class DiceManager : Singleton<DiceManager>
{
    #region Register variable
    [SerializeField] public int baseDiceNum = 5;
    [HideInInspector] public int currentDiceNum;

    [SerializeField] Transform[] diceHolders;
    [SerializeField] GameObject dicePrefabs;
    //[SerializeField] Transform diceHolder;
    public List<Dice> diceList;
    public List<ButtonUI> diceFace;

    // var for reroll
    [Header("Reroll Parameters")]
    [SerializeField] List<Dice> selectedDice;
    public List<Dice> SelectedDice => selectedDice;
    [SerializeField] bool isRolling = false;
    [SerializeField] RectTransform diceHolderLid;
    [SerializeField] RectTransform diceHolderContain;

    // var for dragging 
    [Header("Dragging Parameters")]
    [SerializeField] ButtonUI draggingDice;
    public ButtonUI DraggingDice => draggingDice;
    Vector3 offset;

    private Dictionary<Dice, Skill> skillDicePair = new();

    //dunno
    public Dictionary<(Skill, string), Action<int>> DiceRerollListener = new();
    public Dictionary<DiceType, float> DiceTypeRate = new()
    {
        { DiceType.Gold, 0.3f },
        { DiceType.Twin, 0.5f },
        { DiceType.Rock, 0.4f },
        { DiceType.Gem, 1.2f }
    };
    public void AddOrUpdateDiceRate(DiceType type, float value)
    {
        if (DiceTypeRate.ContainsKey(type))
        {
            DiceTypeRate[type] += value;
        }
        else
        {
            DiceTypeRate[type] = value;
        }
    }

    private Dictionary<DiceType, int> currentDiceTypeCount = new();

    #endregion

    #region Unity Methods
    private void Awake()
    {
        currentDiceNum = baseDiceNum;
    }

    private void Start()
    {

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
            if (!GameManager.Instance.CanInteract)
                return;

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
            if (!GameManager.Instance.CanInteract)
                return;

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
            if (!GameManager.Instance.CanInteract)
                return;

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
                AttackSequence.Instance.UpdatePatternOnDicePlace();

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
        if (dice == null) return;

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
        //dice.GetComponent<GraphicRaycaster>().enabled = true;
    }
    [ContextMenu("Start Turn")]
    public void StartTurn()
    {
        //var dices = new List<Dice>(skillDicePair.Keys);

        //foreach (var d in dices)
        //{
        //    ReturnDice(d);
        //}
        skillDicePair = new();
        diceList.Clear();

        foreach (var card in SkillManager.Instance.Skills())
        {
            card.diceFace = null;
        }
        SkillManager.Instance.canPlay = true;

        for (int i = 0; i < currentDiceNum; i++)
        {
            var dice = PoolingObject.Instance.GetDiceFromPool();
            dice.transform.SetParent(diceHolders[i]);
            dice.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            //dice.gameObject.SetActive(true);
            diceList.Add(dice);
        }

        int diceCount = 0;

        foreach (var dice in diceList)
        {
            AddDiceEvent(dice);
            dice.gameObject.name = $"Dice {diceCount}";
            diceCount++;
        }
        foreach (var d in diceList)
            d.StartRound();
        StartCoroutine(RerollAnim(diceList));
    }
    public void RerollAction()
    {
        if (!isRolling && selectedDice.Count != 0 &&
            GameManager.Instance.GameStatus == GameStatus.Battle && GameManager.Instance.CurrentNumOfReroll >= 0)
        {
            GameManager.Instance.SetRerolls();
            StartCoroutine(RerollAnim(new List<Dice>(selectedDice)));
            foreach (var pair in DiceRerollListener)
            {
                pair.Value?.Invoke(selectedDice.Count);
            }
            selectedDice.Clear();

        }
        else
        {
            if (!isRolling)
            {
                DOTween.Kill(2, true);
                diceHolderContain.DOPunchRotation(Vector3.forward * 5f, 0.15f, 20, 1).SetId(2);
            }
        }
    }

    public IEnumerator RerollAnim(List<Dice> diceList)
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

            shakeSeq.Append(diceHolderContain.DOAnchorPosY(targetY, duration));
        }
        foreach (Dice dice in diceList)
        {
            dice.gameObject.SetActive(true);
        }
        RerollDices(diceList);
        yield return shakeSeq.WaitForCompletion();

        yield return diceHolderLid.DOAnchorPos(new Vector2(-200, 0), 0.8f)
        .SetEase(Ease.InCubic)
        .WaitForCompletion();

        isRolling = false;
    }

    public void RerollDices(List<Dice> diceList)
    {
        AssignDiceTypesWithOverflow(diceList);
        foreach (Dice dice in diceList)
        {
            RerollIndividualDice(dice);
            dice.UpdateDiceInternal();
            dice.GetComponent<Outline>().enabled = false;
        }
    }
    private void RerollIndividualDice(Dice dice)
    {
        if (dice.CurrentDiceType == DiceType.Normal)
            dice.includedInPoint = false;
        if (dice.CurrentDiceType == DiceType.Frozen)
            return;
        dice.currentFace = Random.Range(1, 7);

        if (dice.CurrentDiceType == DiceType.Gem)
            dice.currentFace = 6;
        if (dice.CurrentDiceType == DiceType.Low)
            dice.currentFace = Random.Range(1, 4);
        if (dice.CurrentDiceType == DiceType.High)
            dice.currentFace = Random.Range(4, 7);
        if (dice.CurrentDiceType == DiceType.Rock)
            dice.currentFace = -1;

        //ChangeImage(currentFace);
    }
    private void AssignDiceTypesWithOverflow(List<Dice> diceList)
    {
        if (diceList == null || diceList.Count == 0)
            return;

        //currentDiceTypeCount.Clear();

        // Reset all dice to Normal
        foreach (var dice in diceList)
        {
            if (currentDiceTypeCount.TryGetValue(dice.CurrentDiceType, out int count))
            {
                count--;
                if (count <= 0)
                    currentDiceTypeCount.Remove(dice.CurrentDiceType); // Clean up entry
                else
                    currentDiceTypeCount[dice.CurrentDiceType] = count; // Re-assign updated value
            }

            dice.CurrentDiceType = DiceType.Normal;
        }


        // Make a mutable list of available dice to assign
        var availableDice = new List<Dice>(diceList.OrderBy(_ => UnityEngine.Random.value));

        foreach (var kvp in DiceTypeRate)
        {
            var type = kvp.Key;
            if (type == DiceType.Normal) continue;

            float rate = kvp.Value;
            int guaranteed = Mathf.FloorToInt(rate);
            float chance = rate - guaranteed;

            // Assign guaranteed count
            for (int i = 0; i < guaranteed && availableDice.Count > 0; i++)
            {
                AssignTypeToOne(type, availableDice);
            }

            // Assign based on remaining fractional chance
            if (availableDice.Count > 0 && UnityEngine.Random.value <= chance)
            {
                AssignTypeToOne(type, availableDice);
            }
        }

        // Helper local function
        void AssignTypeToOne(DiceType typeToAssign, List<Dice> pool)
        {
            var die = pool[0];
            pool.RemoveAt(0);
            die.CurrentDiceType = typeToAssign;

            if (!currentDiceTypeCount.ContainsKey(typeToAssign))
                currentDiceTypeCount[typeToAssign] = 0;

            currentDiceTypeCount[typeToAssign]++;
        }
    }

    public void UpdateDiceGraphic(Dice dice)
    {

    }

    #endregion
}