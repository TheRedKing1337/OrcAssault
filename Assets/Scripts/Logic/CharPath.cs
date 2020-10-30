using UnityEngine;

public struct CharPath
{
    public Vector2Int[] path;
    public bool canStop;

    public CharPath(Vector2Int[] path, bool canStop)
    {
        this.path = path;
        this.canStop = canStop;
    }
}
