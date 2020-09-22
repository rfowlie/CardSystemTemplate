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
        transform.localScale = originalScale;
    }

    //notify of card movement, player
    public static event Action<GameObject> MOVED;

    protected override void SetPosition(Vector3 pos) { originalPosition = pos; }
    protected override void SetRotation(Vector3 rot) { originalRotation = rot; }
    protected override void SetScale(float scale) { originalScale = Vector3.one * scale; }
    protected override void SetIndex(int index) { originalIndex = index;  transform.SetSiblingIndex(index); }

    public bool isBusy = false;

    public override void InitialSet(Vector3 pos, Vector3 rot, float scale, int index, bool isCardBack)
    {
        base.InitialSet(pos, rot, scale, index, isCardBack);

        //move the cards smoothly into position
        Debug.Log("SmoothSet");
        StartCoroutine(SmoothSet());
    }

    public float smoothMoveSpeed = 0.4f;
    //if card has new position move to that position smoothly
    IEnumerator SmoothSet()
    {
        Vector3 curPos = transform.localPosition;
        Quaternion curRot = Quaternion.Euler(transform.localEulerAngles);
        Quaternion nextRot = Quaternion.Euler(originalRotation);
        Vector3 curScale = transform.localScale;

        float count = 0f;
        while (count < smoothMoveSpeed)
        {
            //Debug.Log("Smooth Set Count: " + count);
            transform.localPosition = Vector3.Lerp(curPos, originalPosition, count / smoothMoveSpeed);
            transform.localRotation = Quaternion.Lerp(curRot, nextRot, count / smoothMoveSpeed);
            transform.localScale = Vector3.Lerp(curScale, originalScale, count / smoothMoveSpeed);

            count += Time.deltaTime;
            yield return null;
        }

        isBusy = false;
    }

    //UI INTERFACE
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isBusy)
        {
            highlight.SetActive(true);
            Vector3 temp = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
            //Debug.Log("Temp: " + temp);
            GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
            transform.localPosition = temp;
            transform.localEulerAngles = Vector3.zero;
            transform.localScale += inflateScale;
            transform.SetAsLastSibling();
        }        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isBusy)
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
        if(!isBusy)
        {
            //reset pivot
            GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            //adjust position so doesn't look weird...
            transform.localPosition += new Vector3(0f, GetComponent<RectTransform>().rect.height * 0.5f, 0f);

            canvasScaleFactor = GamePlayStatics.CANVASSCALEFACTOR;
            isBusy = true;
        }
    }

    private float canvasScaleFactor;
    public float wobble = 3f;
    public float wobbleClamp = 30f;
    public void OnDrag(PointerEventData eventData)
    {
        if(isBusy)
        {
            //move card perfectly along with mouse
            Vector2 move = eventData.delta / canvasScaleFactor;
            transform.localPosition += new Vector3(move.x, move.y, 0f);
            float x = Mathf.Clamp(eventData.delta.y, -wobbleClamp, wobbleClamp);
            float y = Mathf.Clamp(eventData.delta.x, -wobbleClamp, wobbleClamp);
            transform.localEulerAngles = new Vector3(x * wobble, -y * wobble, 0f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //also turn off highlights
        highlight.SetActive(false);
        MOVED(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }
}