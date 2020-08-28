using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System.Threading;
using System.Runtime.CompilerServices;
using UnityEngine.SocialPlatforms;
using System.Linq;

public enum FilterType
{
    basefilter,
    landfilter,
    terracefilter,
    heightoffsetfilter
}
[System.Serializable]
public struct FilterData
{
    public float scale;
    public int octaves;
    public float persistence;
    public float lacunarity;
    public float strength;

    public float xOffset;
    public float yOffset;

    public FilterType FilterType;

    [Range(0, 1)]
    public float landMin;
    [Range(0, 1)]
    public float landMax;

    public int terraceCount;

    public float levelOffset;

    [Range(0, 1)]
    public float biomeThreshold;

    public float GetMaxHeight()
    {
        switch (FilterType)
        {
            case FilterType.basefilter:
            case FilterType.landfilter:
            case FilterType.terracefilter:
                return scale;
            case FilterType.heightoffsetfilter:
                return levelOffset;
            default:
                break;
        }
        return 0;
    }
};

[ExecuteInEditMode]
public class TerrainGenerator : Singleton<TerrainGenerator>
{
    [SerializeField, Header("terrain generator shader variables")]
    ComputeShader _generatorCS;
    [SerializeField]
    FilterData[] prefilters;
    [SerializeField]
    FilterData[] biomeFilters;
    [SerializeField]
    FilterData[] postfilters;
    [SerializeField]
    bool _loadedPlane = false;

    [SerializeField, Header("color shader variables")]
    Material _terrainMaterial;
    [SerializeField]
    float[] _heights;
    [SerializeField]
    Color[] _heightColors;

    public float MaxHeight
    {
        get;
        private set;
    }

    [SerializeField]
    bool testPerformance = false;
    void Update()
    {
        _terrainMaterial.SetInt("colorCount", _heights.Length);
        _terrainMaterial.SetFloatArray("heigths", _heights);
        _terrainMaterial.SetColorArray("colors", _heightColors);
    }

    private void OnValidate()
    {
        UpdateMaxHeight();

        //filter data
        ComputeBuffer prefilterBuffer = new ComputeBuffer(Mathf.Max(prefilters.Length, 1), 52);
        if (prefilters.Length > 0)
            prefilterBuffer.SetData(prefilters);
        _generatorCS.SetBuffer(kernel, "preFilters", prefilterBuffer);

        ComputeBuffer biomefilterBuffer = new ComputeBuffer(Mathf.Max(biomeFilters.Length, 1), 52);
        if (biomeFilters.Length > 0)
            biomefilterBuffer.SetData(biomeFilters);
        _generatorCS.SetBuffer(kernel, "biomeFilters", biomefilterBuffer);

        ComputeBuffer postfilterBuffer = new ComputeBuffer(Mathf.Max(postfilters.Length, 1), 52);
        if (postfilters.Length > 0)
            postfilterBuffer.SetData(postfilters);
        _generatorCS.SetBuffer(kernel, "postFilters", postfilterBuffer);
    }
    void UpdateMaxHeight()
    {
        MaxHeight = 0;

        foreach (var filter in prefilters)
            MaxHeight += filter.GetMaxHeight();

        float maxBiomeHeight = 0;
        foreach (var biome in biomeFilters)
        {
            float biomeHeight = biome.GetMaxHeight();
            if (biomeHeight > maxBiomeHeight)
                maxBiomeHeight = biomeHeight;
        }
        MaxHeight += maxBiomeHeight;
    }

    int kernel = -1;
    ComputeBuffer _vertexBuffer;
    ComputeBuffer _heightBuffer;
    Vector3[] _vertices;

    public float[] GenerateTerrainCompute(Matrix4x4 localToWorld)
    {
        if (kernel == -1)
            kernel = _generatorCS.FindKernel("CalculateHeight");

        float[] heightData;
        if (!_loadedPlane || _heightBuffer == null || _vertexBuffer == null)
        {
            _loadedPlane = true;

            int stepcount;
            var plane = PlaneGenerator.Instance.GeneratePlane(1, out stepcount);
            _vertices = plane.vertices;

            _vertexBuffer = new ComputeBuffer(_vertices.Length, 12);
            _vertexBuffer.SetData(_vertices);
            _generatorCS.SetBuffer(kernel, "vertices", _vertexBuffer);

            heightData = new float[_vertices.Length];
            _heightBuffer = new ComputeBuffer(_vertices.Length, 4);
            _heightBuffer.SetData(heightData);
            _generatorCS.SetBuffer(kernel, "heightMap", _heightBuffer);
        }
        else
            heightData = new float[_vertices.Length];

        _generatorCS.SetMatrix("localToWorld", localToWorld);
        _generatorCS.SetFloat("maxHeight", MaxHeight);
        _generatorCS.Dispatch(kernel, _vertices.Length / 4 + 1, 1, 1);

        _heightBuffer.GetData(heightData);
        //vertexbuffer.GetData(vertices);

        return heightData;
    }
}
