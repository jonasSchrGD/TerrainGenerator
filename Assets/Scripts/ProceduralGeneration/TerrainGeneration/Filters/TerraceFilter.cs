using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

[CreateAssetMenu(menuName = "Filters/TerraceSFilter.asset")]
public class TerraceFilter : BaseFilter
{
    [SerializeField, Header("filter specific settings")]
    int _teraceCount = 15;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float GetNoiseValue(Vector3 pos)
    {
        float terraceCount = _teraceCount / (TerrainGenerator.Instance.MaxHeight / 10);
        return Mathf.Round(pos.y * terraceCount) / terraceCount;
    }

    public override void SetShaderVariables(ref ComputeShader shader, ref ComputeBuffer vertexbuffer)
    {
        if (kernel == -1)
            kernel = shader.FindKernel("TerraceFilter");

        base.SetShaderVariables(ref shader, ref vertexbuffer);
        shader.SetInt("terraceCount", _teraceCount);
    }
    public override void Dispatch(ref ComputeShader shader, int vertexCount)
    {
        shader.Dispatch(kernel, vertexCount / 4, 1, 1);
    }
}
