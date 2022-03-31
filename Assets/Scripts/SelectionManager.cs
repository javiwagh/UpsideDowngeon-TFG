using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    private Camera cam;
    public LayerMask selectionMask;
    public HexGrid hexGrid;
    private List<Vector3Int> neighbours = new List<Vector3Int>();

    public UnityEvent<GameObject> onUnitSelected;
    public UnityEvent<GameObject> TileSelected;

    private void Awake() {
        cam = playerInput.camera;
    }

    public void HandleClick(InputAction.CallbackContext context) {
        if (context.canceled) {
            Vector3 mousePosition = Input.mousePosition;
            GameObject result;
            //Debug.Log(findRayTarget(mousePosition, out result));
            if (findRayTarget(mousePosition, out result)) {
                if (UnitSelected(result)) {
                    onUnitSelected?.Invoke(result);
                }
                else {
                    TileSelected?.Invoke(result);
                }
                
            }
        }        
    }

    private bool UnitSelected(GameObject result) {
        return result.GetComponent<Unit>() != null;
    }

    private bool findRayTarget(Vector3 mousePosition, out GameObject result) {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit, selectionMask)) {
            result = hit.collider.gameObject;
            return true;
        }
        result = null;
        return false;
    }
}
