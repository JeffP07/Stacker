using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;
    public float mouseSensitivity = 100;

    public int difficulty { get; set; }
    public int gameMode { get; set; }

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
        difficulty = 0;
        gameMode = 0;
    }
}
