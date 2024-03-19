using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour {
    public bool isThrown;
    public GameObject cat;


    // Start is called before the first frame update
    void Start() {
        isThrown = false;
        cat = GameObject.FindGameObjectWithTag("Cat");
    }

    // Update is called once per frame
    void Update() {
        if (transform.position.y < -0.1f) {
            transform.position = new Vector3(transform.position.x, 0.05f, transform.position.z);
            
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (isThrown && collision.collider.CompareTag("Floor")) {
            cat.GetComponent<WanderingAI>().SetToyTarget(transform.gameObject);
            cat.GetComponent<WanderingAI>().SetAction("findtoy");
        }
    }

    public void setThrown(bool b) {
        isThrown = b;
    }
}
