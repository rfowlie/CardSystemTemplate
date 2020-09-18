using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayStatics : MonoBehaviour
{
    public static GamePlayStatics Instance { get; private set; }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CURRENTSTATE = State.PLAYER;
        CANVASSCALEFACTOR = Canvas.scaleFactor;
    }

    //references
    public Canvas Canvas;

   
    //variables
    public static State CURRENTSTATE { get; private set; }
    public static float CANVASSCALEFACTOR { get; private set; }
    
}

public enum State { NONE, PLAYER, AI, ANIMATION, WAIT }
