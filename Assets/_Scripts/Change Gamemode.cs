using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeGamemode : MonoBehaviour {
    [Header("References")]
    public GameObject structureDesc;
    public GameObject heightDesc;

    public void Start() {
        transform.GetComponent<TMP_Dropdown>().value = GameManager.Instance.gameMode;
        if (GameManager.Instance.gameMode == 0) {
            heightDesc.SetActive(true);
            structureDesc.SetActive(false);
        }
        else if (GameManager.Instance.gameMode == 1) {
            heightDesc.SetActive(false);
            structureDesc.SetActive(true);
        }
    }

    public void changeDifficulty(int gameMode) {
        GameManager.Instance.gameMode = gameMode;
        if (gameMode == 0) {
            heightDesc.SetActive(true);
            structureDesc.SetActive(false);
        }
        else if(gameMode == 1) {
            heightDesc.SetActive(false);
            structureDesc.SetActive(true);
        }
    }
}
