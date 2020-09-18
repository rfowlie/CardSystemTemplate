using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameState : MonoBehaviour
{
    //hold global variables
    private GamePlayStatics gps;
    public State CurrentState = State.PLAYER;
    public bool isPlayerTurn = true;

    public Canvas canvas;
    public Transform playerHand;
    private HandController playerHandController;
    public Transform aiHand;
    private HandController aiHandController;
    public Transform pile;
    public TextMeshProUGUI pileText;
    public int handSize = 5;

    public int pileCount = 0;
    public int pileMax = 10;


    //DEBUG
    private void Update()
    {
        //test
        if (Input.GetKeyUp(KeyCode.Z))
        {
            playerHandController.AddCard(GetCard());
            playerHandController.Position();
        }
        
        if(CurrentState == State.AI)
        {
            CurrentState = State.WAIT;
            c = null;
            c = StartCoroutine(AIWait(aiWaitTime));
        }
    }

    IEnumerator AIWait(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);

        GameObject card = aiHandController.PlayCard(Random.Range(0, aiHandController.hand.Count));
        c = StartCoroutine(MoveToPile(card));
    }


    private void Start()
    {
        playerHandController = playerHand.GetComponent<HandController>();
        aiHandController = aiHand.GetComponent<HandController>();

        //deal cards
        for (int i = 0; i < handSize; i++)
        {
            playerHandController.AddCard(GetCard());
            aiHandController.AddCard(GetCard());
        }

        playerHandController.Position();
        aiHandController.Position();

        SetPile(0);
    }

    //track pile value
    private void SetPile(int amount)
    {
        pileCount += amount;
        if(pileCount > pileMax)
        {
            //MAKE A BUNCH OF SHIT HAPPEN HERERERERE
            //redraw cards for all players
            Debug.Log("Pile Maxed");
            pileCount = 0;
        }

        pileText.text = pileCount.ToString();
    }

    //OBJECT POOL FOR CARDS
    public GameObject cardPrefab;
    private List<GameObject> activeCards = new List<GameObject>();
    private Queue<GameObject> deactiveCards = new Queue<GameObject>();
    
    private GameObject GetCard()
    {
        if(deactiveCards.Count > 0)
        {
            GameObject o = deactiveCards.Dequeue();
            activeCards.Add(o);
            o.SetActive(true);
            return o;
        }

        GameObject newTemp = InstantiateCard();
        activeCards.Add(newTemp);
        return newTemp;
    }
    private void RemoveCard(GameObject card)
    {
        if (!activeCards.Contains(card))
        {
            Debug.LogError("Card to be removed not in pool");
            return;
        }

        deactiveCards.Enqueue(card);
        activeCards.Remove(card);
        card.SetActive(false);
    }

    public int cardCount = 0;
    private GameObject InstantiateCard()
    {
        GameObject temp = Instantiate(cardPrefab);
        //temp debug
        temp.name = cardCount++.ToString();
        return temp;
    }

    //check if card is InPlay or should go back to hand    
    private bool CheckInPlay(GameObject card)
    {
        float c = card.GetComponent<RectTransform>().anchoredPosition.y;
        float p = pile.GetComponent<RectTransform>().anchoredPosition.y;
        float ph = playerHand.GetComponent<RectTransform>().anchoredPosition.y;
        //Debug.Log("Distance: " + Mathf.Abs(p - ph) / 2f); 
        return c > Mathf.Abs(p - ph) / 2f;
    }

    private void OnEnable()
    {
        PlayerCard.MOVED += (GameObject card) =>
        {
            //Debug.Log("Card Moved");
            if(CurrentState == State.PLAYER && CheckInPlay(card))
            {
                //Debug.Log("Card in play");                

                //remove card from playerHand card list
                playerHandController.RemoveCard(card);
                playerHandController.Position();
                
                //call coroutine to move card to pile
                c = null;
                c = StartCoroutine(MoveToPile(card));
            }
            else
            {
                //Debug.Log("Reposition cards");
                playerHandController.Reposition(card);
            }
        };
    }

    public float aiWaitTime = 1f;
    public float returnSpeed = 0.5f;
    private Coroutine c = null;
    IEnumerator MoveToPile(GameObject card)
    {
        CurrentState = State.ANIMATION;

        //set card as child to pile
        card.transform.SetParent(pile);
        //set sibling index
        card.transform.SetAsLastSibling();
        //set rotation to normal
        card.transform.eulerAngles = Vector3.zero;

        //get positions
        Vector3 currentPos = card.transform.localPosition;        

        float count = 0f;
        while (count < returnSpeed)
        {
            //Zero because now child of parent
            card.transform.localPosition = Vector3.Lerp(currentPos, Vector3.zero, count / returnSpeed);
            count += Time.deltaTime;
            yield return null;
        }

        //when get to pile, set inactive increment pile???
        SetPile(card.GetComponent<PlayerCard>().value);
        RemoveCard(card);

        isPlayerTurn = !isPlayerTurn;
        CurrentState = isPlayerTurn ? State.PLAYER : State.AI;
    }
}
