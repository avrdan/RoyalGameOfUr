using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.WSA;
using System;
public class StateManager : MonoBehaviour
{
    public int nrPlayers = 2;
    public int currentPlayerId = 0;
    public int diceTotal;
    public bool doneRolling = false;
    public bool doneClicking = false;
    //public bool doneAnimating = false;
    public int animationsPlaying = 0;
    public GameObject noLegalMovesPopup;
    public GameObject gameOverPopup;
    

    BasicAI[] playerAIs;
    public int[] playerScores;
    // TODO: consider a humanizer library
    public string[] playerNames = { "White", "Red"};
    const int WIN_SCORE = 7;

    // Start is called before the first frame update
    void Start()
    {
        playerAIs = new BasicAI[nrPlayers];
        playerAIs[0] = null;
        //playerAIs[0] = new UtilityAI();
        //playerAIs[0] = new UtilityAI();
        playerAIs[1] = new UtilityAI();

        playerScores = new int[nrPlayers];
    }
    // Update is called once per frame
    void Update()
    {
        if (doneRolling && doneClicking && animationsPlaying == 0)
        {
            Debug.Log("Turn finished!");
            if (playerScores[currentPlayerId] < WIN_SCORE)
            {
                NewTurn();
            } else {
                StartCoroutine(GameOverCoroutine());
            }
            
        } else if (doneRolling && doneClicking && animationsPlaying != 0)
        {
            Debug.Log("Waiting for animation to finish...");
        }

        if (playerAIs != null && playerAIs[currentPlayerId] != null)
        {
            playerAIs[currentPlayerId].DoAI();
        }
    }

    public void NewTurn()
    {
        doneRolling = false;
        doneClicking = false;
        //doneAnimating = false;

        currentPlayerId = (currentPlayerId + 1 ) % nrPlayers;
    }

    public void RollAgain()
    {
        doneRolling = false;
        doneClicking = false;
        //doneAnimating = false;    
    }

    public void CheckLegalMoves()
    {
        // if we rolled a zero => no legal moves
        if (diceTotal == 0)
        {
            StartCoroutine(NoLegalMoveCoroutine());
            return;
        }

        // loop through all of the player's stones
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();
        bool hasLegalMove = false;

        foreach (PlayerStone ps in pss)
        {
            if (ps.playerId == currentPlayerId)
            {
                
                if (ps.CanLegallyMoveAhead(diceTotal))
                {
                    // TODO: highlight stones that can be legally moved
                    hasLegalMove = true;
                }
            }
        }

        if (!hasLegalMove)
        {
            // if no legal moves possible, wait a sec then move to next player
            StartCoroutine(NoLegalMoveCoroutine());
        }
    }

    IEnumerator NoLegalMoveCoroutine()
    {
        // Display message
        noLegalMovesPopup.SetActive(true);

        // Wait 1 sec
        yield return new WaitForSeconds(1f);

        noLegalMovesPopup.SetActive(false);

        NewTurn();
    }

    IEnumerator GameOverCoroutine()
    {
        // Display message
        TextMeshProUGUI textComp = gameOverPopup.GetComponentInChildren<TextMeshProUGUI>();
        textComp.text = playerNames[currentPlayerId] + " wins!";
        gameOverPopup.SetActive(true);

        // Wait 1 sec
        //yield return new WaitForSeconds(10f);
        yield return null;

        //gameOverPopup.SetActive(false);

        if (Input.GetMouseButtonUp(0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex,LoadSceneMode.Single);
        }
    }
}
