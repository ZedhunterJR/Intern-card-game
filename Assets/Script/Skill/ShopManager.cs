using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] GameObject shopItemPrefab;
    [SerializeField] Transform cardPart;
    private int numberOfCardPart = 2;

    [SerializeField] Transform relicPart;
    [SerializeField] GameObject relicPrefab;

    public Transform selectedShopItem = null;
    private DataSpriteManager dataSpriteManager;
    private Vector3 offset;
    private void Awake()
    {
        dataSpriteManager = DataSpriteManager.Instance;
    }

    private void Update()
    {
        if (selectedShopItem != null)
        {
            Vector2 targetPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            Vector2 direction = (targetPostion - (Vector2)selectedShopItem.transform.position).normalized;
            selectedShopItem.transform.position = Vector3.Lerp(selectedShopItem.transform.position, targetPostion, 0.1f);
            selectedShopItem.transform.position = ClampScreen(selectedShopItem.transform.position);

            float targetZRotation = -direction.x * 20f;

            targetZRotation = Mathf.Clamp(targetZRotation, -60f, 60f);
            float smoothedZ = Mathf.LerpAngle(selectedShopItem.transform.eulerAngles.z, targetZRotation, 20f * Time.deltaTime);
            selectedShopItem.transform.rotation = Quaternion.Euler(0f, 0f, smoothedZ);
        }
    }
    public void Restock()
    {
        List<Transform> cards = new List<Transform>();
        foreach (Transform child in cardPart.transform)
            cards.Add(child);

        DG.Tweening.Sequence shakeSeq = DOTween.Sequence();

        Vector3 punchRotation = Vector3.forward * 5f; // xoay nhẹ quanh trục Z
        float duration = 0.15f;    // thời gian rung
        int vibrato = 20;          // số lần rung
        float elasticity = 1f;     // độ bật lại

        foreach (Transform card in cards)
        {
            shakeSeq.Join(
                card
                    .DOPunchRotation(punchRotation, duration, vibrato, elasticity)
                    .SetEase(Ease.Linear)
            );
        }

        shakeSeq.OnComplete(() =>
        {
            foreach (Transform child in cards)
                Destroy(child.gameObject);

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
        var priceValueEffect = shopitem.transform.Find("imageprice").transform.Find("price").GetComponent<TextMeshProUGUI>();
        var inforEffect = shopitem.transform.Find("infor").gameObject;
        var inforCanvas = inforEffect.GetComponent<Canvas>();
        var desEffect = inforEffect.transform.Find("des").GetComponent<TextMeshProUGUI>();

        title.text = effect.name;
        effectImage.sprite = dataSpriteManager.GetSpriteCard(effect.id);
        desEffect.text = effect.description;
        priceValueEffect.text = $"${effect.cost}";

        var previourPosItem = Vector2.zero;

        buttonUI.MouseDragBegin = () =>
        {
            selectedShopItem = shopitem.transform;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = mousePosition - (Vector2)selectedShopItem.transform.position;

            canvas.overrideSorting = true;
            canvas.sortingLayerName = "Interact";
            canvas.sortingOrder = 10;
            inforEffect.SetActive(false);
            previourPosItem = selectedShopItem.transform.position;
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

            if (found != null)
            {
                Destroy(shopitem);
                found.ChangeEffect(effect, dataSpriteManager.GetSpriteCard(effect.id));
            }
            else
            {
                //selectedShopItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                selectedShopItem.DOMove(previourPosItem, 0.3f);
                selectedShopItem.GetComponent<RectTransform>().DORotate(Vector3.zero, 0.3f);
                canvas.overrideSorting = false;
            }

            selectedShopItem = null;
        };
        buttonUI.ClickFunc = () =>
        {
            //if (selectedShopItem == null)
            //{
            //    //IventoryManager.Instance.OpenIventory(true, CardPartType.Condition);
            //    selectedShopItem = shopitem.transform;
            //    canvas.overrideSorting = true;
            //    canvas.sortingLayerName = "Interact";
            //    canvas.sortingOrder = 10;
            //    inforEffect.SetActive(false);
            //    previourPosItem = selectedShopItem.transform.position;
            //}
            //else
            //{
            //    PointerEventData pointerData = new PointerEventData(EventSystem.current)
            //    {
            //        position = Input.mousePosition
            //    };

            //    List<RaycastResult> results = new List<RaycastResult>();
            //    EventSystem.current.RaycastAll(pointerData, results);

            //    Skill found = null;
            //    foreach (RaycastResult result in results)
            //    {
            //        Skill card = result.gameObject.GetComponent<Skill>();
            //        if (card != null)
            //        {
            //            found = card;
            //            break;
            //        }
            //    }

            //    if (found != null)
            //    {
            //        Destroy(shopitem);
            //        found.ChangeEffect(effect, dataSpriteManager.EffectSprites[effect.id]);
            //    }
            //    else
            //    {
            //        //selectedShopItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            //        selectedShopItem.transform.position = previourPosItem;
            //        canvas.overrideSorting = false;
            //    }

            //    selectedShopItem = null;
            //}
        };
        buttonUI.MouseHoverEnter = () =>
        {
            if (selectedShopItem == null)
            {
                inforEffect.SetActive(true);
                inforCanvas.overrideSorting = true;
                inforCanvas.sortingLayerName = "Interact";
            }
        };
        buttonUI.MouseHoverExit = () =>
        {
            inforEffect.SetActive(false);
        };
    }

    public void RelicItem()
    {
        var relic = Instantiate(relicPrefab, relicPart);
        var buttonUI = relic.GetComponent<ButtonUI>();

        // Relic param
        var title = relic.transform.Find("title").GetComponent<TextMeshProUGUI>();
        var image = relic.transform.Find("image").GetComponent<Image>();
        var infor = relic.transform.Find("infor").gameObject;
        var inforCanvas = relic.transform.Find("infor").GetComponent<Canvas>();
        var inforDes = infor.transform.Find("des").GetComponent<TextMeshProUGUI>();
        var price = relic.transform.Find("price").gameObject;
        var priceCanvas = relic.transform.Find("price").GetComponent<Canvas>();
        var priceDes = price.transform.Find("priceDes").GetComponent<TextMeshProUGUI>();
        var buyButton = relic.transform.Find("buy").GetComponent<Button>();
        var buyButtonCanvas = relic.transform.Find("buy").GetComponent<Canvas>();

        title.text = "testRelic";
        image.sprite = DataSpriteManager.Instance.GetSpriteCard("test");
        priceDes.text = "999";
        buyButton.onClick.AddListener(TestBuy);

        buttonUI.MouseHoverEnter = () =>
        {
            infor.SetActive(true);
            inforCanvas.overrideSorting = true;
            inforCanvas.sortingLayerName = "Interact";

            buyButton.gameObject.SetActive(true);
            buyButtonCanvas.overrideSorting = true;
            buyButtonCanvas.sortingLayerName = "Interact";

            price.gameObject.SetActive(true);
            priceCanvas.overrideSorting = true;
            priceCanvas.sortingLayerName = "Interact";
        };
        buttonUI.MouseHoverExit = () =>
        {
            infor.SetActive(false);
            buyButton.gameObject.SetActive(false);
            price.gameObject.SetActive(false);
        };
    }

    void TestBuy()
    {
        Debug.Log("test");
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
}
