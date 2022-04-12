using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private PlayerInput playerInput;
    private Camera cam;
    public LayerMask selectionMask;
    public HexGrid hexGrid;
    private List<Vector3Int> neighbours = new List<Vector3Int>();

    public UnityEvent<GameObject> onUnitSelected;
    public UnityEvent<GameObject> onTileSelected;

    private void Awake() {
        cam = playerInput.camera;
        gameManager = FindObjectOfType<GameManager>();
    }

    public void HandleClick(InputAction.CallbackContext context) {
        if (gameManager.monstersTurn && context.canceled) {
            Vector3 mousePosition = Input.mousePosition;
            GameObject result;
            if (findRayTarget(mousePosition, out result)) {
                if (UnitSelected(result)) {
                    onUnitSelected?.Invoke(result);
                }
                else if (TileSelected(result)){
                    onTileSelected?.Invoke(result);
                }
                
            }
        }        
    }

    private bool UnitSelected(GameObject result) {
        return result.GetComponent<Unit>() != null;
    }

    private bool TileSelected(GameObject result) {
        return result.GetComponent<HexagonTile>() != null;
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
