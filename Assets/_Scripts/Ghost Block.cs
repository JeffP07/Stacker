using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBlock : MonoBehaviour {
    BoxCollider[] boxColliders;
    GameManager gameManager;
    int numCollisions = 0;

    // Start is called before the first frame update
    void Start() {
        gameManager = GameManager.Instance;
        gameManager.NumColliders++;
    }

    private void OnTriggerEnter(Collider other) {
        GameManager gm = GameManager.Instance;
        if (numCollisions == 0) {
            gm.NumCollisions++;
        }
        numCollisions++;
    }

    private void OnTriggerExit(Collider other) {
        GameManager gm = GameManager.Instance;
        numCollisions--;
        if (numCollisions == 0) {
            gm.NumCollisions--;
        }
    }
}
