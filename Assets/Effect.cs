using System;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

public class Effect : MonoBehaviour
{
    private SpawnPool shapesPool;
    [SerializeField] private int spawnAmount = 20;
    private string poolName = "Liquid";
    [SerializeField] private GameObject circleSprite;
    [SerializeField] private float spawnInterval;

    private void Start()
    {
        shapesPool = PoolManager.Pools[poolName];
    }

    private void OnEnable()
    {
        StartCoroutine("Spawner");
    }

    private IEnumerator Spawner()
    {
        int count = spawnAmount;
        Transform inst;
        SpawnPool shapesPool = PoolManager.Pools[poolName];
        while (count > 0)
        {
            // Spawn in a line, just for fun
            inst = shapesPool.Spawn(circleSprite);
            inst.localPosition = new Vector3((spawnAmount + 2) - count, 0, 0);
            count--;

            yield return new WaitForSeconds(spawnInterval);
        }

        StartCoroutine(Despawner());

        yield return null;
    }


    private IEnumerator Despawner()
    {
        var spawnedCopy = new List<Transform>(shapesPool);
        Debug.Log(shapesPool.ToString());
        foreach (Transform instance in spawnedCopy)
        {
            shapesPool.Despawn(instance); // Internal count--

            yield return new WaitForSeconds(spawnInterval);
        }

        // Restart
        StartCoroutine(Spawner());

        yield return null;
    }
}