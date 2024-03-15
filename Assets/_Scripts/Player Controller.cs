using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    public Rigidbody player;
    public Transform head;
    public Camera cam;
    public Image crosshair;
    public Sprite dotCursor;
    public Sprite pickupCursor;
    public GameObject cat;

    [Header("Configuration")]
    public float walkSpeed;
    public float mouseLookSensitivity;

    [Header("Player Vars")]
    bool isCrouching = false;

    [Header("Held Item Vars")]
    public float itemPickupDistance;
    Transform heldObject = null;
    public float itemHoldDistance = 1f;
    public float heldObjectMoveForce;
    public float heldObjectDrag;
    public float throwForce;

    private float horizLook;
    private float vertLook;
    private RaycastHit hit;
    private bool cast;
    private MouseScript mouseScript;

    // Start is called before the first frame update
    void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        horizLook = transform.eulerAngles.y;
        vertLook = head.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update() {
        // mouse look
        horizLook += Input.GetAxis("Mouse X") * mouseLookSensitivity * Time.deltaTime;
        vertLook -= Input.GetAxis("Mouse Y") * mouseLookSensitivity * Time.deltaTime;
        vertLook = Mathf.Clamp(vertLook, -85f, 85f);
        head.localRotation = Quaternion.Euler(vertLook, horizLook, 0);

        // Movement
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            isCrouching = !isCrouching;
            if (isCrouching) { 
                head.Translate(Vector3.down * 0.6f, Space.World);
            }
            else { 
                head.Translate(Vector3.up * 0.6f, Space.World);
            }
        }

        // interact with objects
        cast = Physics.Raycast(head.position, head.forward, out hit, itemPickupDistance, -1 & ~(1 << LayerMask.NameToLayer("Ghost Blocks")));

        if (Input.GetKeyDown(KeyCode.F)) {
            if (heldObject != null) { // Drop item
                heldObject.SetParent(null);
                if (heldObject.TryGetComponent(out mouseScript)) {
                    mouseScript.setThrown(true);
                }
                heldObject.GetComponent<Rigidbody>().useGravity = true;
                heldObject.GetComponent<Rigidbody>().drag = 0;
                heldObject.GetComponent<Rigidbody>().angularDrag = 0;
                heldObject = null;
            }
            else {
                if (cast) { // Pick up item
                    if (hit.transform.CompareTag("Pickup")) {
                        heldObject = hit.transform;
                        heldObject.SetParent(transform);
                        itemHoldDistance = hit.distance;
                        heldObject.GetComponent<Rigidbody>().useGravity = false;
                        heldObject.GetComponent<Rigidbody>().drag = heldObjectDrag;
                        heldObject.GetComponent<Rigidbody>().angularDrag = heldObjectDrag;
                        if (heldObject.TryGetComponent(out mouseScript)) {
                            cat.GetComponent<WanderingAI>().SetAction("wander");
                            cat.GetComponent<WanderingAI>().StopPlay();
                            mouseScript.setThrown(false);
                        }
                    }
                    if (hit.transform.CompareTag("Spawner")) {
                        hit.transform.gameObject.GetComponent<BlockSpawner>().spawnBlock();
                    }
                }
            }
        }

        // throw object
        if (Input.GetKeyDown(KeyCode.G)) {
            if (heldObject != null) {
                heldObject.SetParent(null);
                if (heldObject.TryGetComponent(out mouseScript)) {
                    mouseScript.setThrown(true);
                }
                heldObject.GetComponent<Rigidbody>().useGravity = true;
                heldObject.GetComponent<Rigidbody>().drag = 0;
                heldObject.GetComponent<Rigidbody>().angularDrag = 0;
                heldObject.GetComponent<Rigidbody>().AddForce(head.forward * throwForce);
                heldObject = null;
            }
        }

        // keep held object in front of player
        if (heldObject != null) {
            Vector3 moveVector = ((head.position + head.forward * itemHoldDistance) - heldObject.transform.position);
            heldObject.GetComponent<Rigidbody>().AddForce(moveVector * heldObjectMoveForce);
        }
    }

    private void FixedUpdate() {
        Vector3 forward = head.forward;
        Vector3 right = head.right;
        forward.y = 0;
        right.y = 0;
        transform.position += (forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal")) * Time.deltaTime * walkSpeed;
    }

    private void LateUpdate() {
        // update cursor if can interact
        crosshair.sprite = dotCursor;
        if (cast) {
            if (hit.transform.CompareTag("Pickup")) {
                crosshair.sprite = pickupCursor;
            }
            else if (hit.transform.CompareTag("Spawner")) {
                crosshair.sprite = pickupCursor;
            }
        }

        // if holding object
        if (heldObject != null) {
            if (Input.GetKey(KeyCode.Q)) {
                heldObject.Rotate(transform.up * -0.5f, Space.World);
            }
            else if (Input.GetKey(KeyCode.E)) {
                heldObject.Rotate(transform.up * 0.5f, Space.World);
            }

            // mouse wheel scroll
            if (!Input.GetKey(KeyCode.LeftShift)) {
                heldObject.Rotate(head.right * Input.mouseScrollDelta.y * 10f, Space.World);
            }
            if (Input.GetKey(KeyCode.LeftShift) && Input.mouseScrollDelta.y != 0) {
                if ((itemHoldDistance + Input.mouseScrollDelta.y * 0.1f) < 0.5) {
                    itemHoldDistance = 0.5f;
                }
                else if ((itemHoldDistance + Input.mouseScrollDelta.y * 0.1f) > 2) {
                    itemHoldDistance = 2f;
                }
                else {
                    itemHoldDistance += Input.mouseScrollDelta.y * 0.1f;
                }
            }
        }
    }
}
