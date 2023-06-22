using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 본 스크립트가 삽입된 객체에서는 아래의 컴포넌트가 자동으로 추가됩니다.
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Container : MonoBehaviour
{
    public Vector3 containerPosition;
    private MeshData meshData;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    public void Initialized(Material mat, Vector3 position)
    {
        ConfigureComponents();
        meshRenderer.sharedMaterial = mat;
        containerPosition = position;
    }

    private void ConfigureComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();    
    }

    public void GenerateMesh()
    {
        meshData.ClearData();

        Vector3 blockPos =  new Vector3(8,8,8);
        Voxel block = new Voxel() { ID = 1 };

        int counter = 0;
        Vector3[] faceVertices = new Vector3[4];
        Vector2[] faceUVs = new Vector2[4];

        for(int i = 0; i < 6; i++)
        {
            // Draw this Face;

            // Collect the appropriate vertices from the default vertices and add the block position.
            for(int j = 0; j < 4; j++)
            {
                faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + blockPos;
                faceUVs[j] = voxelUVs[j];
            }

            for(int j = 0; j  < 6; j++)
            {
                meshData.vertices.Add(faceVertices[voxelTris[i, j]]);
                meshData.UVs.Add(faceUVs[voxelTris[i, j]]);

                meshData.triangles.Add(counter++);

            }
        }
    }

    public void UploadMesh()
    {
        meshData.UploadMesh();

        if (meshRenderer == null)
            ConfigureComponents();

        meshFilter.mesh = meshData.mesh;

        if (meshData.vertices.Count > 3)
            meshCollider.sharedMesh = meshData.mesh;

    }

    #region meshData
    public struct MeshData
    {
        public Mesh mesh;
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> UVs;

        public bool Initialized;

        public void ClearData()
        {
            if(!Initialized)
            {
                vertices = new List<Vector3>();
                triangles = new List<int>();
                UVs = new List<Vector2>();

                Initialized = true;
                mesh = new Mesh();
            }
            else
            {
                vertices.Clear();
                triangles.Clear();
                UVs.Clear();
                mesh.Clear();
            }
        }

        public void UploadMesh(bool sharedVertices = false)
        {
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0, true);
            mesh.SetUVs(0, UVs);

            mesh.Optimize();

            mesh.RecalculateNormals();

            mesh.RecalculateBounds();

            mesh.UploadMeshData(false);
        }
    }
    #endregion

    #region Voxel Statics

    static readonly Vector3[] voxelVertices = new Vector3[8]
    {
        // 8방향 버텍스
        new Vector3 (0, 0, 0), // 0
        new Vector3 (1, 0, 0), // 1
        new Vector3 (0, 1 ,0), // 2
        new Vector3 (1, 1, 0), // 3

        new Vector3 (0, 0, 1), // 4
        new Vector3 (1, 0, 1), // 5 
        new Vector3 (0, 1, 1), // 6
        new Vector3 (1, 1, 1), // 7
    };

    static readonly int[,] voxelVertexIndex = new int[6, 4]
    {
        {0, 1, 2, 3 },
        {4, 5, 6, 7 },
        {4, 0, 6, 2 },
        {5, 1, 7, 3 },
        {0, 1, 4, 5 },
        {2, 3, 6, 7 }
    };

    static readonly Vector2[] voxelUVs = new Vector2[4]
    {
        new Vector2(0,0),
        new Vector2(0,1),
        new Vector2(1,0),
        new Vector2(1,1)
    };

    static readonly int[,] voxelTris = new int[6, 6]
    {
        {0, 2, 3, 0, 3 ,1 },
        {0, 1, 2, 1, 3, 2 },
        {0, 2, 3, 0, 3, 1 },
        {0, 1, 2, 1, 3, 2 },
        {0, 1, 2, 1, 3, 2 },
        {0, 2, 3, 0, 3, 1 },
    };

    #endregion

}
