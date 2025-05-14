using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] GameObject shopItemPrefab;
    [SerializeField] Transform cardPart;
    //[SerializeField] Transform[] cardPartSlots;
    //public List<ShopItem> cardPartItem;
    private int numberOfCardPart = 2;

    public Transform selectedShopItem;
    private DataSpriteManager dataSpriteManager;
    private void Awake()
    {
        dataSpriteManager = DataSpriteManager.Instance;
    }

    private void Update()
    {
        if (selectedShopItem != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedShopItem.position = mousePos;
        }
    }
    public void Restock()
    {
        // 1) Lấy tất cả các card con hiện có
        List<Transform> cards = new List<Transform>();
        foreach (Transform child in cardPart.transform)
            cards.Add(child);

        // 2) Tạo 1 sequence để đồng bộ mọi shake
        DG.Tweening.Sequence shakeSeq = DOTween.Sequence();

        float shakeDuration = 0.5f;     // tổng thời gian rung
        Vector3 shakeStrength = new Vector3(10, 10, 0); // biên độ rung
        int vibrato = 20;               // số lần rung
        float randomness = 90f;         // độ ngẫu nhiên góc

        foreach (Transform card in cards)
        {
            // join để mọi card cùng rung song song
            shakeSeq.Join(
                card
                    .DOShakePosition(shakeDuration, shakeStrength, vibrato, randomness)
                    .SetEase(Ease.Linear)
            );
            // hoặc muốn rung xoay:
            // shakeSeq.Join(card.DOShakeRotation(shakeDuration, new Vector3(0,0,30), vibrato, randomness));
        }

        // 3) Khi rung xong -> clear và tạo lại
        shakeSeq.OnComplete(() =>
        {
            // Xoá hết card cũ
            foreach (Transform child in cards)
                Destroy(child.gameObject);

            // Sinh lại theo số lượng
            for (int i = 0; i < numberOfCardPart; i++)
                EffectItem();
        });
    }   

    void EffectItem()
    {
        var effect = Global.Instance.Effects.GetRandomValue();
        var shopitem = Instantiate(shopItemPrefab, cardPart);
        var buttonUI = shopitem.GetComponent<ButtonUI>();
        var canvas = shopitem.GetComponent<Canvas>();

        var image = shopitem.GetComponent<Image>();
        image.material = null; // Reset to default
        Material runtimeHueMaterial = new Material(Shader.Find("Custom/UIHueFromColor"));
        runtimeHueMaterial.color = effect.color;
        image.material = runtimeHueMaterial; // Apply new instance

        var title = shopitem.transform.Find("title").GetComponent<TextMeshProUGUI>();
        var effectImage = shopitem.transform.Find("image").GetComponent<Image>();
        var inforEffect = shopitem.transform.Find("infor").gameObject;
        var inforCanvas = inforEffect.GetComponent<Canvas>();
        var desEffect = inforEffect.transform.Find("des").GetComponent<TextMeshProUGUI>();
        title.text = effect.name;
        effectImage.sprite = dataSpriteManager.EffectSprites[effect.id];
        desEffect.text = effect.description;

        buttonUI.ClickFunc = () =>
        {
            if (selectedShopItem == null)
            {
                //IventoryManager.Instance.OpenIventory(true, CardPartType.Condition);
                selectedShopItem = shopitem.transform;
                canvas.overrideSorting = true;
                canvas.sortingLayerName = "Interact";
                canvas.sortingOrder = 10;
                inforEffect.SetActive(false);
            }
            else
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

                if (found != null)
                {
                    Destroy(shopitem);
                    found.ChangeEffect(effect, dataSpriteManager.EffectSprites[effect.id]);
                }
                else
                {
                    selectedShopItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    canvas.overrideSorting = false;
                }

                selectedShopItem = null;
            }
        };
        buttonUI.MouseHoverEnter = () =>
        {
            inforEffect.SetActive(true);
            inforCanvas.overrideSorting = true;
            inforCanvas.sortingLayerName = "Interact";
        };
        buttonUI.MouseHoverExit = () =>
        {
            inforEffect.SetActive(false);
        };
    }
}
