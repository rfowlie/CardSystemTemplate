using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public int pointsMax = 3;
    public int handSizeMax = 5;

    public int pileCount = 0;
    public int pileMax = 10;

    public TextMeshProUGUI playerPointsText;
    private int playerPoints = 0;
    public TextMeshProUGUI aiPointsText;
    private int aiPoints = 0;

    public Transform gameOverUI;


    public void ResetGame()
    {
        //remove game over screen
        gameOverUI.gameObject.SetActive(false);
        
        //remove all cards from active pool
        for (int i = 0; i < activeCards.Count; i++)
        {
            deactiveCards.Enqueue(activeCards[i]);
            activeCards[i].SetActive(false);
        }

        activeCards.Clear();

        //clear hands
        playerHandController.hand.Clear();
        aiHandController.hand.Clear();  

        //deal cards
        for (int i = 0; i < handSizeMax; i++)
        {
            playerHandController.AddCard(GetCard());
            aiHandController.AddCard(GetCard());
        }

        playerHandController.Position();
        aiHandController.Position();

        //set pile and points text
        pileCount = 0;
        pileText.text = pileCount.ToString();
        playerPoints = 0;
        aiPoints = 0;
        playerPointsText.text = playerPoints.ToString();
        aiPointsText.text = aiPoints.ToString();
        isPlayerTurn = true;
        CurrentState = State.PLAYER;
    }

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
        //Cursor.lockState = CursorLockMode.Confined; // keep confined in the game window

        playerHandController = playerHand.GetComponent<HandController>();
        aiHandController = aiHand.GetComponent<HandController>();

        ResetGame();
    }

    //track pile value
    private void SetPile(int amount)
    {
        pileCount += amount;
        if(pileCount >= pileMax)
        {
            //MAKE A BUNCH OF SHIT HAPPEN HERERERERE
            //redraw cards for all players
            Debug.Log("Pile Maxed");

            if(!isPlayerTurn)
            {
                playerPoints++;
                playerPointsText.text = playerPoints.ToString();
            }
            else
            {
                aiPoints++;
                aiPointsText.text = aiPoints.ToString();                
            }

            //check if game over
            if(playerPoints >= pointsMax || aiPoints >= pointsMax) 
            {
                gameOverUI.gameObject.SetActive(true);
                string winner = playerPoints >= pointsMax ? "Player" : "AI";
                gameOverUI.GetComponentInChildren<TextMeshProUGUI>().text = "Game Over \n" + winner + " Wins";
                CurrentState = State.GAMEOVER;
                return;
            }

            pileCount = 0;
            //refill hands
            for (int i = 0; i <= handSizeMax - (playerHandController.hand.Count - 1); i++)
            {
                playerHandController.AddCard(GetCard());
                playerHandController.Position();
            }
            for (int i = 0; i <= handSizeMax - (aiHandController.hand.Count - 1); i++)
            {
                aiHandController.AddCard(GetCard());
                aiHandController.Position();
            }
        }

        pileText.text = pileCount.ToString();
        isPlayerTurn = !isPlayerTurn;
        CurrentState = isPlayerTurn ? State.PLAYER : State.AI;
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
        RemoveCard(card);
        SetPile(card.GetComponent<PlayerCard>().value);
    }
}
