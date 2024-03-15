using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject blockPrefab;

    public void spawnBlock() {
        Instantiate(blockPrefab, transform.position + transform.up * 0.25f, Quaternion.identity);
    }
}
