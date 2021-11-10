using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexOffset : MonoBehaviour
{
    public enum OptMode
    {
        MeshDeform,
        MeshRevert,
    }

    [Header("操作模式")] public OptMode mode = OptMode.MeshDeform;

    [Header("顶点按法线方向偏移")] public float vertexOffset = 0.2f;

    [Header("画刷半径大小")] public float brushRadius = 0.1f;

    private Mesh mesh;
    private Vector3[] orgVertices;
    private Vector3[] cacheVertices;
    private Vector3[] normals;

    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        orgVertices = mesh.vertices;
        cacheVertices = mesh.vertices;
        normals = mesh.normals;
        _camera = Camera.main;

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == OptMode.MeshDeform)
        {
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < orgVertices.Length; i++)
                {
                    cacheVertices[i] = OffetVertex(orgVertices[i], normals[i], vertexOffset);
                }

                UpdateMesh();
            }
        }
        else if (mode == OptMode.MeshRevert)
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Transform objectHit = hit.transform;
                    if (hit.transform == transform)
                    {
                        TryMeshRevert(hit.point);
                    }
                }
            }
           
        }
    }

    private void UpdateMesh()
    {
        mesh.vertices = cacheVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
    }

    Vector3 OffetVertex(Vector3 vertexIn, Vector3 normal, float dis)
    {
        return vertexIn + dis * normal;
    }

    void TryMeshRevert(Vector3 hitPos)
    {
        bool bUpdate = false;
        var modelPos = transform.InverseTransformPoint(hitPos);
        for (int i = 0; i < cacheVertices.Length; i++)
        {
            var len = cacheVertices[i] - modelPos;
            if (len.magnitude <= brushRadius)
            {
                cacheVertices[i] = orgVertices[i];
                bUpdate = true;
            }
        }

        if (bUpdate)
        {
            UpdateMesh();
        }
    }
}