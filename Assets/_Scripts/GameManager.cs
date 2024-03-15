using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;

    public int NumCollisions {  get; set; }
    public int NumColliders { get; set; }

    public static GameManager Instance {
        get {
            if (_instance == null) {
                Debug.LogError("game manager error");
            }
            return _instance;
        }
    }

    private void Awake() {
        if (_instance) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }

        DontDestroyOnLoad(this);
        NumCollisions = 0;
    }
}
