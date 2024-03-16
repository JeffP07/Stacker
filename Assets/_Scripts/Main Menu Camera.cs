using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Header("References")]
    public Transform pivot;

    [Header("Parameters")]
    public float pivotSpeed;

    // Start is called before the first frame update
    void Start() {
        //pivot = transform.GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void Update() {
        transform.LookAt(pivot);
        transform.RotateAround(pivot.position, pivot.up, Time.deltaTime * pivotSpeed);
    }
}
