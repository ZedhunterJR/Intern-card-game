using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonUI : MonoBehaviour, IPointerClickHandler, IDragHandler, IDropHandler,
    IEndDragHandler, IBeginDragHandler
{
    [Header("Debug")]
    public bool interactable = true;
    //public float timerHover = 0;

    public Action ClickFunc = null;
    public Action MouseRightClickFunc = null;
    public Action MouseMiddleClickFunc = null;
    public Action MouseDragBegin = null;
    public Action MouseDrag = null;
    public Action MouseDragEnd = null;
    public Action MouseDrop = null;

    private Image buttonImage;

    [Header("Button Sprites")]
    public Sprite normalSprite;
    public Sprite clickSprite;
    public Sprite disabledSprite;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (normalSprite != null)
            buttonImage.sprite = normalSprite;
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (interactable)
        {
            if (eventData.button == PointerEventData.InputButton.Left || Input.touchCount > 0)
                ClickFunc?.Invoke();
            if (eventData.button == PointerEventData.InputButton.Right)
                MouseRightClickFunc?.Invoke();
            if (eventData.button == PointerEventData.InputButton.Middle)
                MouseMiddleClickFunc?.Invoke();

            if (clickSprite != null)
                buttonImage.sprite = clickSprite;
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        MouseDragBegin?.Invoke();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        MouseDrag?.Invoke();
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        MouseDragEnd?.Invoke();
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        MouseDrop?.Invoke();
    }

    public void SetInteractable(bool value)
    {
        interactable = value;
        if (interactable)
        {
            if (normalSprite != null)
                buttonImage.sprite = normalSprite;
        }
        else
        {
            if (disabledSprite != null)
                buttonImage.sprite = disabledSprite;
        }
    }

    public void ResetButtonUI()
    {
        ClickFunc = null;
        MouseRightClickFunc = null;
        MouseMiddleClickFunc = null;
        MouseDragBegin = null;
        MouseDrag = null;
        MouseDragEnd = null;
        MouseDrop = null;
    }
}
