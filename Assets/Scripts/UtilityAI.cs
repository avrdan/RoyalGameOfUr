using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAI : BasicAI
{

Dictionary<Tile, float> tileDanger;

virtual protected void CalcTileDanger(int playerId)
{
    tileDanger = new Dictionary<Tile, float>();
    Tile[] tiles = GameObject.FindObjectsOfType<Tile>();

    foreach(Tile t in tiles)
    {
        tileDanger[t] = 0;
    }

    PlayerStone[] allStones = GameObject.FindObjectsOfType<PlayerStone>();
    foreach (PlayerStone stone in allStones)
    {
        // if enemy, add a "danger" value in front of it (unless safe)
        if (stone.playerId == playerId)
        {
            continue;
        }

        for (int i = 1; i <= 4; ++i)
        {
            Tile t = stone.GetTileAhead(i);

            if (t == null)
            {
                // invalid tiles
                break;
            }

            if (t.isScoringSpace || t.isSideline || t.isRollAgain)
            {
                // this tile is not a danger zone, so we can ignore it
                continue;
            }

            // this tile is within bopping range => dangerous
            if (i == 2) // most dangerous
            {
                tileDanger[t] += 0.3f;
            } else {
                tileDanger[t] += 0.2f;
            }
        }
    }
}

virtual protected float GetStoneWeight(PlayerStone stone, Tile currTile, Tile futureTile)
{
    float weight = Random.Range(-0.1f, 0.1f); // tie-breaker, noise value
    float aggressivenessBonus = 0.25f; // TODO: added for bops, removed for staying on safe spaces

    if (currTile == null)
    {
        // nice to add more stones to the board
        weight += 0.2f;
    }

    if (currTile != null && currTile.isRollAgain && !currTile.isSideline) // roll-again space in the middle
    {
        weight -= 0.10f;
    }

    if (futureTile.isRollAgain)
    {
        weight += 0.5f;
    }

    if (futureTile.playerStone != null && futureTile.playerStone.playerId != stone.playerId)
    {
        weight += 0.5f; // there's an enemy stone on top
    }

    if (futureTile.isScoringSpace)
    {
        weight += 0.5f;
    }

    float currDanger = 0;
    if (currTile != null)
    {
        currDanger = tileDanger[currTile];
    }
    weight += currDanger - tileDanger[futureTile];
 
    // TODO: add weight for tiles that are behind enemies, and likely to contribute tu boppage
    // TODO: add weight for moving a stone forward when we might be blocking

    return 0;
}
override protected PlayerStone PickStoneToMove(PlayerStone[] legalStones)
{
    if (legalStones == null || legalStones.Length == 0)
    {
        Debug.LogError("No stone left to pick.");
    }

    CalcTileDanger(legalStones[0].playerId);

    // rank how good it would be to pick it: 1 -> great, -1 -> abysmal
    PlayerStone bestStone = null;
    float weight = -Mathf.Infinity;
    
    foreach (PlayerStone stone in legalStones)
    {
        float w = GetStoneWeight(stone, stone.currTile, stone.GetTileAhead());
        if (bestStone == null || w > weight)
        {
            bestStone = stone;
            weight = w;
        }
    }
    
    return bestStone;;
}

}
