using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private HexGrid hexGrid;

    [SerializeField]
    private List<GameObject> unitsOnBoard = new List<GameObject>();
    private List<GameObject> monstersOnBoard = new List<GameObject>();
    private List<GameObject> adventurersOnBoard = new List<GameObject>();

    [SerializeField]
    private Movement movementManager;


    [SerializeField]
    private Unit selectedUnit;
    private HexagonTile previouslySelectedTile;
    List<Unit> availableMeleeTargets = new List<Unit>();
    List<HexagonTile> availablePickUps = new List<HexagonTile>();

    private void Start() {
        Unit[] units = FindObjectsOfType<Unit>();
        updateUnits();
    }

    private void updateUnits() {
        Unit[] units = FindObjectsOfType<Unit>();
        unitsOnBoard = new List<GameObject>();
        adventurersOnBoard = new List<GameObject>();
        monstersOnBoard = new List<GameObject>();
        foreach (Unit unit in units) {
            if (unit.gameObject.activeInHierarchy) unitsOnBoard.Add(unit.gameObject);
        }
        foreach(GameObject unit in unitsOnBoard) {
            hexGrid.getTileAt(hexGrid.GetClosestTile(unit.transform.position)).stepOnTile(unit.GetComponent<Unit>());
            if (unit.GetComponent<Character>().side == Side.Adventurers) adventurersOnBoard.Add(unit);
            else monstersOnBoard.Add(unit);
        }
    }

    public void handleUnitSelection(GameObject unit) {
        Unit logicalUnit = unit.GetComponent<Unit>();

        if (!gameManager.isStageEnded() && (gameManager.monstersTurn && unit.GetComponent<Character>().unitType == UnitType.Monster || 
            !gameManager.monstersTurn && unit.GetComponent<Character>().unitType == UnitType.Adventurer)) {
            
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
            updateUnits();
            if (adventurersOnBoard.Count == 0) gameManager.MonstersWin();
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

    private void clearPickUps() {
        foreach (HexagonTile pickable in availablePickUps) {
            pickable.DisableHighlight();
        }
        availablePickUps = new List<HexagonTile>();
    }

    public void handleTileSelection(GameObject tile) {
        if (selectedUnit == null) return;

        HexagonTile logicalTile = tile.GetComponent<HexagonTile>();

        if (!logicalTile.hasPickUp() && handleTileOutOfRange(logicalTile.HexagonCoordinates) || handleTileWithUnitOn(logicalTile.HexagonCoordinates)) return;

        handleTileSelected(logicalTile);
    }
    
    public void checkAvailableActions (Unit unit) {
        checkMeleeAttack(hexGrid.GetClosestTile(unit.transform.position), unit);
        if (unit.character.side == Side.Adventurers) {
            checkPicking(hexGrid.GetClosestTile(unit.transform.position), unit);
        }
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

    public void checkPicking (Vector3Int origin, Unit unit) {
        List<Vector3Int> neighbors = hexGrid.getNeightbours(origin);
        availablePickUps = new List<HexagonTile>();
        foreach (Vector3Int tilePosition in neighbors) {
            HexagonTile neighbor = hexGrid.getTileAt(tilePosition);
            if (neighbor.hasPickUp()){
                availablePickUps.Add(neighbor);
                neighbor.EnableHighlight();
            }
        }
    }
    
    private void prepareUnitForMovement(Unit unit) {
        if (this.selectedUnit != null) ClearSelection();

        this.selectedUnit = unit;
        this.selectedUnit.Select();
        if (selectedUnit.hasKey) gameManager.canEndStage();
        else gameManager.disableEndStage();
        movementManager.ShowRange(this.selectedUnit, this.hexGrid);
    }

    private void ClearSelection() {
        previouslySelectedTile = null;
        if (this.selectedUnit != null) {
            this.selectedUnit.Deselect();
            this.selectedUnit = null;
        }
        movementManager.HideRange(this.hexGrid);
        
        clearTargets();
        clearPickUps();
    }

    private void handleTileSelected(HexagonTile selectedTile) {        
        if (availablePickUps.Contains(selectedTile)) {
            selectedTile.GetComponent<Key>().Pick();
            selectedUnit.PickKey();
            ClearSelection();
            return;
        }
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
        ClearSelection();
        gameManager.endTurn();
    }
}
