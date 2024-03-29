using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    public CharacterController characterController;
    public Rigidbody player;
    public Transform head;
    public Camera cam;
    public Image crosshair;
    public Sprite dotCursor;
    public Sprite pickupCursor;
    public GameObject cat;

    [Header("Configuration")]
    public float walkSpeed;
    public float gravity;
    private float mouseLookSensitivity;

    [Header("Player Vars")]
    bool isCrouching = false;

    [Header("Held Item Vars")]
    public float itemPickupDistance;
    public Transform heldObject = null;
    public float itemHoldDistance;
    public float heldObjectMoveForce;
    public float heldObjectDrag;
    public float throwForce;

    [Header("Pause Menu")]
    public GameObject escapeMenu;
    public Canvas escapeMenuCanvas;
    public TextMeshProUGUI sensitivityGUI;
    public Slider sensitivitySlider;
    public bool isPaused;

    private float horizLook;
    private float vertLook;
    private RaycastHit hit;
    private bool cast;
    private MouseScript mouseScript;

    // Start is called before the first frame update
    void Start() {
        cat = GameObject.FindGameObjectWithTag("Cat");
        crosshair = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Image>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        horizLook = head.eulerAngles.y;
        vertLook = head.eulerAngles.x;
        escapeMenu = GameObject.FindGameObjectWithTag("Escape");
        escapeMenuCanvas = escapeMenu.GetComponent<Canvas>();
        sensitivitySlider = escapeMenu.GetComponentInChildren<Slider>();
        sensitivityGUI = sensitivitySlider.GetComponentInChildren<TextMeshProUGUI>();
        mouseLookSensitivity = GameManager.Instance.mouseSensitivity;
        sensitivitySlider.value = mouseLookSensitivity;
        isPaused = false;
    }

    // Update is called once per frame
    void Update() {
        // pause menu
        if (Input.GetKeyDown(KeyCode.Escape)) {

            escapeMenuCanvas.enabled = !escapeMenuCanvas.enabled;
            if (escapeMenuCanvas.enabled) {
                isPaused = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                sensitivitySlider.value = player.GetComponent<PlayerController>().mouseLookSensitivity;
            }
            else {
                isPaused = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (isPaused) {
            sensitivityGUI.text = "Mouse Sensitivity: " + sensitivitySlider.value.ToString();
        }

        if (isPaused) {
            return;
        }

        // mouse look
        horizLook += Input.GetAxis("Mouse X") * mouseLookSensitivity * Time.deltaTime;
        vertLook -= Input.GetAxis("Mouse Y") * mouseLookSensitivity * Time.deltaTime;
        vertLook = Mathf.Clamp(vertLook, -85f, 85f);
        head.localRotation = Quaternion.Euler(vertLook, horizLook, 0);

        // crouch
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            isCrouching = !isCrouching;
            if (isCrouching) { 
                head.Translate(Vector3.down * 0.6f, Space.World);
            }
            else { 
                head.Translate(Vector3.up * 0.6f, Space.World);
            }
        }

        // movement
        float deltaX = Input.GetAxis("Horizontal") * walkSpeed;
        float deltaZ = Input.GetAxis("Vertical") * walkSpeed;
        Vector3 movement = (head.forward * deltaZ) + (head.right * deltaX);
        movement = Vector3.ClampMagnitude(movement, walkSpeed);
        movement.y = gravity;
        movement *= Time.deltaTime;
        // Transforms movement from local space to world space.
        movement = transform.TransformDirection(movement);
        characterController.Move(movement);

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

    public void ChangeSensitivity(float sensitivity) {
        mouseLookSensitivity = sensitivity;
        GameManager.Instance.mouseSensitivity = sensitivity;
    }
}
