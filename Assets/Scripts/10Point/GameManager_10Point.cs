using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum State { NONE, PLAYER, AI, ANIMATION, WAIT, GAMEOVER, MAINMENU }


public class GameManager_10Point : MonoBehaviour
{
    //hold global variables
    private GamePlayStatics gps;
    [Header("Game State")]
    public State CurrentState = State.PLAYER;
    public bool isPlayerTurn = true;
    [Space]
    [Header("Game")]
    public Canvas canvas;
    public Transform pile;
    public TextMeshProUGUI pileText;
    public int pointsMax = 3;
    public void ChangePointsMax() => pointsMax = int.Parse(inputField.text);
    public TMP_InputField inputField;
    public int handSizeMax = 5;
    private int pileCount = 0;
    public int pileMax = 10;
    public float moveToPileSpeed = 0.5f;
    public Transform gameOverUI;
    public Transform mainMenuUI;
    [Space]
    [Header("Player")]
    public Transform playerHand;
    private HandController playerHandController;
    public TextMeshProUGUI playerPointsText;
    private int playerPoints = 0;
    public ObjectPool playerCardPool;
    [Space]
    [Header("AI")]
    public Transform aiHand;
    private HandController aiHandController;
    public TextMeshProUGUI aiPointsText;
    private int aiPoints = 0;
    public ObjectPool aiCardPool;
    public float aiWaitTime = 0.6f;


    private void OnEnable()
    {
        PlayerCard_10Point.MOVED += (GameObject card) =>
        {
            //must be player turn and card must be in play
            if (CurrentState == State.PLAYER && CheckInPlay(card))
            {
                //remove card from playerHand card list
                playerHandController.RemoveCard(card);

                //call coroutine to move card to pile
                c = null;
                c = StartCoroutine(MoveToPile(card));
            }
            else
            {
                //not turn or not in play just reposition that card
                playerHandController.Reposition(card);
            }
        };
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined; // keep confined in the game window

        playerHandController = playerHand.GetComponent<HandController>();
        aiHandController = aiHand.GetComponent<HandController>();

        CurrentState = State.MAINMENU;
    }
    private void Update()
    {
        //AI for now
        if (CurrentState == State.AI)
        {
            CurrentState = State.WAIT;
            c = null;
            c = StartCoroutine(AITurn(aiWaitTime));
        }
    }

    public void ReturnToMainMenu()
    {
        gameOverUI.gameObject.SetActive(false);
        mainMenuUI.gameObject.SetActive(true);
    }

    public void ResetGame()
    {
        //update rounds
        ChangePointsMax();
        //remove main menu screen
        mainMenuUI.gameObject.SetActive(false);
        //remove game over screen
        gameOverUI.gameObject.SetActive(false);

        playerCardPool.ReturnAll();
        aiCardPool.ReturnAll();

        //clear hands
        playerHandController.hand.Clear();
        aiHandController.hand.Clear();

        GameObject[] p = new GameObject[handSizeMax];
        GameObject[] a = new GameObject[handSizeMax];
        //deal cards
        for (int i = 0; i < handSizeMax; i++)
        {
            p[i] = playerCardPool.Get();
            a[i] = aiCardPool.Get();
        }

        playerHandController.AddCards(p);
        aiHandController.AddCards(a);

        //set pile and points text
        pileCount = 0;
        pileText.text = pileCount.ToString();
        playerPoints = 0;
        playerPointsText.text = playerPoints.ToString();
        aiPoints = 0;
        aiPointsText.text = aiPoints.ToString();

        isPlayerTurn = true;
        CurrentState = State.PLAYER;
    }  
   
        
    //AI action
    IEnumerator AITurn(float waitTime)
    {
        //pause before playing card to appear thinking
        yield return new WaitForSecondsRealtime(waitTime);

        //different levels of difficulty
        //easy - determine if pile + one of ai cards + one of player cards = 9
        //random card selection
        GameObject card = aiHandController.GetCard(Random.Range(0, aiHandController.hand.Count));

        c = null;
        c = StartCoroutine(MoveToPile(card));
    }
    
    //update the piles value as well as points values
    private void UpdatePile(int amount)
    {
        pileCount += amount;
        if (pileCount >= pileMax)
        {
            pileCount = 0;
            RefillHands();

            if (!isPlayerTurn)
            {
                playerPoints++;
                playerPointsText.text = playerPoints.ToString();
            }
            else
            {
                aiPoints++;
                aiPointsText.text = aiPoints.ToString();
            }
        }

        pileText.text = pileCount.ToString();
    }

    //check whether game over or move to next state
    private void UpdateGameState()
    {
        if (playerPoints >= pointsMax || aiPoints >= pointsMax)
        {
            gameOverUI.gameObject.SetActive(true);
            string winner = playerPoints >= pointsMax ? "Player" : "AI";
            gameOverUI.GetComponentInChildren<TextMeshProUGUI>().text = "Game Over \n" + winner + " Wins";
            CurrentState = State.GAMEOVER;
        }
        else
        {
            isPlayerTurn = !isPlayerTurn;
            CurrentState = isPlayerTurn ? State.PLAYER : State.AI;
        }
    }

    //add cards to hands till at max size
    private void RefillHands()
    {
        List<GameObject> temp = new List<GameObject>();
        //refill hands
        for (int i = 0; i < handSizeMax - (playerHandController.hand.Count); i++)
        {
            temp.Add(playerCardPool.Get());
        }
        playerHandController.AddCards(temp.ToArray());
        temp.Clear();
        for (int i = 0; i < handSizeMax - (aiHandController.hand.Count); i++)
        {
            temp.Add(aiCardPool.Get());
        }
        aiHandController.AddCards(temp.ToArray());
        temp.Clear();
    }
    
    //check if card is InPlay or should go back to hand    
    private bool CheckInPlay(GameObject card)
    {
        float cardToPile = pile.position.y - card.transform.position.y;
        float handToPile = pile.position.y - playerHand.position.y;
        //Debug.Log("Card to Pile: " + cardToPile);
        //Debug.Log("Hand to Pile: " + handToPile);
        return cardToPile < handToPile / 2f;
    }


    //AWKWARD RIGHT NOW AS JUST FIGURING IT OUT    
    private Coroutine c = null;
    IEnumerator MoveToPile(GameObject card)
    {
        CurrentState = State.ANIMATION;

        //SHOULD HAVE THIS GIVE CARD TO PILE AND PILE CALL SMOOTH MOVE ON CARD!!
        //NO, KEEP HERE TO CONTROL GAME FLOW...


        //set card as child to pile
        card.transform.SetParent(pile);
        //set sibling to last so card goes overtop of pile
        card.transform.SetAsLastSibling();

        //get positions
        Vector3 currentPos = card.transform.localPosition;
        //get rotation
        Quaternion currentRot = card.transform.rotation;

        //flip if backwards
        if (card.GetComponent<Abstract_Card>().isCardBack)
        {
            card.GetComponent<Abstract_Card>().FlipCard();
        }

        float count = 0f;
        while(count < moveToPileSpeed)
        {
            card.transform.localPosition = Vector3.Lerp(currentPos, Vector3.zero, count / moveToPileSpeed);
            card.transform.rotation = Quaternion.Lerp(currentRot, Quaternion.identity, count / moveToPileSpeed);
            count += Time.deltaTime;
            yield return null;
        }

        //when get to pile, set inactive increment pile???
        if (isPlayerTurn) { playerCardPool.Return(card); }
        else { aiCardPool.Return(card); }        
        
        //awkward...
        UpdatePile(card.GetComponent<Abstract_Card_10Point>().value);
        UpdateGameState();
    }
}
