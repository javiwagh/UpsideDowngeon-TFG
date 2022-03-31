using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    private HexGrid hexGrid;

    [SerializeField]
    private GameObject[] unitsOnBoard;

    [SerializeField]
    private Movement movementManager;

    public bool MonstersTurn {get; private set;} = true; //Player turn

    [SerializeField]
    private Unit selectedUnit;
    private HexagonTile previouslySelectedTile;
    List<Unit> availableMeleeTargets = new List<Unit>();

    private void Start() {
        foreach(GameObject unit in unitsOnBoard) {
            Debug.Log(hexGrid.GetClosestTile(unit.transform.position));
            hexGrid.getTileAt(hexGrid.GetClosestTile(unit.transform.position)).stepOnTile(unit.GetComponent<Unit>());
        }
    }

    public void handleUnitSelection(GameObject unit) {
        Unit logicalUnit = unit.GetComponent<Unit>();

        if (MonstersTurn && unit.GetComponent<Character>().unitType == UnitType.Monster || 
            !MonstersTurn && unit.GetComponent<Character>().unitType == UnitType.Adventurer) {
            
            if(checkIfSelectedTheSameUnit(logicalUnit)) return;
            
            prepareUnitForMovement(logicalUnit);
            checkAvailableActions(logicalUnit);
        }
        else if (this.selectedUnit != null && checkIfSelectedTargetUnit(logicalUnit)) return;
    }

    private bool checkIfSelectedTheSameUnit(Unit unit) {
        if (this.selectedUnit == unit) {
            ClearSelection();
            return true;
        }
        return false;
    }

    private bool checkIfSelectedTargetUnit(Unit unit) {
        if (this.selectedUnit != unit && availableMeleeTargets.Contains(unit)) {
            this.selectedUnit.Attack(unit.GetComponent<Unit>());
            ClearSelection();
            return true;
        }
        return false;
    }

    private void clearTargets() {
        foreach (Unit availableTarget in availableMeleeTargets) {
            availableTarget.Deselect();
        }
        availableMeleeTargets = new List<Unit>();
    }

    public void handleTileSelection(GameObject tile) {
        if (selectedUnit == null) return;

        HexagonTile logicalTile = tile.GetComponent<HexagonTile>();

        if (handleTileOutOfRange(logicalTile.HexagonCoordinates) || handleTileWithUnitOn(logicalTile.HexagonCoordinates)) return;

        handleTileSelected(logicalTile);
    }
    
    public void checkAvailableActions (Unit unit) {
        checkMeleeAttack(hexGrid.GetClosestTile(unit.transform.position), unit);
    }

    public void checkMeleeAttack (Vector3Int origin, Unit unit) {
        List<Vector3Int> neighbors = hexGrid.getNeightbours(origin);
        availableMeleeTargets = new List<Unit>();
        foreach (Vector3Int tilePosition in neighbors) {
            HexagonTile neighbor = hexGrid.getTileAt(tilePosition);
            if (neighbor.isOccupied() && neighbor.unitOn.GetComponent<Character>().side != unit.GetComponent<Character>().side){
                availableMeleeTargets.Add(neighbor.unitOn);
                neighbor.unitOn.Select();
            }
        }
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
        clearTargets();
    }

    private void handleTileSelected(HexagonTile selectedTile) {
        if (previouslySelectedTile == null || previouslySelectedTile != selectedTile) {
            previouslySelectedTile = selectedTile;
            movementManager.ShowPath(selectedTile.HexagonCoordinates, this.hexGrid);
        }
        else {
            Vector3Int unitPosition = hexGrid.GetClosestTile(selectedUnit.transform.position);
            hexGrid.getTileAt(unitPosition).resetTileType();
            selectedTile.stepOnTile(selectedUnit);
            movementManager.moveUnit(selectedUnit, this.hexGrid);
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

    public void endTurn() {
        MonstersTurn = !MonstersTurn;
        if(MonstersTurn) Debug.Log("It's monster's turn!");
        else Debug.Log("It's adventurer's turn!");
    }
}
