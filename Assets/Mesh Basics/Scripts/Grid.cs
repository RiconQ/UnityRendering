using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    public int xSize, ySize;

    private Vector3[] m_vertices;
    private Mesh m_mesh;

    private void Awake()
    {
        Generate();
    }

    private async void Generate()
    {
        m_vertices = new Vector3[(xSize + 1) * (ySize + 1)];

        m_mesh = new Mesh();
        m_mesh.name = "Procedural Mesh";
        GetComponent<MeshFilter>().mesh = m_mesh;

        var uv = new Vector2[m_vertices.Length];
        var tangents = new Vector4[m_vertices.Length];
        var tangent = new Vector4(1f, 0f, 0f, -1f);
        // Vertex, UV, Tangent
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                m_vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }
        m_mesh.vertices = m_vertices;
        m_mesh.uv = uv;
        m_mesh.tangents = tangents;

        // Triangles
        // 사각형 하나당 Index 6개가 필요하므로 (xSize*ySize) * 6
        var triangles = new int[xSize * ySize * 6];

        // Y축 루프 : ySize만큼 반복하며 반복할때마다 1줄의 사각형들 생성
        // ti : Triangle Index - 현재 사각형의 데이터를 기록할 시작 위치
        //      사각형 하나당 6개의 데이터가 필요하므로 6씩 증가
        // vi : Vertex Index - 정점 배열의 인덱스를 가리킴
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                // vi + xSize + 1 (ti+1, ti+4)--- vi + xSize + 2 (ti+5)
                // | \                                  |
                // |                \                   |
                // |                                  \ |
                // vi(ti)--------------- vi + 1 (ti+2, ti+3)
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;

                //await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
                m_mesh.triangles = triangles;
            }
        }

        // Normals
        m_mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (m_vertices == null) return;

        Gizmos.color = Color.black;
        for (int i = 0; i < m_vertices.Length; i++)
        {
            Gizmos.DrawSphere(m_vertices[i], 0.1f);
        }
    }
}
