using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public abstract class Button_Base : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public abstract void OnMouseEnter();
    public abstract void OnMouseLeave();
    
    public UnityEvent OnClick;
    [SerializeField] protected RectTransform rectTransform;
    protected Image image;
    protected Color originalColor;
    [SerializeField] private float clickDelay;
    private bool isActive = true;
    
    protected void OnEnable()
    {
        image.color = originalColor;
    }
    protected void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        //rectTransform ??= GetComponent<RectTransform>();
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

}