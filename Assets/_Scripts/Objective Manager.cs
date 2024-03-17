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
    public GameObject fireworks;

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
            fireworks.SetActive(true);
        }
        timer.text = (timeLeft<=0) ? "Winner!" : (timeLeft.ToString("0.00") + "s");
    }

    public void LoadObjective() {
        int difficulty = GameManager.Instance.difficulty;
        int gameMode = GameManager.Instance.gameMode;
        if (gameMode == GAMEMODE_HEIGHT) {
            if (difficulty == DIFFICULTY_EASY) {
                stackEasy.SetActive(true);
                numColliders = stackEasy.GetComponentsInChildren<Collider>().Length;
            }
            else if (difficulty == DIFFICULTY_HARD) {
                stackHard.SetActive(true);
                numColliders = stackHard.GetComponentsInChildren<Collider>().Length;
            }
        }
        else if (gameMode == GAMEMODE_BUILD) {
            if (difficulty == DIFFICULTY_EASY) {
                int rand = Random.Range(0, buildingsEasy.Count - 1);
                buildingsEasy[rand].SetActive(true);
                numColliders = buildingsEasy[rand].GetComponentsInChildren<Collider>().Length;
            }
            else if(difficulty == DIFFICULTY_HARD) {
                int rand = Random.Range(0, buildingsHard.Count - 1);
                buildingsHard[rand].SetActive(true);
                numColliders = buildingsHard[rand].GetComponentsInChildren<Collider>().Length;
            }
        }
    }
}
