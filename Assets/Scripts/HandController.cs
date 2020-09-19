using System.Collections;
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

    //set number of cards into card list
    public List<GameObject> hand = new List<GameObject>();

    //position the cards into the correct positions based on number and spacing
    public void Position()
    {
        if(hand.Count < 1) { return; }
        if(hand.Count == 1)
        {
            hand[0].GetComponent<PlayerCard>().SetTransform(Vector3.zero, Vector3.zero, scaling, 0);
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
            hand[i].GetComponent<PlayerCard>().SetTransform(pos, rot, scaling, i);
        }
    }

    //adjust the order of the cards when one is moved in hand
    //SHOULD SET IT UP SO IT DOESN'T NEED THE CARD, JUST CHECK CURRENT CARDS
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

    
    public void AddCard(GameObject card)
    {        
        hand.Add(card);
        card.transform.SetParent(transform);
        card.transform.SetAsLastSibling();
    }

    public void RemoveCard(GameObject card)
    {
        hand.Remove(card);
    }

    public void RemoveCardAtIndex(int index = 0)
    {
        RemoveCard(hand[index]);
    }


    //Get ref to card at index, remove from hand and return to caller
  
    public GameObject PlayCard(int index)
    {
        GameObject card = hand[index];
        hand.RemoveAt(index);
        Position();
        return card;
    }
}
