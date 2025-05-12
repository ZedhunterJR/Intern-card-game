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
    [SerializeField] Transform[] cardPartSlots;
    //public List<ShopItem> cardPartItem;
    private int numberOfCardPart = 5;

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
        for (int i = 0; i < numberOfCardPart; i++)
        {
            cardPartSlots[i].transform.Clear();
            EffectItem(i);
        }
    }

    void EffectItem(int slot)
    {
        var effect = Global.Instance.Effects.GetRandomValue();
        var shopitem = Instantiate(shopItemPrefab, cardPartSlots[slot]);
        var buttonUI = shopitem.GetComponent<ButtonUI>();
        var canvas = shopitem.GetComponent<Canvas>();

        var image = shopitem.GetComponent<Image>();
        image.material = null; // Reset to default
        Material runtimeHueMaterial = new Material(Shader.Find("Custom/UIHueFromColor"));
        runtimeHueMaterial.color = effect.color;
        image.material = runtimeHueMaterial; // Apply new instance

        var title = shopitem.transform.Find("title").GetComponent<TextMeshProUGUI>();
        var effectImage = shopitem.transform.Find("image").GetComponent<Image>();
        title.text = effect.name;
        effectImage.sprite = dataSpriteManager.EffectSprites[effect.id];

        buttonUI.ClickFunc = () =>
        {
            if (selectedShopItem == null)
            {
                //IventoryManager.Instance.OpenIventory(true, CardPartType.Condition);
                selectedShopItem = shopitem.transform;
                canvas.overrideSorting = true;
                canvas.sortingLayerName = "Interact";
                canvas.sortingOrder = 10;
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
    }
}
