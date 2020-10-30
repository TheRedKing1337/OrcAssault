using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedTile : MonoBehaviour
{
    public int pathsIndex;  //the index of paths[] that this tile belongs to
    public int pathPointIndex;  //if canStop, the tile where the path stops
    private void OnMouseDown()
    {
        TurnManager.Instance.SelectTile(this);
    }
}
