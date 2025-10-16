using System;
using UnityEngine;

public class RunData : MonoBehaviour
{
    public static string RunId { get; private set; }
    public static int Seed { get; private set; }
    private void Awake()
    {
        RunId = Guid.NewGuid().ToString("N").Substring(0, 8);
        Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(Seed);   
        Debug.Log($"Run ID: {RunId}, Seed: {Seed}");
    }
}
