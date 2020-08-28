using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    //reference:
    //https://www.youtube.com/watch?v=MRNFcywkUSA
    public static float PerlinNoise(float x, float y, float scale, int octaves = 1, float persistence = 1, float lacunarity = 1)
    {
        float maxvalue = 0;

        float noise = 0;
        float amplitude = 1;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            float xSample = x / scale * frequency;
            float ySample = y / scale * frequency;

            float value = Mathf.PerlinNoise(xSample, ySample) * 2 - 1;
            noise += value * amplitude;
            maxvalue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return Mathf.InverseLerp(-maxvalue, maxvalue, noise);
    }
    public static float PerlinNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }
}
