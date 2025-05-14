using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    public int currentFace;
    public List<int> diceFaces;
    public List<Sprite> diceSprites;
    [SerializeField] Image diceImage;
    [SerializeField] Image filter;
    [SerializeField] List<Sprite> filterSprites;
    private Dictionary<string, Sprite> filterSpritesDict = new();

    private DiceType currentDiceType = DiceType.Normal;
    public DiceType CurrentDiceType
    {
        get => currentDiceType;
        set
        {
            if (value == DiceType.Normal)
                filter.sprite = filterSpritesDict["blank"];
            if (value == DiceType.Rock)
            {
                filter.sprite = filterSpritesDict["rock"];
                includedInPoint = true;
            }
            if (value == DiceType.Gold)
                filter.sprite = filterSpritesDict["gold"];
            if (value == DiceType.Frozen)
                filter.sprite = filterSpritesDict["frozen"];
            if (value == DiceType.Twin)
                filter.sprite = filterSpritesDict["twin"];
            if (value == DiceType.Gem)
                filter.sprite = filterSpritesDict["gem"];
            currentDiceType = value;
        }
    }
    public bool usedInAttack = false;
    public bool includedInPoint = false;

    private void Awake()
    {
        foreach (var s in filterSprites)
        {
            filterSpritesDict[s.name] = s;
        }
    }
    
    public void StartRound()
    {
        Reroll();
    }

    public void Reroll()
    {
        if (CurrentDiceType == DiceType.Normal)
            includedInPoint = false;
        if (CurrentDiceType == DiceType.Frozen)
            return;
        currentFace = diceFaces.GetRandom();

        if (CurrentDiceType == DiceType.Gem)
            currentFace = 6;
        if (CurrentDiceType == DiceType.Low)
            currentFace = Random.Range(1, 4);
        if (CurrentDiceType == DiceType.High)
            currentFace = Random.Range(4, 7);

        ChangeImage(currentFace);
    }
    public int CalculatePoint()
    {
        if (CurrentDiceType == DiceType.Gold)
            return currentFace * 10;
        if (CurrentDiceType == DiceType.Twin)
            return currentFace * 2;
        if (CurrentDiceType == DiceType.Rock)
            return 20;
        if (CurrentDiceType == DiceType.Burn)
            return 0;
        return currentFace;
    }

    void ChangeImage(int index)
    {
        diceImage.sprite = diceSprites[index - 1];
    }

}

public enum DiceType
{
    Normal,
    Gold,
    Twin,
    Rock,
    Frozen,
    Gem,
    Low,
    High,
    Burn,
}