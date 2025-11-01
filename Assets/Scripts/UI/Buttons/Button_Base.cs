using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public abstract class Button_Base : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    public abstract void OnMouseEnter();
    public abstract void OnMouseLeave();
    
    public UnityEvent OnClick;
    public UnityEvent OnRelease;
    public UnityEvent OnInitialPress;
    [SerializeField] protected RectTransform rectTransform;
    protected Image image;
    protected Color originalColor;
    [SerializeField] private float clickDelay;
    private bool isActive = true;
    private float pressStartTime;
    public float PressDuration { get; private set; }
    public float CurrentPressDuration => isActive && pressStartTime > 0 ? Time.time - pressStartTime : 0;
    public bool IsPressed => isActive && pressStartTime > 0;
    protected void OnEnable()
    {
        image.color = originalColor;
    }
    protected void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isActive) OnMouseEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isActive) OnMouseLeave();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if(isActive) StartCoroutine(ClickDelay());
    }

    private IEnumerator ClickDelay()
    {
        yield return new WaitForSecondsRealtime(clickDelay);
        OnClick?.Invoke();
    }

    public void SetIsActive(bool active) => isActive = active;

    public void OnPointerUp(PointerEventData eventData)
    {
        OnRelease?.Invoke();
        PressDuration = Time.time - pressStartTime;
        pressStartTime = 0f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnInitialPress?.Invoke();
        pressStartTime = Time.time;
    }
}