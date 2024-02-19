using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DiceRoller : MonoBehaviour
{
    public int[] diceValues;

    public Sprite[] diceImageOne;
    public Sprite[] diceImageZero;

    StateManager stateMngr;

    // Start is called before the first frame update
    void Start()
    {
        diceValues = new int[4];
        stateMngr = GameObject.FindObjectOfType<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RollDice() 
    {

        // In Ur you roll 4 dice (Tetrahedrons) which have half their faces denomated "1" and the other half "0".

        if (stateMngr.doneRolling == true) { return; }

        // TODO: physics enabled dice

        stateMngr.diceTotal = 0;

        // Random number generation
        for (int i = 0; i < diceValues.Length; ++i)
        {
            diceValues[i] = Random.Range(0, 2);
            stateMngr.diceTotal += diceValues[i];
        
            // Update the visuals to show the dice roll
            // TODO: could include 2D or 3D animations
            if (diceValues[i] == 0)
            {
                this.transform.GetChild(i).GetComponent<Image>().sprite = diceImageZero[Random.Range(0, diceImageZero.Length)];
            } else {
                this.transform.GetChild(i).GetComponent<Image>().sprite = diceImageOne[Random.Range(0, diceImageOne.Length)];
            }
            
        }

        //Debug.Log("Rolled: " + diceTotal);
        stateMngr.doneRolling = true;

        stateMngr.CheckLegalMoves();
    }
}
