using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    const float UNITPOSITION_Y = 0.5411864f;
    
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
    private GameObject unitToSpawn;
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

    public void handleSpawnButtonClick(GameObject unit) {
        if(gameManager.monstersTurn && checkEnoughMana(unit.GetComponent<Character>().cost)) {
            Debug.Log($"Spawning a {unit.GetComponent<Character>().characterName}");
            ClearSelection();
            movementManager.ShowSpawnRange(hexGrid);
            unitToSpawn = unit;
        }
    }

    public void handleUnitSelection(GameObject unit) {
        if(unitToSpawn != null) ClearSelection();
        Unit logicalUnit = unit.GetComponent<Unit>();
        if(checkIfSelectedTheSameUnit(logicalUnit) || logicalUnit.isMoving) return;

        if (!gameManager.isStageEnded() && (gameManager.monstersTurn && unit.GetComponent<Character>().unitType == UnitType.Monster || 
            !gameManager.monstersTurn && unit.GetComponent<Character>().unitType == UnitType.Adventurer)) {           
            prepareUnitForMovement(logicalUnit);
            checkAvailableActions(logicalUnit);
            return;
        }
        if (this.selectedUnit != null) checkIfSelectedTargetUnit(logicalUnit);
    }

    private bool checkIfSelectedTheSameUnit(Unit unit) {
        if (this.selectedUnit == unit) {
            ClearSelection();
            return true;
        }
        return false;
    }

    private void checkIfSelectedTargetUnit(Unit unit) {
        if (this.selectedUnit != unit && availableMeleeTargets.Contains(unit)) {
            this.selectedUnit.Attack(unit.GetComponent<Unit>());
            ClearSelection();
            spendMana(1);
            if (unit.GetComponent<Character>().healthPoints == 0) RemoveUnit(unit.gameObject);
        }
    }

    private void RemoveUnit(GameObject unit) {
        unitsOnBoard.Remove(unit.gameObject);
        if (unit.GetComponent<Character>().side == Side.Adventurers) {                    
            adventurersOnBoard.Remove(unit.gameObject);
            if (adventurersOnBoard.Count == 0) gameManager.MonstersWin();
        }
        else monstersOnBoard.Remove(unit.gameObject);
    }

    private void addUnit(GameObject unit) {
        hexGrid.getTileAt(hexGrid.GetClosestTile(unit.transform.position)).stepOnTile(unit.GetComponent<Unit>());

        unitsOnBoard.Add(unit);
        if (unit.GetComponent<Character>().side == Side.Adventurers) adventurersOnBoard.Add(unit);
        else monstersOnBoard.Add(unit);
        
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
        if (unitToSpawn == null) {
            if (selectedUnit == null) return;

            HexagonTile logicalTile = tile.GetComponent<HexagonTile>();

            if (!logicalTile.hasPickUp() && handleTileOutOfRange(logicalTile.HexagonCoordinates) 
                || handleTileWithUnitOn(logicalTile.HexagonCoordinates)) return;
        }        

        handleTileSelected(tile.GetComponent<HexagonTile>());
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
        if (unitToSpawn != null) {
            movementManager.HideSpawnRange(hexGrid);
            this.unitToSpawn = null;
        }
        if (this.selectedUnit != null) {
            this.selectedUnit.Deselect();
            this.selectedUnit = null;
        }
        movementManager.HideRange(this.hexGrid);
        
        clearTargets();
        clearPickUps();
    }

    private void handleTileSelected(HexagonTile selectedTile) {  
        if (unitToSpawn != null) {
            if (!selectedTile.isWalkable()) return;
            GameObject newUnit = Instantiate(unitToSpawn, new Vector3 (selectedTile.transform.position.x, UNITPOSITION_Y, selectedTile.transform.position.z),
             new Quaternion(selectedTile.transform.rotation.x, selectedTile.transform.rotation.y, selectedTile.transform.rotation.z, selectedTile.transform.rotation.w));
            newUnit.transform.RotateAround(newUnit.transform.position, Vector3.up, 150f);
            ClearSelection();
            addUnit(newUnit);
            spendMana(newUnit.GetComponent<Character>().cost);
            return;
        }      
        if (availablePickUps.Contains(selectedTile)) {
            selectedTile.GetComponent<Key>().Pick();
            selectedUnit.PickKey();
            ClearSelection();
            spendMana(1);
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
            spendMana(1);
        }      
    }

    private void spendMana(int cost) {
        gameManager.player.manaPoints -= cost;
        gameManager.player.updateMana();
        if (gameManager.player.manaPoints == 0) endTurn();
    }

    private bool checkEnoughMana(int cost) {
        if (gameManager.player.manaPoints - cost >= 0) return true;
        return false;
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
