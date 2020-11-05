using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

                //GameObject Obj =
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
                selectedTileObj = WorldManager.Instance.pillars[selectedPos.x, selectedPos.y].transform.parent.gameObject;

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
        if(isSettingValues) { return; }
        selectedTile.height = heightSlider.value;
        selectedTileObj.transform.position = new Vector3(selectedTileObj.transform.position.x, selectedTile.height, selectedTileObj.transform.position.z);
    }
    private void OnTypeChanged() {
        if (isSettingValues) { return; }
        selectedTile.tileType = (Tile.TileType)typeDropdown.value;

        if (selectedTileObj.transform.childCount > 0)
        {
            Destroy(selectedTileObj.transform.GetChild(0).gameObject);
        }

        if(selectedTile.tileType == Tile.TileType.empty){ return; }

        GameObject go = Instantiate(Resources.Load(selectedTile.tileType.ToString()), new Vector3(selectedPos.x, selectedTile.height, selectedPos.y), Quaternion.identity) as GameObject;
        go.transform.SetParent(selectedTileObj.transform);
    }
    private void OnDecoChanged()
    {
        if (isSettingValues) { return; }
        selectedTile.tileObject = (Tile.TileObject)decoDropdown.value;
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
