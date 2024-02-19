using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStone : MonoBehaviour
{
    public Tile startingTile;
    public int playerId;
    public StoneStorage myStoneStorage;
    public Tile currTile { get; protected set; }
    StateManager stateMngr;
    Tile[] moveQueue;
    int moveQueueIndex = -1;
    bool isAnimating = false;
    Vector3 targetPos;
    Vector3 velocity = Vector3.zero;
    float smoothTime = 0.25f;
    float smoothTimeVertical = 0.1f;
    float smoothDistance = 0.01f;
    float smoothHeight = 0.5f;

    PlayerStone stoneToBop;

    // Start is called before the first frame update
    void Start()
    {
        stateMngr = GameObject.FindObjectOfType<StateManager>();
        targetPos  = this.transform.position;     
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnimating) return;

        if (Vector3.Distance(
            new Vector3(this.transform.position.x, 0 , this.transform.position.z),
            new Vector3(targetPos.x, 0, targetPos.z)) 
            < smoothDistance)
        {
            // we've reached the target - do we have moves in the queue; remove y offset
            if (moveQueueIndex == -1 || (moveQueueIndex == moveQueue.Length) &&
            this.transform.position.y - targetPos.y > smoothDistance)
            {
                // above our target
                // out of moves, drop down
                this.transform.position = Vector3.SmoothDamp(
                    this.transform.position, 
                    new Vector3(this.transform.position.x, targetPos.y, this.transform.position.z),
                    ref velocity,
                    smoothTimeVertical);

                if (stoneToBop != null)
                {
                    stoneToBop.ReturnToStorage();
                    stoneToBop = null;
                }
            }
            else {
                // reached our last desired position - is there another move in the queue
                AdvanceMoveQueue();
            }
        }
        // rise up before next pos movement
        else if (this.transform.position.y < (smoothHeight - smoothDistance))
        {
            this.transform.position = Vector3.SmoothDamp(
                this.transform.position, 
                new Vector3(this.transform.position.x, smoothHeight, this.transform.position.z),
                ref velocity,
                smoothTimeVertical);
        } else
        {
            this.transform.position = Vector3.SmoothDamp(
                this.transform.position, 
                new Vector3(targetPos.x, smoothHeight, targetPos.z), 
                ref velocity, 
                smoothTime);
        }
    }

    void SetNewTargetPos(Vector3 pos)
    {
        targetPos = pos;
        velocity = Vector3.zero;
        this.isAnimating = true;
    }

    void AdvanceMoveQueue()
    {
        if (moveQueueIndex != -1 && moveQueueIndex < moveQueue.Length)
        {
            Tile nextTile = moveQueue[moveQueueIndex];
            if (nextTile == null)
            {
                return;
                // TODO: move to scored pile
                //SetNewTargetPos(this.transform.position + Vector3.right*10);
            }
            else 
            {
                SetNewTargetPos(moveQueue[moveQueueIndex].transform.position);
                moveQueueIndex++;
            }
        } else {
            isAnimating = false;
            --stateMngr.animationsPlaying;

            if (currTile != null && currTile.isScoringSpace)
            {
                this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
            // are we on a roll again space?
            else if (currTile != null && currTile.isRollAgain)
            {
                stateMngr.RollAgain();
            }
        }
    }

    public void MoveMe()
    {
        // TODO: is the mouse over a UI element? ignore click

        if (stateMngr.currentPlayerId != playerId)
        {
            return;
        }

        if (stateMngr.doneRolling == false) { return; }

        if (stateMngr.doneClicking) { return; }
        
        // move the stone
        int spacesToMove = stateMngr.diceTotal;

        if (spacesToMove < 1) return;

        // whwere should we end up?
        moveQueue = GetTilesAhead(spacesToMove);
        Tile finalTile = moveQueue[moveQueue.Length-1];

        if (finalTile == null)
        {
            return; // overshooting victory tile
        }

        if (!CanLegallyMoveTo(finalTile))
        {
            finalTile = currTile;
            moveQueueIndex = -1;
            return;
        }

        // if there is an enemy tile in place, kick it
        if (finalTile.playerStone != null)
        {
            stoneToBop = finalTile.playerStone;
            stoneToBop.currTile.playerStone = null;
            stoneToBop.currTile = null;
        }
        

        this.transform.SetParent(null);

        if (currTile != null)
        {
            currTile.playerStone = null;
        }
        
        if (finalTile != null) 
        {
            if (!finalTile.isScoringSpace) // "scoring" tiles are always empty
            {
                finalTile.playerStone = this;
            } else {
                stateMngr.playerScores[playerId]++;
            }
        }

        moveQueueIndex = 0;
        // set our current tile before the animation is done
        currTile = finalTile;
        stateMngr.doneClicking = true;
        isAnimating = true;
        ++stateMngr.animationsPlaying;
    }

    void OnMouseUp()
    {
        MoveMe();
    }

    public Tile[] GetTilesAhead(int spacesToMove)
    {
        if (spacesToMove < 1) return null;

        // whwere should we end up?
        Tile[] listOfTiles = new Tile[spacesToMove];
        Tile finalTile = currTile;

        for (int i = 0; i < spacesToMove; ++i) 
        {
            if (finalTile == null) 
            {
                finalTile = startingTile;
            } else {
                if (finalTile.nextTiles == null || finalTile.nextTiles.Length == 0) {
                    // overshooting the victory, so return some nulls in the array
                    break;
                } else if (finalTile.nextTiles.Length > 1)
                {
                    finalTile = finalTile.nextTiles[playerId];
                } else 
                {
                    finalTile = finalTile.nextTiles[0];
                }
                
            }
            listOfTiles[i] = finalTile;
        }

        return listOfTiles;
    }

    // Return the final tile we'd land on if we moved X spaces
    public Tile GetTileAhead(int spacesToMove)
    {
        Tile[] tiles = GetTilesAhead(spacesToMove);
        
        if (tiles == null)
        {
            return currTile; // we aren't moving at all
        }

        return tiles[tiles.Length-1];
    }

    public Tile GetTileAhead()
    {
        return GetTileAhead(stateMngr.diceTotal);
    }

    public bool CanLegallyMoveAhead(int spacesToMove)
    {
        if (currTile != null && currTile.isScoringSpace)
        {
            return false; // scoring tile so we cannot move
        }
        Tile tile = GetTileAhead(spacesToMove);
        return CanLegallyMoveTo(tile);
    }

    bool CanLegallyMoveTo(Tile destTile)
    { 
        if (destTile == null) return false; // off the board, overshooting the victory roll -> not legal

        // does the stile already have a  stone?
        if (destTile.playerStone == null)
        {
            return true;
        }
        // what type of stone?
        if (destTile.playerStone.playerId == this.playerId)
        {
            // can't land on own stone
            return false;
        }
        // if it's an enemy stone, is it in a safe square?
        if (destTile.isRollAgain)
        {
            return false;
        }

        return true;
    }

    public void ReturnToStorage()
    {
        //this.isAnimating = true;
        //++stateMngr.animationsPlaying;
        //currTile.playerStone = null;
        //currTile = null;
        moveQueueIndex = -1;
        Vector3 savePos = this.transform.position;
        myStoneStorage.AddStoneToStorage(this.gameObject);
        SetNewTargetPos(this.transform.position);
        this.transform.position = savePos;
    }
}
