using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AICard : MonoBehaviour
{
    public TextMeshProUGUI text;
    private RectTransform rect;
    public int value;

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
}
