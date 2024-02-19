using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI
{
    StateManager stateMngr;

    public BasicAI()
    {
        stateMngr = GameObject.FindObjectOfType<StateManager>();
    }

    virtual public void DoAI()
    {
        if(!stateMngr.doneRolling)
        {
            // roll the dice
            DoRoll();
            return;
        }

        if (!stateMngr.doneClicking)
        {
            DoClick();
            return;
        }
    }

    virtual protected void DoRoll()
    {
        DiceRoller diceRoller = GameObject.FindObjectOfType<DiceRoller>();
        diceRoller.RollDice();
    }

    virtual protected void DoClick()
    {
        // pick a stone to move, then "click" it

        // BasicAI simply picks a legal move at random
        PlayerStone[] legalStones = GetLegalMoves();

        if (legalStones == null || legalStones.Length == 0)
        {
            // no legal moves
            return;
        }

        PlayerStone stone = PickStoneToMove(legalStones);
        stone.MoveMe();
    }

    virtual protected PlayerStone PickStoneToMove(PlayerStone[] legalStones)
    {
        return legalStones[Random.Range(0, legalStones.Length)];
    }

    // Returns a list of stones that can be legally moved
    protected PlayerStone[] GetLegalMoves()
    {
        List<PlayerStone> legalStones = new List<PlayerStone>();
        // if we rolled a zero => no legal moves
        if (stateMngr.diceTotal == 0)
        {
            return legalStones.ToArray();
        }

        // loop through all of the player's stones
        PlayerStone[] pss = GameObject.FindObjectsOfType<PlayerStone>();
        foreach (PlayerStone ps in pss)
        {
            if (ps.playerId == stateMngr.currentPlayerId)
            {
                
                if (ps.CanLegallyMoveAhead(stateMngr.diceTotal))
                {
                    legalStones.Add(ps);
                }
            }
        }

        return legalStones.ToArray();
    }
}
