using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour {

    public int numCollisions { get; set; } = 0;
    public int numColliders { get; set; } = 0;

    [Header("References")]
    public List<GameObject> buildingsEasy = new List<GameObject>();
    public List<GameObject> buildingsHard = new List<GameObject>();
    public GameObject stackEasy;
    public GameObject stackHard;
    public TextMeshPro timer;

    const int DIFFICULTY_EASY = 0;
    const int DIFFICULTY_HARD = 1;
    const int GAMEMODE_HEIGHT = 0;
    const int GAMEMODE_BUILD = 1;

    public float winPercentage;
    public float timeToWin;
    private float timeLeft;
    private Transform player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timeLeft = timeToWin;
        LoadObjective();
    }

    private void Update() {
        Transform held = player.GetComponent<PlayerController>().heldObject;
        if (((float)numCollisions / numColliders) >= winPercentage) {
            if (held != null) {
                if (!held.name.Contains("Stacking Block")) {
                    timeLeft -= Time.deltaTime;
                }
                else {
                    timeLeft = timeToWin;
                }
            }
            else {
                timeLeft -= Time.deltaTime;
            }
        }
        else {
            timeLeft = timeToWin;
        }

        if (timeLeft <= 0) {
            Debug.Log("WIN");
        }
        //Debug.Log(timeLeft);
        timer.text = (timeLeft<=0) ? "Winner!" : timeLeft.ToString("#.00");
    }

    public void LoadObjective() {
        int difficulty = GameManager.Instance.difficulty;
        int gameMode = GameManager.Instance.gameMode;
        if (gameMode == GAMEMODE_HEIGHT) {
            if (difficulty == DIFFICULTY_EASY) {
                stackEasy.SetActive(true);
            }
            else if (difficulty == DIFFICULTY_HARD) {
                stackHard.SetActive(true);
            }
        }
        else if (gameMode == GAMEMODE_BUILD) {
            if (difficulty == DIFFICULTY_EASY) {
                buildingsEasy[Random.Range(0, buildingsEasy.Count - 1)].SetActive(true);
            }
            else if(difficulty == DIFFICULTY_HARD) {
                buildingsHard[Random.Range(0, buildingsHard.Count - 1)].SetActive(true);
            }
        }
    }
}
