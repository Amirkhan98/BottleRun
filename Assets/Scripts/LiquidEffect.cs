using System;
using PathologicalGames;
using UnityEngine;

public class LiquidEffect : MonoBehaviour
{
    private SpawnPool shapesPool;
    [SerializeField] private int spawnAmount;
    private string poolName = "Liquid";
    [SerializeField] private GameObject circleSprite;
    public static Action<Vector3> onParticleHit;

    private void Start()
    {
        onParticleHit += OnParticleHit;
    }

    private void OnParticleHit(Vector3 position)
    {
        position += new Vector3(0, 0.01f, 0);
        SpawnPool shapesPool = PoolManager.Pools[poolName];
        Transform inst;

        inst = shapesPool.Spawn(circleSprite);
        inst.localPosition = position;
        if (shapesPool.Count > spawnAmount)
        {
            shapesPool.Despawn(shapesPool[0]);
        }
    }
}