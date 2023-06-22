using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Material worldMaterial;
    private Container container;

    private void Start()
    {
        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        container = cont.AddComponent<Container>();

        container.Initialized(worldMaterial, Vector3.zero);
        container.GenerateMesh();
        container.UploadMesh();
    }


}
