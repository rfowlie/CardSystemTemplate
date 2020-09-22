using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;


public class PlayerCard_10Point : Abstract_Card_10Point,
                    IPointerEnterHandler,
                    IPointerExitHandler,
                    IBeginDragHandler,
                    IDragHandler,
                    IEndDragHandler,
                    IPointerDownHandler
{   
    public GameObject highlight;
    public Vector3 originalPosition;
    public Vector3 inflatePosition;
    public Vector3 originalRotation;
    public Vector3 originalScale;
    public Vector3 inflateScale;
    public int originalIndex;

    private void OnDisable()
    {
        isDrag = false;
        transform.localScale = originalScale;
    }

    //notify of card movement, player
    public static event Action<GameObject> MOVED;

    protected override void SetPosition(Vector3 pos) { originalPosition = pos; transform.localPosition = originalPosition; }
    protected override void SetRotation(Vector3 rot) { originalRotation = rot; transform.localEulerAngles = originalRotation; }
    protected override void SetScale(float scale) { originalScale = Vector3.one * scale; transform.localScale = originalScale; }
    protected override void SetIndex(int index) { originalIndex = index;  transform.SetSiblingIndex(index); }  

    //UI INTERFACE
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isDrag)
        {
            highlight.SetActive(true);
            Vector3 temp = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
            Debug.Log("Temp: " + temp);
            GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
            transform.localPosition = temp;
            transform.localEulerAngles = Vector3.zero;
            transform.localScale += inflateScale;
            transform.SetAsLastSibling();
        }        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isDrag)
        {
            highlight.SetActive(false);
            GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            transform.localPosition = originalPosition;
            transform.localEulerAngles = originalRotation;
            transform.localScale = originalScale;
            transform.SetSiblingIndex(originalIndex);
        }        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!isDrag)
        {
            //reset pivot
            GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            //adjust position so doesn't look weird...
            transform.localPosition += new Vector3(0f, GetComponent<RectTransform>().rect.height * 0.5f, 0f);

            canvasScaleFactor = GamePlayStatics.CANVASSCALEFACTOR;
            isDrag = true;
        }
    }

    private bool isDrag = false;
    private float canvasScaleFactor;
    public void OnDrag(PointerEventData eventData)
    {
        if(isDrag)
        {
            Vector2 move = eventData.delta / canvasScaleFactor;
            transform.localPosition += new Vector3(move.x, move.y, 0f);
            //GetComponent<RectTransform>().anchoredPosition += eventData.delta / canvasScaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //also turn off highlights
        highlight.SetActive(false);
        transform.localScale = originalScale;
        //transform.localEulerAngles = originalRotation;
        MOVED(gameObject);

        //isDrag = false;
        StartCoroutine(CardReset());
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    //gradually return to normal card shape
    public IEnumerator CardReset()
    {
        Vector3 currentScale = transform.localScale;
        float count = 0f;
        while(count < 0.5f)
        {
            transform.localScale = Vector3.Lerp(currentScale, originalScale, count / 0.5f);
            count += Time.deltaTime;
            yield return null;
        }

        isDrag = false;
    }
}