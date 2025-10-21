using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationGrid : MonoBehaviour
{
    public Transform prefab;
    public int gridResolution = 10;

    private Transform[] m_grid;
    private List<Transformation> m_transformations;
    private Matrix4x4 m_transformation;

    private void Awake()
    {
        m_transformations = new();
        m_grid = new Transform[gridResolution * gridResolution * gridResolution];
        for (int i = 0, z = 0; z < gridResolution; z++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                for (int x = 0; x < gridResolution; x++, i++)
                {
                    m_grid[i] = CreateGridPoint(x, y, z);
                }
            }
        }
    }

    private void Update()
    {
        UpdateTransformation();
        for (int i =0, z=0; z < gridResolution; z++)
        {
            for(int y = 0;y < gridResolution; y++)
            {
                for(int x = 0;x < gridResolution;x++, i++)
                {
                    m_grid[i].localPosition = TransformPoint(x, y, z);
                }
            }
        }
    }

    private void UpdateTransformation()
    {
        GetComponents<Transformation>(m_transformations);

        if(m_transformations.Count > 0)
        {
            m_transformation = m_transformations[0].Matrix;
            for(int i =0;i < m_transformations.Count;i++)
            {
                m_transformation = m_transformations[i].Matrix * m_transformation;
            }
        }
    }

    private Vector3 TransformPoint(int x, int y, int z)
    {
        Vector3 coordinates = GetCoordinates(x, y, z);
        return m_transformation.MultiplyPoint(coordinates);
    }

    private Transform CreateGridPoint(int x, int y, int z)
    {
        Transform point = Instantiate<Transform>(prefab);
        point.localPosition = GetCoordinates(x, y, z);
        point.GetComponent<MeshRenderer>().material.color = new Color(
            (float)x / gridResolution,
            (float)y / gridResolution,
            (float)z / gridResolution
            );
        return point;
    }

    private Vector3 GetCoordinates(int x, int y, int z)
    {
        return new Vector3
        (
            x - (gridResolution - 1) * 0.5f,
            y - (gridResolution - 1) * 0.5f,
            z - (gridResolution - 1) * 0.5f
        );
    }
}
