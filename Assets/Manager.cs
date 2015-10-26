using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Manager : MonoBehaviour
{

    public Text displayedText = null;
    public int playersFoundCounter = 2;

    // Use this for initialization
    void Start()
    {
        displayedText = GetComponent<Text>();
        displayedText.text = "Waiting for FIELD...";
    }

    // Update is called once per frame
    void Update()
    {
        GameObject field = GameObject.FindGameObjectWithTag("Field");

        if (field)
        {
            if (field.GetComponent<MeshRenderer>().isVisible)
            {
                if (playersFoundCounter == 0)
                {
                    displayedText.text = "";
                }
                displayedText.text = "FIELD found, Waiting for " + playersFoundCounter + " players...";
            }
            else
                displayedText.text = "Waiting for FIELD...";

        }

    }
}
