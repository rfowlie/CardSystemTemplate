using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//the base abstract card for the 10 point game
//needs a text field as well as an int to store the card value, as well as initialize it on load
public abstract class Abstract_Card_10Point : Abstract_Card
{
    public TextMeshProUGUI text;
    public int value;

    protected virtual void OnEnable()
    {
        //set values...
        value = UnityEngine.Random.Range(1, 4);
        text.text = value.ToString();
    }
}
