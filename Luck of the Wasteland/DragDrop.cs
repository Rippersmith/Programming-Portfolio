using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Canvas canvas;
    //public bool isEmpty = true;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 returnToPoint;

    //protected VendorMenusScript vendorMenusScript;

    //bool properDrop = false;

    [SerializeField] Transform parentObj;

    private void Awake()
    {
        canvas = GameObject.Find("CardHandCanvas").GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        //vendorMenusScript = canvas.GetComponent<VendorMenusScript>();

        parentObj = canvas.transform.Find("Hand");
        returnToPoint = GetComponent<RectTransform>().anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //if (isEmpty) return;

        print("START DRAG");
        //canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(parentObj.root);
        //transform.parent = canvas.transform;
    }

    public void OnDrag(PointerEventData eventData)
    {
        print("DRAGGING...");
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //transform.parent = canvas.transform;
        transform.SetParent(canvas.transform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print("END DRAG");
        //canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        transform.SetParent(parentObj);
        GetComponent<RectTransform>().anchoredPosition = returnToPoint;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("POINTER CLICK");
        OnEndDrag(eventData);
    }

}
