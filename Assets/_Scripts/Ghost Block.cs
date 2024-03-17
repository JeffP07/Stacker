using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class GhostBlock : MonoBehaviour {
    int numCollisions = 0;
    ObjectiveManager om;

    public void Start() {
        om = FindAnyObjectByType<ObjectiveManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if (numCollisions == 0) {
            om.numCollisions++;
        }
        numCollisions++;
    }

    private void OnTriggerExit(Collider other) {
        numCollisions--;
        if (numCollisions == 0) {
            om.numCollisions--;
        }
    }
}
