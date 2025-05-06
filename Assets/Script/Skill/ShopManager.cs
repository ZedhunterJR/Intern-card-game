using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;


public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] GameObject shopItemPrefab;
    [SerializeField] Transform[] cardPartSlots;
    //public List<ShopItem> cardPartItem;
    private int numberOfCardPart = 5;

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
            int ran = Random.Range(0, 3);
            switch (ran)
            {
                case 0:
                    PosItem(i);
                    break;
                case 1:
                    ConditionItem(i);
                    break;
                case 2:
                    EffectItem(i);
                    break;
                default:
                    break;
            }
        }
    }

    void PosItem(int slot)
    {
        var pos = Global.Instance.Positions.GetRandom();
        var shopitem = Instantiate(shopItemPrefab, cardPartSlots[slot]);
        var buttonUI = shopitem.GetComponent<ButtonUI>();
        var canvas = shopitem.GetComponent<Canvas>();

        // Update Graphic
        shopitem.transform.Find("Tag").GetComponentInChildren<TextMeshProUGUI>().text = "Position";
        shopitem.transform.Find("Des").gameObject.SetActive(false);
        shopitem.transform.Find("Mult").GetComponentInChildren<TextMeshProUGUI>().text = Global.Instance.CalculatePosMul(pos).DecimalFormat(2, 1) + "x";

        var images = new List<Image>();
        var pTransform = shopitem.transform.Find("Position");
        foreach (Transform arrow in pTransform)
        {
            if (arrow == pTransform)
                continue;
            images.Add(arrow.GetComponent<Image>());
        }
        Global.Instance.UpdatePositionArrowGraphic(pos, images.ToArray());

        buttonUI.ClickFunc = () =>
        {
            if (selectedShopItem == null)
            {
                //IventoryManager.Instance.OpenIventory(true, CardPartType.Position);
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
                    found.ChangeSkillPosCondition(pos);
                    //IventoryManager.Instance.OpenIventory(false);
                    Destroy(shopitem);
                }
                else
                {
                    //IventoryManager.Instance.OpenIventory(false);
                    selectedShopItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    canvas.overrideSorting = false;
                }

                selectedShopItem = null;
            }
        };

    }
    void ConditionItem(int slot)
    {
        var con = Global.Instance.Conditions.GetRandomValue();
        var shopitem = Instantiate(shopItemPrefab, cardPartSlots[slot]);
        var buttonUI = shopitem.GetComponent<ButtonUI>();
        var canvas = shopitem.GetComponent<Canvas>();

        // Update Graphic
        shopitem.transform.Find("Tag").GetComponentInChildren<TextMeshProUGUI>().text = "Condition";
        shopitem.transform.Find("Des").GetComponentInChildren<TextMeshProUGUI>().text = con.description;
        shopitem.transform.Find("Mult").GetComponentInChildren<TextMeshProUGUI>().text = con.multiplier.DecimalFormat(2, 1) + "x";
        shopitem.transform.Find("Position").gameObject.SetActive(false);

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
                    found.ChangeActivateCondition(con);
                    //IventoryManager.Instance.OpenIventory(false);
                    Destroy(shopitem);
                }
                else
                {
                    //IventoryManager.Instance.OpenIventory(false);
                    selectedShopItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    canvas.overrideSorting = false;
                }

                selectedShopItem = null;
            }
        };
    }

    void EffectItem(int slot)
    {
        var con = Global.Instance.Effects.GetRandomValue();
        var shopitem = Instantiate(shopItemPrefab, cardPartSlots[slot]);
        var buttonUI = shopitem.GetComponent<ButtonUI>();
        var canvas = shopitem.GetComponent<Canvas>();

        // Update Graphic
        shopitem.transform.Find("Tag").GetComponentInChildren<TextMeshProUGUI>().text = "Effect";
        shopitem.transform.Find("Des").GetComponentInChildren<TextMeshProUGUI>().text = con.description;
        shopitem.transform.Find("Mult").gameObject.SetActive(false);
        shopitem.transform.Find("Position").gameObject.SetActive(false);

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
                    found.ChangeEffect(con);
                    //IventoryManager.Instance.OpenIventory(false);
                    Destroy(shopitem);
                }
                else
                {
                    //IventoryManager.Instance.OpenIventory(false);
                    selectedShopItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    canvas.overrideSorting = false;
                }

                selectedShopItem = null;
            }
        };
    }
}
