using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeGamemode : MonoBehaviour {
    public void Start() {
        transform.GetComponent<TMP_Dropdown>().value = GameManager.Instance.gameMode;
    }

    public void changeDifficulty(int gameMode) {
        GameManager.Instance.gameMode = gameMode;
    }
}
