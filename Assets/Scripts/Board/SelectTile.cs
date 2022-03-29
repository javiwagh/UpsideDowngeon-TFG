using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTile : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    public LayerMask selectionMask;
    public HexGrid hexGrid;
    private List<Vector3Int> neighbours = new List<Vector3Int>();

    private void Awake() {
        if(mainCamera == null)
            mainCamera = Camera.main;
    }

    public void HandleClick() {
        Vector3 mousePosition = Input.mousePosition;
        GameObject result;
        //Debug.Log(findRayTarget(mousePosition, out result));
        if (findRayTarget(mousePosition, out result)) {
            HexagonTile selectedTile = result.GetComponent<HexagonTile>();

            selectedTile.DisableHighlight();
            foreach(Vector3Int neighbour in neighbours) {
                hexGrid.getTileAt(neighbour).DisableHighlight();
            }

            neighbours = hexGrid.getNeightbours(selectedTile.HexagonCoordinates);
            foreach(Vector3Int neighbour in neighbours) {
                hexGrid.getTileAt(neighbour).EnableHighlight();
            }
            
        }
    }

    private bool findRayTarget(Vector3 mousePosition, out GameObject result) {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit, selectionMask)) {
            result = hit.collider.gameObject;
            return true;
        }
        result = null;
        return false;
    }
}
