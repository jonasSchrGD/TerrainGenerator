using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneGenerator : Singleton<PlaneGenerator>
{
    [SerializeField]
    float _size = 100;
    public float Size => _size;

    [SerializeField]
    float _planeResPow = 1;
    public float PlaneResPow => _planeResPow;

    [SerializeField]
    int _stepSizePow = 4;
    public int StepSizePow => _stepSizePow;

    int _maxStepCount = 241;
    public int MaxStepCount => _maxStepCount;

    Dictionary<int, Mesh> _generatedPlanes = new Dictionary<int, Mesh>();
    public Mesh GeneratePlane(int resStepSize, out int stepCount)
    {
        stepCount = (_maxStepCount - 1) / resStepSize + 1;

        Mesh generatedMesh = new Mesh();
        if (_generatedPlanes.ContainsKey(resStepSize))
            return Instantiate(_generatedPlanes[resStepSize]);

        float stepSize = _size / (_maxStepCount - 1);

        //generated vertices
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> UVs = new List<Vector2>();
        for (int i = 0; i < _maxStepCount; i += resStepSize)
        {
            for (int j = 0; j < _maxStepCount; j += resStepSize)
            {
                vertices.Add(new Vector3(-_size / 2 + j * stepSize, 0, -_size / 2 + i * stepSize));
                UVs.Add(new Vector2((j * stepSize) / _size, (i * stepSize) / _size));
            }
        }
        generatedMesh.SetVertices(vertices);
        generatedMesh.SetUVs(0, UVs);

        //generate indices
        List<int> indices = new List<int>();

        for (int i = 0; i < stepCount - 1; i++)
        {
            for (int j = 0; j < stepCount - 1; j++)
            {
                int startidx = i * stepCount + j;

                //triangle 1
                indices.Add(startidx);
                indices.Add(startidx + stepCount);
                indices.Add(startidx + stepCount + 1);

                //triangle 2
                indices.Add(startidx);
                indices.Add(startidx + stepCount + 1);
                indices.Add(startidx + 1);
            }
        }
        generatedMesh.SetIndices(indices, MeshTopology.Triangles, 0);
        _generatedPlanes.Add(resStepSize, generatedMesh);

        return generatedMesh;
    }
}
