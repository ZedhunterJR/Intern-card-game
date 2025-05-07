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
    private int numberOfCardPart = 2;

    public Transform selectedShopItem;
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
        var con = Global.Instance.Effects.GetRandomValue();
        var shopitem = Instantiate(shopItemPrefab, cardPartSlots[slot]);
        var buttonUI = shopitem.GetComponent<ButtonUI>();
        var canvas = shopitem.GetComponent<Canvas>();

        var image = shopitem.GetComponent<Image>();
        image.material = null; // Reset to default
        Material runtimeHueMaterial = new Material(Shader.Find("Custom/UIHueFromColor"));
        runtimeHueMaterial.color = con.color;
        image.material = runtimeHueMaterial; // Apply new instance

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
