using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    public void CombineMesh(){
        Quaternion oldRot = transform.rotation;
        Vector3 oldPos = transform.position;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        Debug.Log(name + " is combining " + filters.Length + " meshes!");

        Mesh finalMesh = new Mesh();

        //set to 32 bit mesh to allow for more tris, needed for foliage mesh
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        CombineInstance[] combiners = new CombineInstance[filters.Length];

        for(int a=0;a<filters.Length;a++){
            if (filters[a].transform == transform) continue;

            combiners[a].subMeshIndex = 0;
            combiners[a].mesh = filters[a].sharedMesh;
            combiners[a].transform = filters[a].transform.localToWorldMatrix;
        }

        finalMesh.CombineMeshes(combiners);

        GetComponent<MeshRenderer>().sharedMaterial = filters[1].gameObject.GetComponent<MeshRenderer>().sharedMaterial;

        GetComponent<MeshFilter>().sharedMesh = finalMesh;

        transform.rotation = oldRot;
        transform.position = oldPos;

        for(int i=0;i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
