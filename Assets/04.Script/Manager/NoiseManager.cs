using System.Collections.Generic;
using UnityEngine;

public enum NoiseType
{
    None,
    Move,
    Disarm,       // 장치 해제
    Trigger,      // 격발
    ThrowCoin,    // 투척
    Ambush,       // 암습
    Unstealth,    // 은신 해제
    Steal         // 훔치기
}

public static class NoiseManager
{
    private static List<Noise> activeNoises = new List<Noise>();

    // 소음 발생
    public static void AddNoise(Vector3Int pos, NoiseType type, bool isSuccess = true)
    {
        int radius = GetNoiseRadius(type, isSuccess);
        if (radius <= 0f) return; // 소음이 없는 경우

        activeNoises.Add(new Noise(pos, radius, type));
        Debug.Log($"[NoiseManager] {type} 소음 발생 ({radius}칸) at {pos}");
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

    private static int GetNoiseRadius(NoiseType type, bool isSuccess)
    {
        switch (type)
        {
            case NoiseType.Move:
                return 5; // 이동 시 9*9 소음 발생 (캐릭터 위치 미포함 기준 모든 방면으로 4칸)
            case NoiseType.Disarm:
                return isSuccess ? 2 : 6;
            case NoiseType.Trigger:
                return isSuccess ? 4 : 8;   // 쏠 때 격발한 엔티티 노드 미포함 기준 3*3 소음 발생
            case NoiseType.ThrowCoin:
                return 5;
            case NoiseType.Ambush:
                return 2;
            case NoiseType.Unstealth:
                return 8;
            case NoiseType.Steal:
                return isSuccess ? 0 : 5;
            default:
                return 0;
        }
    }
}

public struct Noise
{
    public Vector3Int pos;
    public int radius;
    public NoiseType type;

    public Noise(Vector3Int pos, int radius, NoiseType type)
    {
        this.pos = pos;
        this.radius = radius;
        this.type = type;
    }
}
