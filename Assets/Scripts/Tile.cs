using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Tile
{
    //float height
    public float height;
    //enum tileType, the prefab ground type of the tile ex: grass, rock, bridge(has the actual tile lowered)
    public TileType tileType;
    //enum tileObject, the prefab that gets spawned on the Tile ex: tree, spikeWall, riverEast(river that flows to the right)
    public TileObject tileObject;
    //bool canNavigate
    public bool canNavigate;

    public Tile(float height, TileType tileType, TileObject tileObject, bool canNavigate)
    {
        this.height = height;
        this.tileType = tileType;
        this.tileObject = tileObject;
        this.canNavigate = canNavigate;
    }

    public enum TileType { empty, grassTile, rockTile, bridgeTile }
    public enum TileObject { empty, treeDeco, bushDeco, flowerDeco, grassDeco, rockDeco, riverDeco }
}
