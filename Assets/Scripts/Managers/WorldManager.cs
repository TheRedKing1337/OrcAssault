using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoSingleton<WorldManager>
{
    public int worldSizeX = 1;
    public int worldSizeY = 1;
    public bool skipAnimation;
    public bool combineMeshes = true;

    public Tile[,] world;

    private MeshCombiner[] meshCombiners;

    //private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

    private GameObject tileObject;
    private GameObject decoObject;

    public GameObject[,] pillars;
    private GameObject[,] decos;

    private List<LerpObject> pillarList = new List<LerpObject>();
    private List<LerpObject> decoList = new List<LerpObject>();

    //[ContextMenu("Reload World")]
    //private void ReloadInEditor()
    //{
    //    //remove old versions
    //    DestroyImmediate(GameObject.Find("Tiles"));
    //    DestroyImmediate(GameObject.Find("Decorations"));

    //    //build world and skip anims and dont combine meshes
    //    StartCoroutine(GenWorld(true, false));
    //}
    private void Start()
    {
        StartCoroutine(GenWorld(skipAnimation, combineMeshes));
    }

    #region Testing functions
    public void ReloadLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SkipWorldAnim()
    {
        StopAllCoroutines();
        FinishPlacement();
    }
    #endregion

    #region World building functions
    private void FinishPlacement()
    {
        StopAllCoroutines();
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                pillars[x, y].SetActive(true);
                pillars[x, y].transform.position = new Vector3(pillars[x, y].transform.position.x, world[x, y].height, pillars[x, y].transform.position.z);
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
    public void CombineMeshes(){
        for (int i = 0; i < meshCombiners.Length; i++)
        {
            meshCombiners[i].CombineMesh();
        }
    }
    public bool CanNavigate(Vector2Int pos)
    {
        //check if is bounds of array
        if (pos.x > worldSizeX-1 || pos.x < 0 || pos.y > worldSizeY-1 || pos.y < 0) return false;
        return world[pos.x, pos.y].canNavigate;
    }
    public float GetHeight(Vector2Int pos)
    {
        if (pos.x > worldSizeX - 1 || pos.x < 0 || pos.y > worldSizeY - 1 || pos.y < 0) return 0;
        return world[pos.x,pos.y].height;
    }
    IEnumerator GenWorld(bool skipAnims, bool combineMeshes)
    {
        world = new Tile[worldSizeX, worldSizeY];
        pillars = new GameObject[worldSizeX, worldSizeY];
        decos = new GameObject[worldSizeX, worldSizeY];

        meshCombiners = new MeshCombiner[2];

        tileObject = new GameObject("Tiles");
        tileObject.AddComponent<MeshFilter>();
        tileObject.AddComponent<MeshRenderer>();
        meshCombiners[0] = tileObject.AddComponent<MeshCombiner>();

        decoObject = new GameObject("Decorations");
        decoObject.AddComponent<MeshFilter>();
        decoObject.AddComponent<MeshRenderer>();
        meshCombiners[1] = decoObject.AddComponent<MeshCombiner>();

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                //replace this with loading from lvl file   -4 to bring terrain to around 0
                float perlinValue = Mathf.PerlinNoise(x / 10f, y / 10f) * 10 - 4;

                Tile.TileObject obj = (Tile.TileObject)Random.Range(0, 5);

                world[x, y] = new Tile(perlinValue, Tile.TileType.grassTile, obj, true);

                GameObject go = Instantiate(Resources.Load(world[x, y].tileType.ToString()), new Vector3(x, 0, y), Quaternion.identity) as GameObject;
                go.transform.SetParent(tileObject.transform);
                go.name = x + "|" + y;
                //LerpObject toMove = new LerpObject(go.transform, world[x, y].height);
                //moveDownList.Add(toMove);

                go.transform.GetChild(0).gameObject.SetActive(false);
                pillars[x, y] = go.transform.GetChild(0).gameObject;

                if (world[x, y].tileObject != Tile.TileObject.empty)
                {
                    GameObject deco = Instantiate(Resources.Load(world[x, y].tileObject.ToString()), new Vector3(x, world[x, y].height, y), Quaternion.identity) as GameObject;
                    deco.transform.SetParent(go.transform);

                    deco.transform.localScale = Vector3.zero;
                    //LerpObject toGrow = new LerpObject(deco.transform, world[x, y].height);
                    //growList.Add(toGrow);

                    decos[x, y] = deco;
                }
            }
        }

        if (!skipAnims)
        {
            StartCoroutine(AddMeshesToList(pillars, pillarList));
            StartCoroutine(AnimatePillars());

            yield return new WaitForSeconds(2);

            StartCoroutine(AddMeshesToList(decos, decoList));
            yield return StartCoroutine(AnimateDecos());
        }

        FinishPlacement();

        if (combineMeshes)   
        {
            CombineMeshes();
        }

        yield break;
    }
    #endregion

    #region World animation functions
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

        yield break;
    }
    public class LerpObject
    { 
        public Transform tf; 
        public float height; 
        public float lerpAmount; 
        public LerpObject(Transform tf, float height) { this.tf = tf; this.height = height; this.lerpAmount = 0; } 
    }
    #endregion
}
