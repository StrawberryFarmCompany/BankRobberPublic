using System.Collections.Generic;
using UnityEngine;

public static class NoiseManager
{
    private static List<Noise> activeNoises = new List<Noise>();

    // 소음 발생
    public static void CreateNoise(Vector3 pos, float radius)
    {
        activeNoises.Add(new Noise(pos, radius));
    }

    // 모든 소음 확인
    public static List<Noise> GetActiveNoises()
    {
        return activeNoises;
    }

    // 턴이 끝나면 소음 제거
    public static void ClearNoises()
    {
        activeNoises.Clear();
    }
}

public struct Noise
{
    public Vector3 pos;
    public float radius;

    public Noise(Vector3 pos, float radius)
    {
        this.pos = pos;
        this.radius = radius;
    }
}
