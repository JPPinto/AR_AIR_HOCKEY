using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/**
 * 
 * STARTING = Waiting for field to be detected, Starting point if field is undetected mid game
 * WAITING_PLAYERS = Field has been detected and is waiting for players to be detected
 * PLAYING = Three objects detected and game is running
 * PAUSED = One or both of players is undetected, game is paused until otherwise
 * ENDED = Winning condition has been reached
 * 
 */
enum States : int { STARTING, WAITING_PLAYERS, PLAYING, PAUSED, ENDED };

public class Manager : MonoBehaviour
{
    private States stateMachine = States.STARTING;
    public Text displayedText = null;
    public int playersFoundCounter = 0;

    // Use this for initialization
    void Start()
    {
        displayedText = GetComponent<Text>();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
        UpdateText();
    }

    States getCurrentState()
    {
        return stateMachine;
    }

    //TODO Missing End condition handling
    void UpdateState()
    {
        //Incremented on every call
        playersFoundCounter = 0;

        GameObject field = GameObject.FindGameObjectWithTag("Field");
        if (stateMachine.Equals(States.PLAYING))
        {
            print("PLAYING");
        }
        if (field)
        {
            if (field.GetComponent<MeshRenderer>().isVisible)
            {
                GameObject playerOne = GameObject.FindGameObjectWithTag("PlayerOne");
                GameObject playerTwo = GameObject.FindGameObjectWithTag("PlayerTwo");

                if (playerOne.GetComponent<MeshRenderer>().enabled)
                {
                    playersFoundCounter++;
                }

                if (playerTwo.GetComponent<MeshRenderer>().enabled)
                {
                    playersFoundCounter++;
                }

                if (stateMachine.Equals(States.PLAYING) && playersFoundCounter < 2)
                {
                    stateMachine = States.PAUSED;
                    return;
                }

                if (playersFoundCounter == 2)
                {
                    stateMachine = States.PLAYING;
                }
                else
                {
                    if (!stateMachine.Equals(States.PAUSED))
                        stateMachine = States.WAITING_PLAYERS;
                }
            }
            else { stateMachine = States.STARTING; }
        }

    }

    void UpdateText()
    {

        switch (stateMachine)
        {

            case States.STARTING:
                displayedText.text = "Waiting for field...";
                break;
            case States.WAITING_PLAYERS:
                displayedText.text = "Waiting for " + (2 - playersFoundCounter) + " players...";
                break;
            case States.PLAYING:
                displayedText.text = "GO!";
                break;
            case States.PAUSED:
                displayedText.text = "PAUSE\n " + (2 - playersFoundCounter) + " undetected";
                break;
            case States.ENDED:
                displayedText.text = "Waiting for FIELD...";
                break;
        }
    }
}
