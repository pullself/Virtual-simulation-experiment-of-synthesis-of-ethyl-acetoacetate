using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QRCode : MonoBehaviour,IBeginDragHandler,IDragHandler,IPointerDownHandler
{
    //--------------------------------------------------------------
    //drag
    private RectTransform parentRTF;
    private Vector3 offset;

    void Start()
    {
        parentRTF = GetComponent<RectTransform>();
        defaultScale = parentRTF.localScale;
        defaultPos = parentRTF.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            parentRTF,
            eventData.position,
            eventData.pressEventCamera,
            out worldPos);
        offset = transform.position - worldPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            parentRTF,
            eventData.position,
            eventData.pressEventCamera,
            out worldPos);
        transform.position = worldPos + offset;
    }

    //--------------------------------------------------------------
    //double click
    private float doubleClickDelay = 0.3f;
    private float lastClickTime = -1;
    string url = "http://182.254.187.40:8080/home.html";

    void OnDoubleClick()
    {
        Debug.Log(url);
        //Application.OpenURL(url);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Time.time - lastClickTime <= doubleClickDelay)
            OnDoubleClick();
        lastClickTime = Time.time;
    }

    //--------------------------------------------------------------
    //scale
    private Vector3 defaultScale;
    private Vector3 defaultPos;
    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            parentRTF.localScale = defaultScale;
            parentRTF.position = defaultPos;
        }
        parentRTF.localScale *= 1 + Input.GetAxis("Mouse ScrollWheel");
    }
}
