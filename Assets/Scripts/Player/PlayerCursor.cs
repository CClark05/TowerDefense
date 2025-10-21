using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCursor : Singleton<PlayerCursor>
{
    public IHoverable CurrentHoveredObject { get; private set; }
    private Camera mainCam;
    [SerializeField] private LayerMask hoverLayer;
    private new void Awake()
    {
        base.Awake();
        mainCam = Camera.main;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && CurrentHoveredObject != null)
        {
            CurrentHoveredObject.OnClick();
        }
        
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0,hoverLayer);
        if (hit.collider != null && hit.collider.GetComponent<IHoverable>() != null)
        {
            IHoverable hoverable = hit.collider.GetComponent<IHoverable>();
            if (hoverable == CurrentHoveredObject) return;
            CurrentHoveredObject?.OnLeaveHover();
            hit.collider.GetComponent<IHoverable>().OnHover();
            CurrentHoveredObject = hoverable;
        }else if (CurrentHoveredObject != null)
        {
            CurrentHoveredObject.OnLeaveHover();
            CurrentHoveredObject = null;
        }
        
        
    }
    
}