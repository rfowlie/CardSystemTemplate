using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Abstract_Card : MonoBehaviour
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
    public virtual void InitialSet(Vector3 pos, Vector3 rot, float scale, int index, bool isCardBack)
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