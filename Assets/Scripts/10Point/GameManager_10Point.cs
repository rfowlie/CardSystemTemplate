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
    public Transform show;
    public TextMeshProUGUI pileText;
    public int pointsMax = 3;
    public void ChangePointsMax() => pointsMax = int.Parse(inputField.text);
    public TMP_InputField inputField;
    public int handSizeMax = 5;
    private int pileCount = 0;
    public int pileMax = 10;
    public float pileSpeed = 0.5f;
    public float showSpeed = 1f;
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
                c = StartCoroutine(CardAnimation(card));
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

        //exit
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            //return to main menu
            CurrentState = State.NONE;
            mainMenuUI.gameObject.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
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
        c = StartCoroutine(CardAnimation(card));
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

    public float animationInflate = 1.5f;
    
    private Coroutine c = null;
    IEnumerator CardAnimation(GameObject card)
    {
        //set state so nothing else will fire
        CurrentState = State.ANIMATION;

        //FIRST SEND TO SHOW POSITION THEN TO PILE

        //flip if backwards
        if (card.GetComponent<Abstract_Card>().isCardBack)
        {
            card.GetComponent<Abstract_Card>().FlipCard();
        }
        //set card as child to show
        card.transform.SetParent(show);
        //set sibling to last so card is on top
        card.transform.SetAsLastSibling();

        //get position in relation to new parent
        Vector3 currentPos = card.transform.localPosition;
        //get rotation
        Quaternion currentRot = card.transform.rotation;
        //get scale so we can adjust it
        Vector3 currentScale = card.transform.localScale;
        Vector3 nextScale = currentScale * animationInflate;

        float count = 0f;
        float add = 0f;
        Vector3 handDistance = show.transform.InverseTransformPoint(playerHand.position);
        Debug.Log("hand distance: " + handDistance + " card distance: " + currentPos);
        float calcSpeed = (currentPos.magnitude / handDistance.magnitude) * showSpeed;
        Debug.Log("Calc: " + calcSpeed);
        while(count < calcSpeed)
        {
            card.transform.localPosition = Vector3.Lerp(currentPos, Vector3.zero, count / calcSpeed);
            card.transform.rotation = Quaternion.Lerp(currentRot, Quaternion.identity, count / calcSpeed);
            card.transform.localScale = Vector3.Lerp(currentScale, nextScale, count / calcSpeed);
            count += Time.deltaTime;
            yield return null;
        }

        //set card as child to pile
        card.transform.SetParent(pile);
        //set sibling to last so card goes overtop of pile
        card.transform.SetAsLastSibling();
        //get positions
        currentPos = card.transform.localPosition;
        //get scale so we can adjust it
        currentScale = card.transform.localScale;
        nextScale = Vector3.one;

        count = 0f;
        while(count < pileSpeed)
        {
            card.transform.localPosition = Vector3.Lerp(currentPos, Vector3.zero, count / pileSpeed);
            //card.transform.rotation = Quaternion.Lerp(currentRot, Quaternion.identity, count / moveToPileSpeed);
            card.transform.localScale = Vector3.Lerp(currentScale, nextScale, count / pileSpeed);
            count += Time.deltaTime;
            add += Time.deltaTime;
            count += add;
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
