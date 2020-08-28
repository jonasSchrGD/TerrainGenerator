using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Filters/BaseFilter.asset")]
public class BaseFilter : ScriptableObject
{
    [SerializeField, Header("base settings")]
    protected float _scale = 1;
    [SerializeField]
    protected int _octaves = 1;
    [SerializeField]
    protected float _persistence = 1;
    [SerializeField]
    protected float _lacunarity = 1;
    [SerializeField]
    protected float _strength = 1;
    
    public virtual float GetMaxHeigth()
    {
        return _strength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual float GetNoiseValue(Vector3 pos)
    {
        return _strength * Noise.PerlinNoise(pos.x, pos.z, _scale, _octaves, _persistence, _lacunarity);
    }

    protected int kernel = -1;
    public virtual void SetShaderVariables(ref ComputeShader shader, ref ComputeBuffer vertexbuffer)
    {
        if (kernel == -1)
            kernel = shader.FindKernel("PerlinFilter");

        shader.SetBuffer(kernel, "vertices", vertexbuffer);
        shader.SetFloat("scale", _scale);
        shader.SetInt("octaves", _octaves);
        shader.SetFloat("persistence", _persistence);
        shader.SetFloat("lacunarity", _lacunarity);
        shader.SetFloat("strength", _strength);
    }
    public virtual void Dispatch(ref ComputeShader shader, int vertexCount)
    {
        shader.Dispatch(kernel, vertexCount / 4, 1, 1);
    }
}
