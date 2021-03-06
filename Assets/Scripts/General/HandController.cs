﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//JOB: TO KEEP TRACK AND ORGANIZE THE VISUAL ASPECT OF A HAND OF CARDS
public class HandController : MonoBehaviour
{
    public float spacingX = 10f;
    public float spacingY = -1f;
    public float rotation = 3f;
    public float scaling = 1f;
    public int maxCards = 7;

    public bool cardBack = false;

    //set number of cards into card list
    public List<GameObject> hand = new List<GameObject>();

    //position the cards into the correct positions based on number and spacing
    private void Position()
    {
        if(hand.Count < 1) { return; }
        if(hand.Count == 1)
        {
            hand[0].GetComponent<Abstract_Card>().InitialSet(Vector3.zero, Vector3.zero, scaling, 0, cardBack);
            return;
        }

        int numberOfCards = hand.Count;
        float middleCard = (numberOfCards - 1) / 2f;
        float width = numberOfCards * spacingX;
        float widthSpacing = numberOfCards == 1 ? 0 : width / (numberOfCards - 1);
        float widthStart = (width / 2f) - width;
        

        for (int i = 0; i < hand.Count; i++)
        {
            //set transform
            Vector3 pos = new Vector3(widthStart + widthSpacing * i, Mathf.Abs(i - middleCard) * spacingY, 0f);
            Vector3 rot = new Vector3(0f, 0f, (middleCard - i) * rotation);
            hand[i].GetComponent<Abstract_Card>().InitialSet(pos, rot, scaling, i, cardBack);
        }
    }

    //UNFORTUNATELY PUBLIC AS GAMEMANAGER HAS TO CHECK IF PLAYER TURN...
    //adjust the order of the cards when one is moved in hand
    public void Reposition(GameObject card)
    {
        //Debug.Log("Reposition");
        if(hand.Contains(card) == false) { return; }
        hand.Remove(card);

        //go through hand and insert card at appropriate spot for x axis
        for (int i = 0; i < hand.Count; i++)
        {
            if(hand[i].transform.localPosition.x > card.transform.localPosition.x)
            {
                hand.Insert(i, card);
                Position();
                return;
            }
        }

        //goes at the end
        hand.Add(card);
        Position();
    }    

    //add one
    public void AddCard(GameObject card)
    {
        hand.Add(card);
        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;
        card.transform.localEulerAngles = Vector3.zero;
        card.transform.localScale = Vector3.one;
        card.transform.SetAsLastSibling();
        Position();
    }

    //add multiple
    public void AddCards(GameObject[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            hand.Add(cards[i]);
            cards[i].transform.SetParent(transform);
            cards[i].transform.localPosition = Vector3.zero;
            cards[i].transform.localEulerAngles = Vector3.zero;
            cards[i].transform.localScale = Vector3.one * scaling;
            cards[i].transform.SetAsLastSibling();
        }

        Position();
    }
    
    //remove card from hand
    public void RemoveCard(GameObject card)
    {
        hand.Remove(card);
        Position();
    }

    //remove card based on index
    public void RemoveCardAtIndex(int index = 0)
    {
        RemoveCard(hand[index]);
    }

    //get card at hand index
    public GameObject GetCard(int index)
    {
        GameObject card = hand[index];
        hand.RemoveAt(index);
        Position();
        return card;
    }
}
