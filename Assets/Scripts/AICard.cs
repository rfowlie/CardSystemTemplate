using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AICard : Card
{
    public TextMeshProUGUI text;    
    public int value;

    private void OnEnable()
    {
        //set values...
        value = UnityEngine.Random.Range(1, 4);
        text.text = value.ToString();
    }
}
