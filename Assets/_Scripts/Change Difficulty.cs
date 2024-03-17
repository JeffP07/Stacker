using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDifficulty : MonoBehaviour {

    public void Start() {
        transform.GetComponent<TMP_Dropdown>().value = GameManager.Instance.difficulty;
    }

    public void changeDifficulty(int difficulty) {
        GameManager.Instance.difficulty = difficulty;
    }
}
