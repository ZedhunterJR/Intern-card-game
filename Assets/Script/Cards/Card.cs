using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Register Variable
    private Vector3 offset;
    private RectTransform cardRect;

    [Header("Movement")]
    [SerializeField] private float moveSpeedLimit = 50;

    [Header("Stats")]
    [SerializeField] private bool isDragging;
    [SerializeField] private bool isSelected = false;
    [SerializeField] private bool isHovering;

    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> BeginDragEvent;
    [HideInInspector] public UnityEvent<Card> EndDragEvent;
    [HideInInspector] public UnityEvent<Card> OnDragEvent;
    [HideInInspector] public UnityEvent<Card> PointerDownEvent;
    [HideInInspector] public UnityEvent<Card> PointerUpEvent;
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;

    [Header("Performance")]
    private int frameCounter = 0;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        cardRect = GetComponent<RectTransform>();
    }

    private void Start()
    {

    }

    public void Update()
    {
        if (isDragging)
        {

            Vector2 targetPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            Vector2 direction = (targetPostion - (Vector2)transform.position).normalized;
            //Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(transform.position, targetPostion) / Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPostion, 0.1f);
        }

        ClampScreen();
    }
    #endregion

    #region Methods
    void ClampScreen()
    {
        Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
        Vector2 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Vector2 clampPosition = transform.position;
        if (transform.position.x > topRight.x) clampPosition.x = topRight.x;
        if (transform.position.x < bottomLeft.x) clampPosition.x = bottomLeft.x;
        if (transform.position.y > topRight.y) clampPosition.y = topRight.y;
        if (transform.position.y < bottomLeft.y) clampPosition.y = bottomLeft.y;

        transform.position = clampPosition;
    }
    #endregion

    #region Pointer
    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent.Invoke(this);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = mousePosition - (Vector2)transform.position;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragEvent.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent.Invoke(this);
        isDragging = false;

        cardRect.DOAnchorPos(Vector2.zero, .3f).SetUpdate(true);
        isSelected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDownEvent.Invoke(this);
        isSelected = !isSelected;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUpEvent.Invoke(this);

        if (isSelected) cardRect.DOAnchorPosY((transform.position.y + 50f), 0.3f).SetUpdate(true);
        else if (!isSelected) cardRect.DOAnchorPos(Vector2.zero, 0.3f/2).SetUpdate(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent.Invoke(this);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);
        isHovering = false;
    }

    #endregion
}
