﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;


public class PlayerCard : MonoBehaviour,
                    IPointerEnterHandler,
                    IPointerExitHandler,
                    IBeginDragHandler,
                    IDragHandler,
                    IEndDragHandler,
                    IPointerDownHandler
{
    //when the mouse mouses over a card it should turn its highlight on and off
    public GameObject cardFront;
    public GameObject cardBack;
    public GameObject highlight;
    public TextMeshProUGUI text;
    public RectTransform rect;
    public int value;

    public Vector3 originalPosition;
    public Vector2 inflatePosition;
    public Vector3 originalRotation;
    public Vector3 originalScale;
    [Tooltip("How much scaleS increase you want to ADD to normal scale")]
    public Vector3 inflateScale;
    public int originalIndex;

    public bool isCardBack = false;

    //notify of card movement
    public static event Action<GameObject> MOVED;

    
    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        //set values...
        value = UnityEngine.Random.Range(1, 4);
        text.text = value.ToString();
    }

    public void SetPosition(Vector3 pos) { originalPosition = pos; transform.localPosition = originalPosition; }
    public void SetRotation(Vector3 rot) { Debug.Log("Rot: " + rot); originalRotation = rot;  Debug.Log("Orig: " + originalRotation); transform.localEulerAngles = originalRotation; }
    public void SetScale(float scale) { originalScale = Vector3.one * scale; transform.localScale = originalScale; }
    public void SetIndex(int index) { originalIndex = index;  transform.SetSiblingIndex(index); }
    public void SetCardBack(bool b) 
    { 
        isCardBack = b;
        if (b) { cardBack.transform.SetAsLastSibling(); }
        else { cardBack.transform.SetAsFirstSibling(); }
    }

    public void FlipCard()
    {
        SetCardBack(!isCardBack);
    }

    public void SetCard(Vector3 pos, Vector3 rot, float scale, int index, bool isCardBack)
    {
        SetPosition(pos);
        SetRotation(rot);
        SetScale(scale);
        SetIndex(index);
        SetCardBack(isCardBack);
    }

    
    

    //UI INTERFACE
    public void OnPointerEnter(PointerEventData eventData)
    {
        highlight.SetActive(true);
        //increase size, maybe adjust position
        transform.localScale += inflateScale;
        rect.anchoredPosition += inflatePosition;
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlight.SetActive(false);
        transform.localPosition = originalPosition;
        transform.localScale = originalScale;
        transform.SetSiblingIndex(originalIndex);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        rect.eulerAngles = Vector3.zero;
        canvasScaleFactor = GamePlayStatics.CANVASSCALEFACTOR;
        Debug.Log("CSF: " + canvasScaleFactor);
    }

    private float canvasScaleFactor;
    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvasScaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //also turn off highlights
        highlight.SetActive(false);
        transform.localScale = originalScale;
        rect.eulerAngles = originalRotation;
        MOVED(gameObject);

        //c = null;
        //c = StartCoroutine(ReturnToOriginal());
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        rect.SetAsLastSibling();
    }
}
