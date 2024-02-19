using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentPlayerDisplay : MonoBehaviour
{
    StateManager stateMngr;
    TextMeshProUGUI myText;


    // Start is called before the first frame update
    void Start()
    {
        stateMngr = GameObject.FindObjectOfType<StateManager>();
        myText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        myText.text = "Current Player: " + stateMngr.playerNames[stateMngr.currentPlayerId];
    }
}
