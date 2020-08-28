using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

[CreateAssetMenu(menuName = "Filters/WaterLevelFilter.asset")]
public class WaterLevelFilter : BaseFilter
{
    [SerializeField, Header("filter specific settings")]
    float _waterLevel = 0.75f;

    public override float GetMaxHeigth()
    {
        return -_waterLevel;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float GetNoiseValue(Vector3 pos)
    {
        return Mathf.Clamp(pos.y - _waterLevel, 0, float.MaxValue);
    }

    public override void SetShaderVariables(ref ComputeShader shader, ref ComputeBuffer vertexbuffer)
    {
        if (kernel == -1)
            kernel = shader.FindKernel("HeightOffsetFilter");

        base.SetShaderVariables(ref shader, ref vertexbuffer);
        shader.SetFloat("heightOffset", _waterLevel);
    }
    public override void Dispatch(ref ComputeShader shader, int vertexCount)
    {
        shader.Dispatch(kernel, vertexCount / 4, 1, 1);
    }
}
