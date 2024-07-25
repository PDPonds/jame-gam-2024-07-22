using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public SpawnerSlot[] spawnerSlots;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < spawnerSlots.Length; i++)
            {
                GameObject go = Instantiate(spawnerSlots[i].enemyPrefab, spawnerSlots[i].pos.position, Quaternion.identity);
                EnemyController eCon = go.GetComponent<EnemyController>();
                eCon.OnSpawn();
            }

            Destroy(gameObject);

        }
    }
}

[Serializable]
public class SpawnerSlot
{
    public GameObject enemyPrefab;
    public Transform pos;
}

