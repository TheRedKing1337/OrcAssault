using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharPath[] paths = new CharPath[4];
    public Vector2Int currentPos = new Vector2Int(0, 0);
    public float stepHeight = 1;

    public GameObject arrowPrefab;
    public GameObject endPointPrefab;

    // Start is called before the first frame update
    void Start()
    {
        paths[0] = new CharPath(new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3) }, true);
        paths[1] = new CharPath(new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(0, -2), new Vector2Int(0, -3) }, true);
        paths[2] = new CharPath(new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0) }, true);
        paths[3] = new CharPath(new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(-2, 0), new Vector2Int(-3, 0) }, true);

        //ShowPaths();
    }
    private void OnMouseDown()
    {
        if (TurnManager.Instance.phase == TurnManager.Phases.playerMove)
        {
            TurnManager.Instance.SelectCharacter(this);
        }
    }
    public void Select()
    {
        Outline ol = GetComponent<Outline>();
        ol.OutlineColor = Color.yellow;
        ol.OutlineMode = Outline.Mode.OutlineAll;
    }
    public void DeSelect()
    {
        Outline ol = GetComponent<Outline>();
        ol.OutlineColor = Color.blue;
        ol.OutlineMode = Outline.Mode.OutlineHidden;
    }

    public void ShowPaths()
    {
        for (int i = 0; i < paths.Length; i++)
        {
            if (CheckPath(paths[i]))
            {
                DrawPath(paths[i], i);
            }
        }
    }
    bool CheckPath(CharPath path)
    {
        if (path.canStop)
        {
            if (WorldManager.Instance.CanNavigate(path.path[0] + currentPos)) //if first point is in bounds you can atleast move 1 tile in said path
            {
                return true;
            }
            else { return false; }
        }
        else
        {
            if (WorldManager.Instance.CanNavigate(path.path[path.path.Length - 1] + currentPos)) //if endpoint is navigable
            {
                for (int i = 0; i < path.path.Length - 1; i++)
                {
                    if (WorldManager.Instance.CanNavigate(path.path[i] + currentPos)) { Debug.Log("midpoint was not navigable"); return false; }
                    //if the stepheight between point 0 and 1 is small enough
                    float h1 = WorldManager.Instance.GetHeight(path.path[i] + currentPos);
                    float h2 = WorldManager.Instance.GetHeight(path.path[i + 1] + currentPos);
                    if (Mathf.Abs(h2 - h1) > stepHeight)
                    {
                        Debug.Log("Stepheight for path was too high");
                        return false;
                    }
                }
                return true;
            }
            Debug.Log("endpoint was not navigable");
            return false;
        }
    }
    void DrawPath(CharPath path, int pathsIndex)
    {
        for (int i = 0; i < path.path.Length; i++)
        {
            if (WorldManager.Instance.CanNavigate(path.path[i] + currentPos))
            {
                if (i < path.path.Length - 1)
                {
                    float h1 = WorldManager.Instance.GetHeight(path.path[i] + currentPos);
                    float h2 = WorldManager.Instance.GetHeight(path.path[i + 1] + currentPos);
                    if (Mathf.Abs(h2 - h1) > stepHeight)
                    {
                        //Debug.Log("Stepheight for path was too high");
                        break;
                    }
                }

                Vector3 pos = new Vector3(path.path[i].x + currentPos.x, WorldManager.Instance.GetHeight(path.path[i] + currentPos), path.path[i].y + currentPos.y);
                if (i == path.path.Length - 1 || path.canStop)
                {
                    SelectedTile a = Instantiate(endPointPrefab, pos, endPointPrefab.transform.rotation).GetComponent<SelectedTile>();
                    a.pathsIndex = pathsIndex;
                    a.pathPointIndex = i;
                    TurnManager.Instance.moveTiles.Add(a.gameObject);
                }
                else
                {
                    Vector3 nextPos = new Vector3(path.path[i + 1].x + currentPos.x, WorldManager.Instance.GetHeight(path.path[i] + currentPos), path.path[i + 1].y + currentPos.y);
                    GameObject go = Instantiate(arrowPrefab, pos, arrowPrefab.transform.rotation);
                    go.transform.LookAt(nextPos, Vector3.up);
                    go.transform.Rotate(new Vector3(90, 0, 0));
                    TurnManager.Instance.moveTiles.Add(go);
                }
            } else {
                break;
            }
        }
    }
}
