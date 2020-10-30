using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    //get data from all 3 save files and show in UI
    public void SelectLevel(int id){
        //load data from level id into GlobalVar and load HomeScreen scene
    }
    public void CreateNew(int id){
        //start new save, ask for name and load HomeScreen
    }
    public void Delete(int id){
        //deletes save, ask for confirmation and set all values to 0, false or ""
    }
}
