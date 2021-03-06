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

    public TooltipManager tooltipManager;

    public Texture2D cursorRest;
    public Texture2D cursorGlow;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public bool runningTutorial = false;
    public Tutorial basicTutorial;
    public bool runningBasicTutorial = false;
    public Tutorial movementTutorial;
    public bool runningMovementTutorial = false;
    public bool listenUnitMovement = false;

    public void updateUnits() {
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
        ClearSelection();
        if(gameManager.monstersTurn && checkEnoughMana(unit.GetComponent<Character>().cost)) {
            Cursor.SetCursor(cursorGlow, hotSpot, cursorMode);
            movementManager.ShowSpawnRange(hexGrid);
            unitToSpawn = unit;
        }
    }

    public void handleUnitSelection(GameObject unit) {
        Unit logicalUnit = unit.GetComponent<Unit>();
        if (this.selectedUnit != null && selectedTargetUnit(logicalUnit)) return;
        if(unitToSpawn != null) ClearSelection();
        
        if(checkIfSelectedTheSameUnit(logicalUnit) || logicalUnit.isMoving) return;
        if (selectedUnit != null && selectedUnit.actionPoints == 0) return;

        if (logicalUnit.actionPoints > 0 && !gameManager.isStageEnded() 
            && (gameManager.monstersTurn && unit.GetComponent<Character>().unitType == UnitType.Monster 
            || !gameManager.monstersTurn && unit.GetComponent<Character>().unitType == UnitType.Adventurer)) {
            showActions(logicalUnit);
            if (gameManager.monstersTurn) {
                Cursor.SetCursor(cursorGlow, hotSpot, cursorMode);
                if (movementTutorial != null && movementTutorial.gameObject.activeInHierarchy && runningMovementTutorial) movementTutorial.PlayerActionDone(unit.transform);
            }
            //checkAvailableActions(logicalUnit);
            return;
        }
    }

    public void handleTileSelection(GameObject tile) {
        if (unitToSpawn == null) {
            Debug.LogWarning(selectedUnit);
            if (selectedUnit == null) return;

            HexagonTile logicalTile = tile.GetComponent<HexagonTile>();
            
            Debug.LogWarning(logicalTile.HexagonCoordinates);
            if (!logicalTile.hasPickUp() && handleTileOutOfRange(logicalTile.HexagonCoordinates) 
                || handleTileWithSelectedUnitOn(logicalTile.HexagonCoordinates)) return;
        }        

        handleTileSelected(tile.GetComponent<HexagonTile>());
    }

    private bool checkIfSelectedTheSameUnit(Unit unit) {
        if (this.selectedUnit == unit) {
            ClearSelection();
            return true;
        }
        return false;
    }

    private bool selectedTargetUnit(Unit unit) {
        if (this.selectedUnit.GetComponent<Character>().side != unit.GetComponent<Character>().side 
        && movementManager.tileInRange(unit.onTile.HexagonCoordinates)) {
            //this.selectedUnit.Attack(unit.GetComponent<Unit>());
            handleTileSelection(unit.onTile.gameObject);
            return true;
            //ClearSelection();
            //if (unit.GetComponent<Character>().healthPoints == 0) RemoveUnit(unit.gameObject);
        }
        return false;
    }

    public void RemoveUnit(GameObject unit) {
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
            availableTarget.ResetHighlight();
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
    
    /*public void checkAvailableActions (Unit unit) {
        if (!unit.isBard()) checkMeleeAttack(hexGrid.GetClosestTile(unit.transform.position), unit);
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
    }*/
    
    private void showActions(Unit unit) {
        if (this.selectedUnit != null) ClearSelection();

        this.selectedUnit = unit;
        this.selectedUnit.Select();
        if (selectedUnit.hasKey && selectedUnit.GetComponent<Character>().side == Side.Adventurers) gameManager.canEndStage();
        else gameManager.disableEndStage();
        movementManager.ShowRange(this.selectedUnit, this.hexGrid);
    }

    public void ClearSelection() {
        if (!runningMovementTutorial) {
            previouslySelectedTile = null;
            if (unitToSpawn != null) {
                movementManager.HideSpawnRange(hexGrid);
                this.unitToSpawn = null;
            }
            if (this.selectedUnit != null) {
                this.selectedUnit.ResetHighlight();
                this.selectedUnit.Deselect();
                this.selectedUnit = null;
            }
            movementManager.HideRange(this.hexGrid);
            if (gameManager.monstersTurn) Cursor.SetCursor(cursorRest, hotSpot, cursorMode);
            clearTargets();
            clearPickUps();
        }
    }

    private void handleTileSelected(HexagonTile selectedTile) {  
        if (unitToSpawn != null) {
            if (!movementManager.spawnTiles.Contains(selectedTile)) return;
            GameObject newUnit = Instantiate(unitToSpawn, new Vector3 (selectedTile.transform.position.x, UNITPOSITION_Y, selectedTile.transform.position.z),
             new Quaternion(selectedTile.transform.rotation.x, selectedTile.transform.rotation.y, selectedTile.transform.rotation.z, selectedTile.transform.rotation.w));
            newUnit.transform.RotateAround(newUnit.transform.position, Vector3.up, 150f);
            newUnit.GetComponent<Unit>().gameManager = gameManager;
            
            ClearSelection();
            addUnit(newUnit);
            spendMana(newUnit.GetComponent<Character>().cost);
            if (basicTutorial != null && basicTutorial.gameObject.activeInHierarchy && runningBasicTutorial) basicTutorial.PlayerActionDone(newUnit.transform);
            return;
        }
        if (movementTutorial != null && movementTutorial.gameObject.activeInHierarchy && runningMovementTutorial && !listenUnitMovement) movementTutorial.PlayerActionDone(selectedTile.gameObject.transform);
        if (previouslySelectedTile == null || previouslySelectedTile != selectedTile) {
            if (previouslySelectedTile != null) {
                previouslySelectedTile.ResetHighlight();
                if (previouslySelectedTile.unitOn != null) previouslySelectedTile.unitOn.ResetHighlight();
            }
            previouslySelectedTile = selectedTile;
            
            if (!(selectedTile.hasPickUp() || selectedTile.isOccupied())) movementManager.ShowPath(selectedTile.HexagonCoordinates, selectedUnit.onTile.transform.position, hexGrid);
            else {
                if (!Neighbours(selectedUnit.onTile, selectedTile)) movementManager.ShowPath(selectedTile.HexagonCoordinates, selectedUnit.onTile.transform.position, hexGrid);
                if(selectedTile.isOccupied()) selectedTile.unitOn.HighlightTarget();
                else selectedTile.HighlightTarget();
            }
        }
        else {
            bool selectedTileIsNeighbor = Neighbours(selectedUnit.onTile, selectedTile);
            if (movementTutorial != null && movementTutorial.gameObject.activeInHierarchy && runningMovementTutorial && listenUnitMovement) movementTutorial.PlayerActionDone(selectedTile.gameObject.transform);

            if (selectedTile.hasPickUp()) {
                if (!selectedTileIsNeighbor) moveUnit(selectedTile);
                selectedUnit.PickKey(selectedTile.GetComponent<Key>(), selectedTileIsNeighbor);
            }
            else if (selectedTile.isOccupied() && selectedTile != selectedUnit.onTile) {
                if (!selectedTileIsNeighbor) moveUnit(selectedTile);
                selectedUnit.Attack(selectedTile.unitOn.GetComponent<Unit>(), selectedTileIsNeighbor);
            }
            else moveUnit(selectedTile);
            ClearSelection();    
        }      
    }

    public bool Neighbours(HexagonTile originTile, HexagonTile selectedTile) {
        List<Vector3Int> neighbors = hexGrid.getNeightbours(originTile.HexagonCoordinates);
        foreach (Vector3Int tilePosition in neighbors) {
            if (tilePosition == selectedTile.HexagonCoordinates) return true;
        }
        return false;
    }

    public void moveUnit(HexagonTile targetTile) {
        Vector3Int unitPosition = selectedUnit.onTile.HexagonCoordinates;
        hexGrid.getTileAt(unitPosition).resetTileType();
        movementManager.moveUnit(selectedUnit, this.hexGrid);
    }

    private void spendMana(int cost) {
        gameManager.player.manaPoints -= cost;
        gameManager.player.updateMana();
    }

    private bool checkEnoughMana(int cost) {
        if (gameManager.player.manaPoints - cost >= 0) return true;
        return false;
    }

    private bool handleTileWithSelectedUnitOn(Vector3Int tilePosition) {
        if(tilePosition == selectedUnit.onTile.HexagonCoordinates) {
             Debug.Log("The tile is not reachable");
            selectedUnit.ResetHighlight();
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
        if (!gameManager.isStageEnded()) {
            gameManager.endTurn();
            performTurnShiftUnitActions();
        }

        if (!gameManager.isStageEnded() && !gameManager.monstersTurn) StartCoroutine(PerformAITurn()); //Perform AI turn      
    }

    private void performTurnShiftUnitActions() {
        List<GameObject> died = new List<GameObject>();
        foreach (GameObject unit in unitsOnBoard) {
            Unit logicalUnit = unit.GetComponent<Unit>();
            logicalUnit.TurnShiftUnitActions();
            if (unit.GetComponent<Character>().healthPoints > 0) logicalUnit.Exhaust();
            else died.Add(unit);
        }

        foreach (GameObject unit in died) RemoveUnit(unit);

        List<GameObject> units;
        if (gameManager.monstersTurn) units = monstersOnBoard;
        else units = adventurersOnBoard;
        foreach (GameObject unit in units) {
            unit.GetComponent<Unit>().restoreActionPoints();
        } 
    }

    //AI TURN METHODS
    IEnumerator PerformAITurn() {
        tooltipManager.paused = true;

        foreach(GameObject adventurer in adventurersOnBoard) {
            AdventurerBehavior behavior = adventurer.GetComponent<AdventurerBehavior>();
            while(adventurer.GetComponent<Unit>().actionPoints > 0 && !gameManager.isStageEnded()) {
                gameManager.followAdventurer(adventurer);
                behavior.Perform();
                while (behavior.Performing) yield return null;
                ClearSelection();
                while (gameManager.gameState == GameManager.GameState.Moving) yield return null;
            }
            if (gameManager.isStageEnded()) break;
        }
        gameManager.freeCamera();
        tooltipManager.paused = false;
        if (runningTutorial && movementTutorial != null && movementTutorial.gameObject.activeInHierarchy && monstersOnBoard.Count != 0) {
            movementTutorial.StartTutorial();
            runningMovementTutorial = true;
        }
        endTurn();
    }

    public HexagonTile findPath(Vector3Int target) {
        Vector3Int targetTileInRangePosition = movementManager.findPath(target, selectedUnit, hexGrid);
        if (targetTileInRangePosition != new Vector3Int()) return hexGrid.getTileAt(targetTileInRangePosition);
        return null;
    }

    public List<HexagonTile> getMonstersInRange() {
        List<HexagonTile> monstersTileList = new List<HexagonTile>();
        IEnumerable<Vector3Int> unitsInRange = movementManager.getUnitsInRange();
        if (unitsInRange != null) {
            foreach (Vector3Int tilePosition in unitsInRange) {
                HexagonTile tile = hexGrid.getTileAt(tilePosition);
                if (tile.unitOn.GetComponent<Character>().side == Side.Monsters) monstersTileList.Add(tile);
            }
        }
        return monstersTileList;
    }

    public List<HexagonTile> getTilesAvailable() {
        List<HexagonTile> tilesAvailable = new List<HexagonTile>();
        IEnumerable<Vector3Int> unitsInRange = movementManager.getRangePositions();
        if (unitsInRange != null) {
            foreach (Vector3Int tilePosition in unitsInRange) tilesAvailable.Add(hexGrid.getTileAt(tilePosition));
        }
        return tilesAvailable;
    }
}
