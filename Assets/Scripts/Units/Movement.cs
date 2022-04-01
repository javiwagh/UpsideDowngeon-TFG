using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour{
    private BFSearch movementRange = new BFSearch();
    private List<Vector3Int> currentPath = new List<Vector3Int>();

    public void HideRange(HexGrid hexGrid) {
        IEnumerable<Vector3Int> rangePositions = movementRange.getRangePositions();
        if (rangePositions != null) {
            foreach (Vector3Int tilePosition in rangePositions)
            {
                hexGrid.getTileAt(tilePosition).ResetHighlight();
                hexGrid.getTileAt(tilePosition).DisableHighlight();
            }
        }
        movementRange = new BFSearch();
    }

    public void ShowRange (Unit selectedUnit, HexGrid hexGrid) {
        CalculateRange(selectedUnit, hexGrid);

        Vector3Int unitPosition = hexGrid.GetClosestTile(selectedUnit.transform.position);
        /*if (!selectedUnit.hasKey) {
            foreach(Vector3Int tilePosition in movementRange.getRangePositions()) {
                if (hexGrid.getTileAt(tilePosition).isStageEnd()) {
                    movementRange.RemoveFromRange(tilePosition);
                    break;
                }     
            }
        }*/
        
        foreach(Vector3Int tilePosition in movementRange.getRangePositions()) {
            if (unitPosition != tilePosition)
                hexGrid.getTileAt(tilePosition).EnableHighlight();
        }
    }

    public void CalculateRange(Unit selectedUnit, HexGrid hexGrid) {
        movementRange = GraphSearch.BFSGetRange(hexGrid, hexGrid.GetClosestTile(selectedUnit.transform.position), selectedUnit.MovementPoints);
    }

    public void ShowPath(Vector3Int selectedTilePosition, HexGrid hexGrid) {
        if (movementRange.getRangePositions().Contains(selectedTilePosition)) {
            foreach (Vector3Int tilePosition in currentPath) {
                hexGrid.getTileAt(tilePosition).ResetHighlight();
            }
            currentPath = movementRange.getPathTo(selectedTilePosition);
            foreach (Vector3Int tilePosition in currentPath) {
                hexGrid.getTileAt(tilePosition).HighlightPath();
            }
        }
    }

    public void moveUnit (Unit selectedUnit, HexGrid hexGrid) {
        Debug.Log("Moving unit " + selectedUnit.name);
        selectedUnit.moveThroughPath(currentPath.Select(pos => hexGrid.getTileAt(pos).transform.position).ToList());
    }

    public bool tileInRange (Vector3Int tilePosition) {
        return movementRange.tileInRange(tilePosition);
    }
}
