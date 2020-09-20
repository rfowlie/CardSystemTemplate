using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class Card : MonoBehaviour
{
    public GameObject cardFront;
    public GameObject cardBack;

    public bool isCardBack = false;
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

    //hands need to be able to set all these aspects of the cards position
    public void SetCard(Vector3 pos, Vector3 rot, float scale, int index, bool isCardBack)
    {
        SetPosition(pos);
        SetRotation(rot);
        SetScale(scale);
        SetIndex(index);
        SetCardBack(isCardBack);
    }
    //abstract extra needs in here
    protected virtual void SetPosition(Vector3 pos) { transform.localPosition = pos; }
    protected virtual void SetRotation(Vector3 rot) { transform.localEulerAngles = rot; }
    protected virtual void SetScale(float scale) { transform.localScale = Vector3.one * scale; }
    protected virtual void SetIndex(int index) { transform.SetSiblingIndex(index); }
}

public class PlayerCard : Card,
                    IPointerEnterHandler,
                    IPointerExitHandler,
                    IBeginDragHandler,
                    IDragHandler,
                    IEndDragHandler,
                    IPointerDownHandler
{   
    public GameObject highlight;
    public TextMeshProUGUI text;
    public int value;

    public Vector3 originalPosition;
    public Vector3 inflatePosition;
    public Vector3 originalRotation;
    public Vector3 originalScale;
    public Vector3 inflateScale;
    public int originalIndex;

    //notify of card movement, player
    public static event Action<GameObject> MOVED;
   
    private void OnEnable()
    {        
        //set values...
        value = UnityEngine.Random.Range(1, 4);
        text.text = value.ToString();
    }

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
            transform.localScale += inflateScale;
            transform.localPosition += inflatePosition;
            transform.SetAsLastSibling();
        }        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isDrag)
        {
            highlight.SetActive(false);
            transform.localPosition = originalPosition;
            transform.localScale = originalScale;
            transform.SetSiblingIndex(originalIndex);
        }        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.eulerAngles = Vector3.zero;
        canvasScaleFactor = GamePlayStatics.CANVASSCALEFACTOR;
        Debug.Log("CSF: " + canvasScaleFactor);
        isDrag = true;
    }

    private bool isDrag = false;
    private float canvasScaleFactor;
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 move = eventData.delta / canvasScaleFactor;
        transform.localPosition += new Vector3(move.x, move.y, 0f);
        //GetComponent<RectTransform>().anchoredPosition += eventData.delta / canvasScaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //also turn off highlights
        highlight.SetActive(false);
        transform.localScale = originalScale;
        transform.localEulerAngles = originalRotation;
        MOVED(gameObject);

        isDrag = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }
}
