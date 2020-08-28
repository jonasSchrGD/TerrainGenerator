using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

[CreateAssetMenu(menuName = "Filters/LandFilter.asset")]
public class LandFilter : BaseFilter
{
    [SerializeField, Header("filter specific settings")]
    float _xOffset = 0;
    [SerializeField]
    float _yOffset = 0;
    [SerializeField]
    AnimationCurve _heightRemapping;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float GetNoiseValue(Vector3 pos)
    {
        float perlin = Noise.PerlinNoise(_xOffset + pos.x, _yOffset + pos.z, _scale, _octaves, _persistence, _lacunarity);

        return _strength * _heightRemapping.Evaluate(perlin);
    }

    public override void SetShaderVariables(ref ComputeShader shader, ref ComputeBuffer vertexbuffer)
    {
        if (kernel == -1)
            kernel = shader.FindKernel("LandFilter");
        
        base.SetShaderVariables(ref shader, ref vertexbuffer);
        shader.SetFloat("xOffset", _xOffset);
        shader.SetFloat("yOffset", _yOffset);
    }
    public override void Dispatch(ref ComputeShader shader, int vertexCount)
    {
        shader.Dispatch(kernel, vertexCount / 4, 1, 1);
    }
}
