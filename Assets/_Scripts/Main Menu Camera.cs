using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Header("References")]
    public Transform pivot;

    [Header("Parameters")]
    public float pivotSpeed;

    // Update is called once per frame
    void Update() {
        transform.LookAt(pivot);
        transform.RotateAround(pivot.position, pivot.up, Time.deltaTime * pivotSpeed);
    }
}
