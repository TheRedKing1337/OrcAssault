using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelEditor : MonoBehaviour
{
    public float maxTapLength = 0.25f;

    public Slider heightSlider;
    public Dropdown typeDropdown;
    public Dropdown decoDropdown;

    private Vector2Int selectedPos;
    private Tile selectedTile;
    private GameObject selectedTileObj;
    private float tapTimer;
    private bool isSettingValues;

    private void Awake()
    {
        heightSlider.onValueChanged.AddListener(delegate { OnHeightChanged(); });
        typeDropdown.onValueChanged.AddListener(delegate { OnTypeChanged(); });
        decoDropdown.onValueChanged.AddListener(delegate { OnDecoChanged(); });
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            tapTimer = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (tapTimer + maxTapLength < Time.time) { return; }
            RaycastHit hit;

            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

            if (hit.transform != null)
            {
                Debug.Log(hit.transform.position.x + "   " + hit.transform.position.z);

                Vector2Int tapPos = new Vector2Int((int)hit.transform.position.x, (int)hit.transform.position.z);

                if (tapPos == selectedPos)
                {
                    HideTileUI();
                    tapPos = Vector2Int.zero;
                }
                else
                {
                    ShowTileUI();
                    selectedPos = tapPos;
                }
                selectedTile = WorldManager.Instance.world[selectedPos.x, selectedPos.y];
                selectedTileObj = WorldManager.Instance.pillars[selectedPos.x, selectedPos.y];

                //isSettingValues makes the system skip the onValueChanged event
                isSettingValues = true;
                heightSlider.value = selectedTile.height;
                typeDropdown.value = (int)selectedTile.tileType;
                decoDropdown.value = (int)selectedTile.tileObject;
                isSettingValues = false;
            }
        }
    }
    private void OnHeightChanged()
    {
        if (isSettingValues) { return; }
        selectedTile.height = heightSlider.value;
        selectedTileObj.transform.position = new Vector3(selectedTileObj.transform.position.x, selectedTile.height, selectedTileObj.transform.position.z);

        WorldManager.Instance.world[selectedPos.x, selectedPos.y] = selectedTile;
    }
    private void OnTypeChanged()
    {
        if (isSettingValues) { return; }

        SetTileType(selectedPos, (Tile.TileType)typeDropdown.value);
    }
    private void OnDecoChanged()
    {
        if (isSettingValues) { return; }

        SetDecoType(selectedPos, (Tile.TileObject)decoDropdown.value);
    }
    private void SetTileType(Vector2Int pos, Tile.TileType tileType)
    {
        WorldManager.Instance.world[pos.x, pos.y].tileType = tileType;
        Tile tile = WorldManager.Instance.world[pos.x, pos.y];
        GameObject tileObj = WorldManager.Instance.pillars[pos.x, pos.y];
        //if wasnt empty delete old tile       
        if (tileObj.transform.childCount > 0)
        {
            Destroy(tileObj.transform.GetChild(0).gameObject);
        }

        if (tile.tileType == Tile.TileType.empty) { SetDecoType(pos, Tile.TileObject.empty); return; }

        GameObject go = Instantiate(Resources.Load(tile.tileType.ToString()), new Vector3(pos.x, tile.height, pos.y), Quaternion.identity) as GameObject;
        go.transform.SetParent(tileObj.transform);

        WorldManager.Instance.world[pos.x, pos.y] = tile;

        if (WorldManager.Instance.world[pos.x, pos.y].tileObject != Tile.TileObject.empty)
        {
            SetDecoType(pos, tile.tileObject);
        }
    }
    private void SetDecoType(Vector2Int pos, Tile.TileObject tileDeco)
    {
        WorldManager.Instance.world[pos.x, pos.y].tileObject = tileDeco;
        Tile tile = WorldManager.Instance.world[pos.x, pos.y];
        GameObject tileObj = WorldManager.Instance.pillars[pos.x, pos.y];

        if (tileObj.transform.childCount > 1)
        {
            Destroy(tileObj.transform.GetChild(1).gameObject);
        }

        if (tile.tileObject == Tile.TileObject.empty) { return; }

        GameObject go = Instantiate(Resources.Load(tile.tileObject.ToString()), new Vector3(pos.x, tile.height, pos.y), Quaternion.identity) as GameObject;
        go.transform.SetParent(tileObj.transform);

        WorldManager.Instance.world[pos.x, pos.y] = tile;
    }
    private void ShowTileUI()
    {
        heightSlider.gameObject.SetActive(true);
        typeDropdown.transform.parent.gameObject.SetActive(true);
    }
    private void HideTileUI()
    {
        heightSlider.gameObject.SetActive(false);
        typeDropdown.transform.parent.gameObject.SetActive(false);
    }
}
