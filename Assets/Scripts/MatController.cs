using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatController : MonoBehaviour
{
    public float spacingX = 10f;
    public float spacingY = -1f;
    public float rotation = 3f;
    public float scaling = 1f;

    public bool cardBack = false;

    //set number of cards into card list
    public List<GameObject> tokens = new List<GameObject>();

    //move all cards gradually when placing a new card???
    

    public GameObject debugObject = null;
    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            tokens.Add(Instantiate(debugObject, transform));
        }

        Position();
    }

    //position the cards into the correct positions based on number and spacing
    private void Position()
    {
        if (tokens.Count < 1) { return; }
        if (tokens.Count == 1)
        {
            tokens[0].GetComponent<Abstract_Card>().SetCard(Vector3.zero, Vector3.zero, scaling, 0, cardBack);
            return;
        }

        int numberOfCards = tokens.Count;
        float middleCard = (numberOfCards - 1) / 2f;
        float width = numberOfCards * spacingX;
        float widthSpacing = numberOfCards == 1 ? 0 : width / (numberOfCards - 1);
        float widthStart = (width / 2f) - width;


        for (int i = 0; i < tokens.Count; i++)
        {
            //set transform
            Vector3 pos = new Vector3(widthStart + widthSpacing * i, Mathf.Abs(i - middleCard) * spacingY, 0f);
            Vector3 rot = new Vector3(rotation, 0f, 0f);
            tokens[i].transform.localPosition = pos;
            tokens[i].transform.localEulerAngles = rot;
            //tokens[i].GetComponent<Abstract_Card>().SetCard(pos, rot, scaling, i, cardBack);
        }
    }

    //UNFORTUNATELY PUBLIC AS GAMEMANAGER HAS TO CHECK IF PLAYER TURN...
    //adjust the order of the cards when one is moved in hand
    public void Reposition(GameObject card)
    {
        //Debug.Log("Reposition");
        if (tokens.Contains(card) == false) { return; }
        tokens.Remove(card);

        //go through hand and insert card at appropriate spot for x axis
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i].transform.localPosition.x > card.transform.localPosition.x)
            {
                tokens.Insert(i, card);
                Position();
                return;
            }
        }

        //goes at the end
        tokens.Add(card);
        Position();
    }

    //add one
    public void AddCard(GameObject card)
    {
        tokens.Add(card);
        card.transform.SetParent(transform);
        card.transform.SetAsLastSibling();
        Position();
    }

    //add multiple
    public void AddCards(GameObject[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            tokens.Add(cards[i]);
            cards[i].transform.SetParent(transform);
            cards[i].transform.SetAsLastSibling();
        }

        Position();
    }

    //remove card from hand
    public void RemoveCard(GameObject card)
    {
        tokens.Remove(card);
        Position();
    }

    //remove card based on index
    public void RemoveCardAtIndex(int index = 0)
    {
        RemoveCard(tokens[index]);
    }

    //get card at hand index
    public GameObject GetCard(int index)
    {
        GameObject card = tokens[index];
        tokens.RemoveAt(index);
        Position();
        return card;
    }
}
