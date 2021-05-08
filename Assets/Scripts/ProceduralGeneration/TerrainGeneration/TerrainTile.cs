using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class TerrainTile : MonoBehaviour
{
    int _lastStepSize = 0;
    int _currentStepCount = 1;

#if UNITY_EDITOR
    [SerializeField]
    bool _clearHeightData = false;
    [SerializeField]
    bool _calculateHeightPerFrame = false;
#endif

    MeshFilter _meshFilter;
    MeshCollider _meshCollider;

    void Awake()
    {
        _kernel = -1;
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();

        _clearHeightData = true;
        LoadTerrainData();
    }
    void OnEnable()
    {
        Awake();
    }

    void Update()
    {
#if UNITY_EDITOR
        if ((_clearHeightData || _calculateHeightPerFrame) && !Application.isPlaying)
            LoadTerrainData();
#endif
        var planegen = PlaneGenerator.Instance;
        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        int stepMagn = Mathf.RoundToInt(planegen.StepSizePow * distance / (planegen.PlaneResPow * planegen.Size));
        int stepSize = Mathf.ClosestPowerOfTwo(stepMagn);

#if UNITY_EDITOR
        if (_lastStepSize != stepSize || (_clearHeightData || _calculateHeightPerFrame) && !Application.isPlaying)
#else
        if (_lastStepSize != stepSize)
#endif
        {
            _lastStepSize = Mathf.Clamp(stepSize, 1, 16);
            UpdateTerrain(PlaneGenerator.Instance.GeneratePlane(_lastStepSize, out _currentStepCount));
        }
    }

    int _kernel = -1;
    ComputeBuffer _heightBuffer;
    void LoadTerrainData()
    {
        if (_kernel == -1)
            _kernel = _computeShader.FindKernel("CSMain");

        if (!_highResMesh || _clearHeightData)
        {
            Debug.Log("created high res mesh");
            int stepCount;
            _highResMesh = PlaneGenerator.Instance.GeneratePlane(1, out stepCount);
            _highResMesh.RecalculateNormals();
            _meshFilter.mesh = _highResMesh;

            _clearHeightData = false;
        }

        _computeShader = Instantiate(_computeShader);
        _computeShader.name = "TileHeightShader";

        float[] heightmap = TerrainGenerator.Instance.GenerateTerrainCompute(transform.localToWorldMatrix);
        if (_heightBuffer == null || (_heightBuffer.IsValid() && _heightBuffer.count != heightmap.Length))
            _heightBuffer = new ComputeBuffer(heightmap.Length, 4);
        _heightBuffer.SetData(heightmap);
        _computeShader.SetBuffer(_kernel, "heightMap", _heightBuffer);
    }
    void UpdateTerrain(Mesh mesh)
    {
        if (!_meshFilter)
            _meshFilter = GetComponent<MeshFilter>();
        if (!_meshCollider)
            _meshCollider = GetComponent<MeshCollider>();

        var vertices = mesh.vertices;
        UpdateHeight(ref vertices);
        mesh.SetVertices(vertices);
        mesh.RecalculateNormals();
        _meshFilter.mesh = mesh;
    }

    [SerializeField]
    ComputeShader _computeShader;
    Mesh _highResMesh;

    void UpdateHeight(ref Vector3[] vertices)
    {
        ComputeBuffer buffer = new ComputeBuffer(vertices.Length, 12);
        buffer.SetData(vertices);
        _computeShader.SetBuffer(_kernel, "vertices", buffer);

        _computeShader.SetInt("stepSize", _lastStepSize);
        _computeShader.SetInt("steps", _currentStepCount);
        _computeShader.SetInt("maxSteps", PlaneGenerator.Instance.MaxStepCount);
        _computeShader.SetFloat("maxHeight", TerrainGenerator.Instance.MaxHeight);
        _computeShader.Dispatch(_kernel, vertices.Length / 4 + 1, 1, 1);

        buffer.GetData(vertices);
        buffer.Dispose();
    }
}
