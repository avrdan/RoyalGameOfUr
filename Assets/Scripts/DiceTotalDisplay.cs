using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DicelTotalDisplay : MonoBehaviour
{
    StateManager stateMngr;

    // Start is called before the first frame update
    void Start()
    {
        stateMngr = GameObject.FindObjectOfType<StateManager>();
        if (stateMngr == null ) {
            Debug.LogError("Dice roller not found! Cannot update text.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (stateMngr.doneRolling == false)
        {
            GetComponent<TextMeshProUGUI>().text = "= ?";
        } else {
            GetComponent<TextMeshProUGUI>().text = "= " + stateMngr.diceTotal;
        }
        
    }
}
