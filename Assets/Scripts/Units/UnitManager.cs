using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    private HexGrid hexGrid;

    [SerializeField]
    private Movement movementManager;

    public bool PlayersTurn {get; private set;} = true;

    [SerializeField]
    private Unit selectedUnit;
    private HexagonTile previouslySelectedTile;

    public void handleUnitSelection(GameObject unit) {
        if (!PlayersTurn) return;

        Unit logicalUnit = unit.GetComponent<Unit>();
        if(checkIfSelectedTheSameUnit(logicalUnit)) return;

        prepareUnitForMovement(logicalUnit);
    }

    private bool checkIfSelectedTheSameUnit(Unit unit) {
        if (this.selectedUnit == unit) {
            ClearSelection();
            return true;
        }
        return false;
    }

    public void handleTileSelection(GameObject tile) {
        if (selectedUnit == null || !PlayersTurn) return;

        HexagonTile logicalTile = tile.GetComponent<HexagonTile>();

        if (handleTileOutOfRange(logicalTile.HexagonCoordinates) || handleTileWithUnitOn(logicalTile.HexagonCoordinates)) return;

        handleTileSelected(logicalTile);
    }

    private void prepareUnitForMovement(Unit unit) {
        if (this.selectedUnit != null) ClearSelection();

        this.selectedUnit = unit;
        this.selectedUnit.Select();
        movementManager.ShowRange(this.selectedUnit, this.hexGrid);
    }

    private void ClearSelection() {
        previouslySelectedTile = null;
        this.selectedUnit.Deselect();
        movementManager.HideRange(this.hexGrid);
        this.selectedUnit = null;
    }

    private void handleTileSelected(HexagonTile selectedTile) {
        if (previouslySelectedTile == null || previouslySelectedTile != selectedTile) {
            previouslySelectedTile = selectedTile;
            movementManager.ShowPath(selectedTile.HexagonCoordinates, this.hexGrid);
        }
        else {
            movementManager.moveUnit(selectedUnit, this.hexGrid);
            //PlayersTurn = false;
            //selectedUnit.MovementFinished += ResetTurn;
            ClearSelection();
        }
    }

    private bool handleTileWithUnitOn(Vector3Int tilePosition) {
        if(tilePosition == hexGrid.GetClosestTile(selectedUnit.transform.position)) {
            selectedUnit.Deselect();
            ClearSelection();
            return true;
        }
        return false;
    }

    private bool handleTileOutOfRange(Vector3Int tilePosition) {
        if (!movementManager.tileInRange(tilePosition)) {
            Debug.Log("The tile is not reachable");
            return true;
        }
        return false;
    }

    private void ResetTurn(Unit unit) {
        //selectedUnit.MovementFinished -= ResetTurn;
        PlayersTurn = true;
    }
}
