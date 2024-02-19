using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneStorage : MonoBehaviour
{
    public Tile startingTile;
    public GameObject stonePrefab;

    // Start is called before the first frame update
    void Start()
    {
        // create one stone for each placeholder spot
        for (int i = 0; i < this.transform.childCount ; ++i)
        {
            GameObject stone = Instantiate(stonePrefab);
            stone.GetComponent<PlayerStone>().startingTile = this.startingTile;
            stone.GetComponent<PlayerStone>().myStoneStorage = this;
            AddStoneToStorage(stone, this.transform.GetChild(i));
        }
    }

    public void AddStoneToStorage(GameObject stone, Transform placeholder = null)
    {
        if (placeholder == null)
        {
            // find first empty placeholder
            for (int i = 0; i < this.transform.childCount; ++i)
            {
                Transform p = this.transform.GetChild(i);
                if (p.childCount == 0) { // placeholder is empty
                    placeholder = p;
                    break;
                }
            }
        }
        
        if (placeholder == null)
        {
            Debug.LogError("Trying to add stone, but all placeholders are filled.");
        }
        
        // parent stone to the placeholder
        stone.transform.SetParent(placeholder);

        // reset the stone's local position
        stone.transform.localPosition = Vector3.zero;
    }
}
