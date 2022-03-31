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
    private Unit selectedObjective;
    private HexagonTile previouslySelectedTile;

    private void Start() {
        foreach(GameObject unit in unitsOnBoard) {
            Debug.Log(hexGrid.GetClosestTile(unit.transform.position));
            hexGrid.getTileAt(hexGrid.GetClosestTile(unit.transform.position)).stepOnTile();
        }
    }

    public void handleUnitSelection(GameObject unit) {
        if (MonstersTurn && unit.GetComponent<Character>().unitType == UnitType.Monster || 
                !MonstersTurn && unit.GetComponent<Character>().unitType == UnitType.Adventurer) {
                Unit logicalUnit = unit.GetComponent<Unit>();
                if(checkIfSelectedTheSameUnit(logicalUnit)) return;

                checkAvailableActions(logicalUnit);
                prepareUnitForMovement(logicalUnit);
        }
        /*
        else if (MonstersTurn && unit.GetComponent<Character>().unitType == UnitType.Adventurer || 
            !MonstersTurn && (unit.GetComponent<Character>().unitType == UnitType.Monster
            || unit.GetComponent<Character>().unitType == UnitType.Nest)) {

            if (selectedUnit == null) return;

            Unit objectiveUnit = unit.GetComponent<Unit>();
            HexagonTile objectiveUnitTile = hexGrid.getTileAt(hexGrid.GetClosestTile(objectiveUnit.transform.position));

            prepareUnitForAttack(objectiveUnit, objectiveUnitTile);
        }*/
    }

    private bool checkIfSelectedTheSameUnit(Unit unit) {
        if (this.selectedUnit == unit) {
            ClearSelection();
            return true;
        }
        return false;
    }

    public void handleTileSelection(GameObject tile) {
        if (selectedUnit == null) return;

        HexagonTile logicalTile = tile.GetComponent<HexagonTile>();

        if (handleTileOutOfRange(logicalTile.HexagonCoordinates) || handleTileWithUnitOn(logicalTile.HexagonCoordinates)) return;

        handleTileSelected(logicalTile);
    }
    
    public void checkAvailableActions (Unit unit) {
        checkMeleeAttack(hexGrid.GetClosestTile(unit.transform.position));        
    }

    public void checkMeleeAttack (Vector3Int origin) {
        List<Vector3Int> neighbours = hexGrid.getNeightbours(origin);
        foreach (Vector3Int tilePosition in neighbours) {
            if (hexGrid.getTileAt(tilePosition).isOccupied()){
                Debug.Log("There is an available melee target.");
            }
        }
    }
    
    private void prepareUnitForMovement(Unit unit) {
        if (this.selectedUnit != null) ClearSelection();

        this.selectedUnit = unit;
        this.selectedUnit.Select();
        movementManager.ShowRange(this.selectedUnit, this.hexGrid);
    }

    /*private void prepareUnitForAttack(Unit objectiveUnit, HexagonTile objectiveTile) {
        if (selectedObjective == null || selectedObjective != objectiveUnit) {
            Debug.Log("Preparing for the offensive!");

            selectedObjective = objectiveUnit;
            this.selectedObjective.Select();
            movementManager.ShowPath(objectiveTile.HexagonCoordinates, this.hexGrid);
        }
        else {
            Vector3Int unitPosition = hexGrid.GetClosestTile(selectedUnit.transform.position);
            hexGrid.getTileAt(unitPosition).resetTileType();
            selectedTile.stepOnTile();
            movementManager.moveUnit(selectedUnit, this.hexGrid);
            ClearSelection();
        }
    }*/

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
            Vector3Int unitPosition = hexGrid.GetClosestTile(selectedUnit.transform.position);
            hexGrid.getTileAt(unitPosition).resetTileType();
            selectedTile.stepOnTile();
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
        //selectedUnit.MovementFinished -= ResetTurn;
        MonstersTurn = !MonstersTurn;
        if(MonstersTurn) Debug.Log("It's monster's turn!");
        else Debug.Log("It's adventurer's turn!");
    }
}
