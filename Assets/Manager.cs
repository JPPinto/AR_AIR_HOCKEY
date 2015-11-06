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
public enum State : int { STARTING, WAITING_PLAYERS, PLAYING, PAUSED, ENDED };

public class Manager : MonoBehaviour
{
    private State stateMachine = State.STARTING;
    public Text displayedText = null;
    public int playersFoundCounter = 0;
    public int playerOneCounter = 0;
    public int playerTwoCounter = 0;

    // Use this for initialization
    void Start()
    {
        playerOneCounter = 0;
        playerTwoCounter = 0;
        displayedText = GetComponent<Text>();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
        UpdateText();
    }

    public State getCurrentState()
    {
        return stateMachine;
    }

    //TODO Missing End condition handling
    void UpdateState()
    {
        //Incremented on every call
        playersFoundCounter = 0;

        GameObject field = GameObject.FindGameObjectWithTag("Field");
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

                if (stateMachine.Equals(State.PLAYING) && playersFoundCounter < 2)
                {
                    stateMachine = State.PAUSED;
                    return;
                }

                if (playersFoundCounter == 2)
                {
                    stateMachine = State.PLAYING;
                }
                else
                {
                    if (!stateMachine.Equals(State.PAUSED))
                        stateMachine = State.WAITING_PLAYERS;
                }
            }
            else { stateMachine = State.STARTING; }
        }

    }

    void UpdateText()
    {

        switch (stateMachine)
        {

            case State.STARTING:
                displayedText.text = "Waiting for field...";
                break;
            case State.WAITING_PLAYERS:
                displayedText.text = "Waiting for " + (2 - playersFoundCounter) + " players...";
                break;
            case State.PLAYING:
                displayedText.text = "GO!";
                break;
            case State.PAUSED:
                displayedText.text = "PAUSE\n " + (2 - playersFoundCounter) + " undetected";
                break;
            case State.ENDED:
                displayedText.text = "Waiting for FIELD...";
                break;
        }
    }
}
