using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject blockPrefab;
    public Transform spawnPoint;
    public GameObject spawnHint;

    public void Update() {
        if (spawnHint.activeSelf) {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            Vector3 hintLook = spawnHint.transform.position - player.position;
            hintLook.y = 0;
            spawnHint.transform.rotation = Quaternion.LookRotation(hintLook, spawnHint.transform.up);
        }
    }

    public void spawnBlock() {
        Instantiate(blockPrefab, spawnPoint.position, Quaternion.identity);
        spawnHint.SetActive(false);
    }
}
