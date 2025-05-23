using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataSpriteManager : Singleton<DataSpriteManager>
{
    [SerializeField] Sprite spriteTemp;
    public Dictionary<string, Sprite> EffectSprites = new Dictionary<string, Sprite>();
    [SerializeField] GameObject pattentPrefab;
    [SerializeField] Transform pattentHolder;
    private Dictionary<DicePattern, int> dicePattent = new Dictionary<DicePattern, int>()
    {
        { DicePattern.Single, 1 },
        { DicePattern.OnePair, 1 },
        { DicePattern.TwoPair, 1 },
        { DicePattern.ThreeOfAKind, 1 },
        { DicePattern.FourOfAKind, 1 },
        { DicePattern.FiveOfAKind, 1 },
        { DicePattern.SixOfAKind, 0 },
        { DicePattern.FullHouse, 1 },
        { DicePattern.TwoTriplet, 0 },
        { DicePattern.ThreePair, 0 },
        { DicePattern.FullSixes, 1 },
        { DicePattern.Sequence3, 0 },
        { DicePattern.Sequence4, 0 },
        { DicePattern.Sequence5, 0 },
        { DicePattern.Sequence6, 0 }
    };
    [SerializeField] List<Sprite> pattentSprite;
    private Dictionary<DicePattern, Sprite> dicSpritePattent = new Dictionary<DicePattern, Sprite>()
    {
        { DicePattern.Single, null },
        { DicePattern.OnePair, null },
        { DicePattern.TwoPair, null },
        { DicePattern.ThreeOfAKind, null },
        { DicePattern.FourOfAKind, null },
        { DicePattern.FiveOfAKind, null },
        { DicePattern.SixOfAKind, null },
        { DicePattern.FullHouse, null },
        { DicePattern.TwoTriplet, null },
        { DicePattern.ThreePair, null },
        { DicePattern.FullSixes, null },
        { DicePattern.Sequence3, null },
        { DicePattern.Sequence4, null },
        { DicePattern.Sequence5, null },
        { DicePattern.Sequence6, null }
    };

    private void Awake()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprite");

        foreach (Sprite s in sprites)
        {
            EffectSprites[s.name] = s;
        }
        AddSpriteToDic();
        InitPattents();
    }

    public Sprite GetSpriteCard(string cardName)
    {
        if (EffectSprites.TryGetValue(cardName, out Sprite card))
        {
            return card;
        }
        return spriteTemp;
    }

    void InitPattents()
    {
        foreach (var pattent in dicePattent)
        {
            if (pattent.Value == 1)
            {
                GameObject obj = Instantiate(pattentPrefab, pattentHolder);
                AddEffect(obj, pattent.Key);
            }
        }
    }

    void AddEffect(GameObject obj, DicePattern dicePattern)
    {
        obj.name = Global.Instance.DisplayNames[dicePattern];
        var buttonUI = obj.GetComponent<ButtonUI>();

        var pattentName = obj.transform.Find("name").GetComponent<TextMeshProUGUI>();
        var pattentLvl = obj.transform.Find("lv").transform.Find("lvl").GetComponent<TextMeshProUGUI>();
        var pattentPoint = obj.transform.Find("point").transform.Find("value").GetComponent<TextMeshProUGUI>();
        var inforObj = obj.transform.Find("infor").gameObject;
        var inforCanvas = obj.transform.Find("infor").GetComponent<Canvas>();
        var img = inforObj.transform.Find("img").GetComponent<Image>();

        // Handle
        pattentName.text = obj.name;
        pattentPoint.text = $"{Global.Instance.GetPatternPoints(dicePattern)}";
        img.sprite = dicSpritePattent[dicePattern];

        buttonUI.MouseHoverEnter = () =>
        {
            inforCanvas.sortingOrder = 30;
            inforCanvas.sortingLayerName = "Interact";
            
            inforObj.SetActive(true);
        };
        buttonUI.MouseHoverExit = () =>
        {
            inforObj.SetActive(false);
        };
    }

    void EnablePattern(DicePattern pattern)
    {
        if (dicePattent.ContainsKey(pattern))
        {
            dicePattent[pattern] = 1;
            GameObject obj = Instantiate(pattentPrefab, pattentHolder);
            AddEffect(obj, pattern);
        }
    }
    void AddSpriteToDic()
    {
        foreach (var key in dicSpritePattent.Keys.ToList())
        {
            var pattentName = Global.Instance.DisplayNames[key];

            // DEBUG: In ra để kiểm tra
            Debug.Log($"Trying to match pattern '{key}' with name '{pattentName}'");

            var match = pattentSprite.FirstOrDefault(sprite => sprite.name == pattentName);

            if (match != null)
            {
                dicSpritePattent[key] = match;
                Debug.Log($"Gán sprite: {pattentName} to {match.name}");
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy sprite có tên: {pattentName}");
            }
        }
    }

}
