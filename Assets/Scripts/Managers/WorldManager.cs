using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoSingleton<WorldManager>
{
    public int worldSizeX = 1;
    public int worldSizeY = 1;
    public Tile[,] world;

    private MeshCombiner[] meshCombiners;

    //private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

    private GameObject[,] pillars;
    private GameObject[,] decos;

    private List<LerpObject> pillarList = new List<LerpObject>();
    private List<LerpObject> decoList = new List<LerpObject>();

    private void Start()
    {
        StartCoroutine(GenWorld());     
    }
    public void SkipWorldAnim(){
        StopAllCoroutines();
        FinishPlacement();
    }
    public void ReloadLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void CombineMeshes(){
        SkipWorldAnim();
        for (int i = 0; i < meshCombiners.Length; i++)
        {
            meshCombiners[i].CombineMesh();
        }
    }
    public bool CanNavigate(Vector2Int pos)
    {
        if (pos.x > worldSizeX-1 || pos.x < 0 || pos.y > worldSizeY-1 || pos.y < 0) return false;
        if (world[pos.x, pos.y].tileObject == TileObject.treeDeco) return false;
        return true;
    }
    public float GetHeight(Vector2Int pos)
    {
        if (pos.x > worldSizeX - 1 || pos.x < 0 || pos.y > worldSizeY - 1 || pos.y < 0) return 0;
        return world[pos.x,pos.y].height;
    }
    IEnumerator GenWorld()
    {
        world = new Tile[worldSizeX, worldSizeY];
        pillars = new GameObject[worldSizeX, worldSizeY];
        decos = new GameObject[worldSizeX, worldSizeY];

        meshCombiners = new MeshCombiner[2];

        GameObject tileObject = new GameObject("Tiles");
        tileObject.AddComponent<MeshFilter>();
        tileObject.AddComponent<MeshRenderer>();
        meshCombiners[0] = tileObject.AddComponent<MeshCombiner>();

        GameObject decoObject = new GameObject("Decorations");
        decoObject.AddComponent<MeshFilter>();
        decoObject.AddComponent<MeshRenderer>();
        meshCombiners[1] = decoObject.AddComponent<MeshCombiner>();

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                //replace this with loading from lvl file
                float perlinValue = Mathf.PerlinNoise(x / 10f, y / 10f) * 10;

                TileObject obj = (TileObject)Random.Range(0, 5);

                world[x, y] = new Tile(perlinValue, TileType.grassTile, obj, true);

                GameObject go = Instantiate(Resources.Load(world[x, y].tileType.ToString()), new Vector3(x, 0, y), Quaternion.identity) as GameObject;
                go.transform.SetParent(tileObject.transform);

                //LerpObject toMove = new LerpObject(go.transform, world[x, y].height);
                //moveDownList.Add(toMove);

                go.SetActive(false);
                pillars[x, y] = go;

                if (world[x, y].tileObject != TileObject.empty)
                {
                    GameObject deco = Instantiate(Resources.Load(world[x, y].tileObject.ToString()), new Vector3(x, world[x, y].height, y), Quaternion.identity) as GameObject;
                    deco.transform.SetParent(decoObject.transform);

                    deco.transform.localScale = Vector3.zero;
                    //LerpObject toGrow = new LerpObject(deco.transform, world[x, y].height);
                    //growList.Add(toGrow);

                    decos[x, y] = deco;
                }
            }
        }

        StartCoroutine(AddMeshesToList(pillars, pillarList));
        StartCoroutine(AnimatePillars());

        yield return new WaitForSeconds(2);

        StartCoroutine(AddMeshesToList(decos, decoList));
        StartCoroutine(AnimateDecos());

        yield break;
    }
    IEnumerator AddMeshesToList(GameObject[,] array, List<LerpObject> list)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);

        int xOffset = 0;
        int yOffset = 0;

        int smallestSide = GetSmallest(width, height);
        int largestSide = GetLargest(width, height);

        int numSides = smallestSide + largestSide - 1;

        for (int i = 0; i < numSides; i++)
        {
            int numRows = i + 1;
            if (numRows > largestSide)
            {
                numRows = numSides - i;
                xOffset++;
                yOffset++;
            }
            else if (numRows > smallestSide)
            {
                numRows = smallestSide;
                if (smallestSide == height)
                {
                    xOffset++;
                }
                else { yOffset++; }
            }

            for (int x = 0; x < numRows; x++)
            {
                int xPos = x + xOffset;
                int yPos = numRows - x - 1 + yOffset;

                //code here
                if (array[xPos, yPos] != null)
                {
                    LerpObject lo = new LerpObject(array[xPos, yPos].transform, world[xPos, yPos].height);
                    list.Add(lo);
                }
            }
            yield return null;
        }


        yield break;
    }
    int GetSmallest(int a, int b){
        return (a > b) ? b : a;
    }
    int GetLargest(int a, int b)
    {
        return (a > b) ? a : b;
    }
    IEnumerator AnimatePillars()
    {
        while (pillarList.Count > 0)
        {
            for (int i = 0; i < pillarList.Count; i++)
            {
                //THREAD BELOW
                //if is first time in loop, enable gameobject
                if(pillarList[i].lerpAmount == 0){
                    pillarList[i].tf.gameObject.SetActive(true);
                }
                    ////lerp to pos
                    pillarList[i].lerpAmount += Time.deltaTime * 0.5f;
                if (pillarList[i].lerpAmount > 1)
                {
                    pillarList.RemoveAt(i);
                    continue;
                }
                float newHeight = Mathf.Lerp(0, pillarList[i].height, pillarList[i].lerpAmount);
                pillarList[i].tf.position = new Vector3(pillarList[i].tf.position.x, newHeight, pillarList[i].tf.position.z);                
            }
            yield return null;
        }
        yield break;
    }
    IEnumerator AnimateDecos()
    {
        //wait for first object to be added to list, might not happen first frame
        while (decoList.Count == 0){
            yield return null;
        }
        while (decoList.Count > 0)
        {
            for (int i = 0; i < decoList.Count; i++)
            {
                decoList[i].lerpAmount += Time.deltaTime * 0.5f;
                if (decoList[i].lerpAmount > 1)
                {
                    decoList.RemoveAt(i);
                    continue;
                }

                float lerpValue = Mathf.Lerp(0, 1, decoList[i].lerpAmount);

                decoList[i].tf.localScale = new Vector3(lerpValue, lerpValue, lerpValue);                
            }
            yield return null;
        }
        //remove skip button    
        CombineMeshes();
        yield break;
    }
    private void FinishPlacement(){
        StopAllCoroutines();        
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                pillars[x, y].SetActive(true);
                pillars[x, y].transform.position = new Vector3(pillars[x, y].transform.position.x, world[x,y].height, pillars[x, y].transform.position.z);
            }
        }
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (decos[x, y] != null)
                {
                    decos[x, y].transform.localScale = Vector3.one;
                }
            }
        }
        TurnManager.Instance.phase = TurnManager.Phases.playerMove;
    }
    public class LerpObject
    { 
        public Transform tf; 
        public float height; 
        public float lerpAmount; 
        public LerpObject(Transform tf, float height) { this.tf = tf; this.height = height; this.lerpAmount = 0; } 
    }
    public struct Tile {
        //float height
        public float height;
        //enum tileType, the prefab ground type of the tile ex: grass, rock, bridge(has the actual tile lowered)
        public TileType tileType;
        //enum tileObject, the prefab that gets spawned on the Tile ex: tree, spikeWall, riverEast(river that flows to the right)
        public TileObject tileObject;
        //bool canNavigate
        public bool canNavigate;

        public Tile(float height, TileType tileType, TileObject tileObject, bool canNavigate) {
            this.height = height;
            this.tileType = tileType;
            this.tileObject = tileObject;
            this.canNavigate = canNavigate;
        }
    }
    public enum TileType { empty, grassTile, rockTile, bridgeTile}
    public enum TileObject { empty, treeDeco, bushDeco, flowerDeco,grassDeco, rockDeco, riverDeco}

}
