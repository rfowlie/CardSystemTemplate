using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//GM for new game, just change name after
public class Abstract_GameManager : MonoBehaviour
{
    [Header("Game State")]
    public State CurrentState = State.PLAYER;
    public bool isPlayerTurn = true;
    [Space]
    [Header("Game")]
    public MatController playerMat;
    public MatController aiMat;

    private void OnEnable()
    {
        //read card movement
        PlayerCard_10Point.MOVED += (GameObject card) =>
        {
            if(CurrentState == State.PLAYER)
            {

            }
        };
    }

}
