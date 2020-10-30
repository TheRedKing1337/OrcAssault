using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoSingleton<TurnManager>
{
    bool isPlayerTurn; //maybe use enum, playerMove, playerAttack, enemyMove, enemyAttack
    public enum Phases { playerMove, enemyMove, wait }
    public Phases phase = Phases.wait;

    //the list that contains all the highlighted tiles
    public List<GameObject> moveTiles = new List<GameObject>();

    private Character selectedChar;

    //select move, called by the SelectedTile, plays moving animation, and transitions to playerAttack once done
    public void SelectTile(SelectedTile tile)
    {
        phase = Phases.wait;
        //delete highlighted tiles
        for(int i=0;i<moveTiles.Count;i++){
            Destroy(moveTiles[i]);
        }
        //move selectedChar
        StartCoroutine(MoveCharacterAlongPath(selectedChar.paths[tile.pathsIndex], tile.pathPointIndex));
    }
    IEnumerator MoveCharacterAlongPath(CharPath path, int indexToMoveTo)
    {
        Vector2Int oldPos = selectedChar.currentPos;
        for (int i=0;i<indexToMoveTo+1;i++)
        {
            //this but play animation instead
            selectedChar.gameObject.transform.position = new Vector3(path.path[i].x + oldPos.x, WorldManager.Instance.GetHeight(path.path[i] + oldPos), path.path[i].y + oldPos.y);
            yield return new WaitForSeconds(0.5f);
        }
        selectedChar.currentPos = path.path[indexToMoveTo] + oldPos;
        selectedChar.DeSelect();
        selectedChar = null;

        StartCoroutine(EnemyTurn());
    }
    public void SelectCharacter(Character selectedChar)
    {        
        if (selectedChar != this.selectedChar)
        {
            //delete highlighted tiles
            for (int i = 0; i < moveTiles.Count; i++)
            {
                Destroy(moveTiles[i]);
            }
            if (this.selectedChar != null)
            {
                this.selectedChar.DeSelect();
            }
            this.selectedChar = selectedChar;
            selectedChar.Select();
            selectedChar.ShowPaths();
        } else {
            this.selectedChar.DeSelect();
            this.selectedChar = null;
            //delete highlighted tiles
            for (int i = 0; i < moveTiles.Count; i++)
            {
                Destroy(moveTiles[i]);
            }
        }   
    }

    //Player move turn, allows player to select characters, once selected show paths
    public IEnumerator PlayerTurn()
    {
        Debug.Log("Start Player Move phase");
        phase = Phases.playerMove;

        yield break;
    }

    //Enemy move turn, ai moves , plays moving animation and transitions to enemyAttack
    public IEnumerator EnemyTurn()
    {
        selectedChar = null;
        phase = Phases.enemyMove;
        Debug.Log("Start Enemy Move phase");

        StartCoroutine(PlayerTurn());
        yield break;
    }
}
