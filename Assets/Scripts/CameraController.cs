using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public bool playerPanControl = true;

    [SerializeField] 
    private bool mousePanControl;
    [SerializeField] 
    private float speed = 20f;
    [SerializeField] 
    private float followSpeed = 15f;
    
    private float panZone = Screen.width / 50;
    [SerializeField] 
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;

    [SerializeField] 
    private Vector2 limit;
    private Vector3 cameraOffset;
    public Vector2 followingCameraOffset;

    private Transform target;

    private void Awake() {
        playerInput.GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Zoom.performed += performZoom;
        cameraOffset = this.transform.position;
        target = null;
    }

    private void Update() {
        //Debug.LogWarning($"My target is {target}");
        if (target != null) {
            Vector3 targetPosition = new Vector3(target.transform.position.x - followingCameraOffset.x, transform.position.y, target.transform.position.z - followingCameraOffset.y);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }
        if (playerPanControl) {
            Vector2 input = playerInputActions.Player.Movement.ReadValue<Vector2>();
            Vector3 move = new Vector3();
            if (mousePanControl && input == new Vector2(0f, 0f)) {
                if (Input.mousePosition.x >= Screen.width - panZone) input.x = 1f;
                else if (Input.mousePosition.x <= panZone) input.x = -1f;

                if (Input.mousePosition.y >= Screen.height - panZone) input.y = 1f;
                else if (Input.mousePosition.y <= panZone) input.y = -1f;
            }

            move = (transform.right * input.x + transform.forward * input.y) * speed * Time.deltaTime;
            
            Vector3 finalPosition = transform.position + move;
            //if (Vector3.Distance(cameraOffset, finalPosition) < limit) transform.position = finalPosition;
            finalPosition.x = Mathf.Clamp(finalPosition.x, cameraOffset.x -limit.x, cameraOffset.x + limit.x);
            finalPosition.z = Mathf.Clamp(finalPosition.z, cameraOffset.z -limit.y, cameraOffset.z + limit.y); 

            transform.position = finalPosition;
        }        
    }

    public void performZoom(InputAction.CallbackContext context) {
        //Debug.Log("Hewo?");
        //context.ReadValue<float>(); 
        //Debug.Log(context.ReadValue<float>());
    }

    public void followAdventurer(GameObject adventurer) {
        target = adventurer.GetComponent<Transform>();
        playerPanControl = false;
    }

    public void focusOn(Transform focus) {
        target = focus;
        playerPanControl = false;
    }

    public void freeCamera() {
        target = null;
        playerPanControl = true;
    }
}
