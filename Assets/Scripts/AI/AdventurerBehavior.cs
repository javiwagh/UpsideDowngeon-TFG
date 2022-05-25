using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerBehavior : MonoBehaviour
{
    [SerializeField]
    private float TILE_SELECT_WAITING_TIME = 0.5f;
    [SerializeField]
    private float CHARACTER_SELECT_WAITING_TIME = 1.0f;
    public UnitManager unitManager;
    private Unit unit;
    private Character character;
    private HexagonTile targetTile;
    private HexagonTile targetTileInRange;
    private Room endRoom = null;
    private Room keyRoom = null;
    private HexagonTile tileWithKeyMonster = null;
    public bool Performing;
    public bool characterSelected;
    public bool tileSelected;
    private bool attacking;
    private bool firstSelection = true;

    private List<Room> visitedRooms;
    private void Start() {
        this.unitManager = FindObjectOfType<UnitManager>();
        this.unit = this.GetComponent<Unit>();
        this.character = this.GetComponent<Character>(); 
        visitedRooms = new List<Room>();
        targetTile = null;
        Performing = false;
        characterSelected = false;
    }
    public void Perform() {        
        Debug.Log($"Performing {character.characterName}'s turn!");
        Performing = true;
        characterSelected = false;
        attacking = false;
        checkRoom();
        foreach (Room room in visitedRooms) Debug.Log($"I have already visited {room}");
        StartCoroutine(PerformTurnCoroutine());
    }

    IEnumerator PerformTurnCoroutine() {
        //SELECT THE CHARACTER
        StartCoroutine(selectCharacter());
        while (!characterSelected) yield return null;

        //BINARY DECISION BT////////////////////////////////////////////////////////////////////
        Debug.Log("Are there any nearby monsters?");
        List<HexagonTile> monstersInRange = lookForMonstersInRange();
        if (monstersInRange.Count > 0){
            Debug.Log("Yes, there is at least one of those filthy creatures nearby!");
            Debug.Log("Hm... Shall I attack?");
            if (coinFlip()) {
                Debug.Log("YES! GOTTA KILL IT!");
                attacking = true;
                setMonsterInRangeAsTarget(monstersInRange);
            } 
        }
        if (!attacking) {
            Debug.Log("Nope.");
            Debug.Log("Do I have the key?");
            if (gotTheKey()) {
                Debug.Log("Yes, I got the key");
                if (endRoom == null) endRoom = alreadyVisitedEndRoom();
                Debug.Log("Have I arleady visited the end room?");
                if (endRoom != null) {
                    Debug.Log("Yes! Heading the end!");
                    setTarget(endRoom.endTile);
                }
                else {
                    Debug.Log("Nope, gotta find that end tile!");
                    explore();
                } 
            }
            else {
                Debug.Log("Nope, gotta find the key yet!");
                Debug.Log("Have I arleady visited the key room?");
                if (keyRoom == null) keyRoom = alreadyVisitedKeyRoom();
                if (keyRoom != null) {
                    Debug.Log("YES! Gotta take that key!");
                    setTarget(keyRoom.keyTile);
                }
                else {
                    Debug.Log("Nope, gotta find that key!");
                    Debug.Log("Is there a monster with the key in the room?");
                    tileWithKeyMonster = lookForKeyMonstersInRoom();
                    if (tileWithKeyMonster != null) {
                        Debug.Log("YES! GOTTA KILL IT!");
                        attacking = true;
                        setTarget(tileWithKeyMonster);
                    }
                    else{
                        Debug.Log("Nope, there is no monster with key here!");
                        explore();
                    }   
                } 
            }
        }
        
        //GO TOWARDS THE TARGET////////////////////////////////////////////////////////////////////

        if (targetTileInRange != null) {
            StartCoroutine(selectTile());
            while (!tileSelected) yield return null;

            StartCoroutine(selectTile());
            while (!tileSelected) yield return null;

            if (targetTile == targetTileInRange) targetTile = null;
        }
        while (unit.isMoving) yield return null;
        yield return new WaitForSeconds(CHARACTER_SELECT_WAITING_TIME);
        Performing = false;
    }

    //ACTIONS////////////////////////////////////////////////////////////////////
    private void explore() {
        if (targetTile == null) setPriorityTarget(); 
        else setTarget(targetTile);
    }

    private void setTarget(HexagonTile tile) {        
        this.targetTile = tile;
        if (!goTowardsTarget()) setPriorityTarget(); 
        else Debug.Log($"My target is the door at {targetTile.HexagonCoordinates}!");
    }

    private void setMonsterInRangeAsTarget(List<HexagonTile> monstersInRange) {   
        monstersInRange = Shuffle(monstersInRange);  
        foreach (HexagonTile monsterTile in monstersInRange) {
            targetTile = monsterTile;
            if (goTowardsTarget()) {
                Debug.Log($"My target is the monster at {targetTile.HexagonCoordinates}!");
                targetTileInRange = targetTile;
                return;
            }
        }   
        setPriorityTarget(); 
    }

    //CHECKING METHODS////////////////////////////////////////////////////////////////////
    private bool coinFlip() {
        if (Random.value < 0.5f) return true;
        return false;
    }

    private bool gotTheKey() {
        if (unit.hasKey) return true;
        return false;
    }

    private Room alreadyVisitedEndRoom() {
        foreach (Room room in visitedRooms) {
            if (room != null && room.hasEndTile) return room;
        }
        return null;
    }

    private Room alreadyVisitedKeyRoom() {
        foreach (Room room in visitedRooms) {
            if (room.hasKeyTile) return room;
        }
        return null;
    }

    private List<HexagonTile> lookForMonstersInRange() {
        return unitManager.getMonstersInRange();
    }

    private HexagonTile lookForKeyMonstersInRoom() {
        if (unit.onTile.originalTileType == TileType.Door) {
            foreach (Room room in unit.onTile.GetComponent<Door>().roomsAvailable) {
                if (room.monstersInRoom().Count > 0) foreach (HexagonTile tile in room.monstersInRoom()) {
                    if (tile.unitOn.hasKey) return tile;
                }
            } 
        }
        else {
            if (unit.onTile.room.monstersInRoom().Count > 0) foreach (HexagonTile tile in unit.onTile.room.monstersInRoom()) {
                if (tile.unitOn.hasKey) return tile;
            }
        }
        return null;
    }

    //AUX METHODS////////////////////////////////////////////////////////////////////
    private void checkRoom() {
        if (unit.onTile.originalTileType == TileType.Door) {
            foreach (Room room in unit.onTile.GetComponent<Door>().roomsAvailable) {
                if (room != null && !visitedRooms.Contains(room)) visitedRooms.Add(room);
            } 
        }
        else {
            if (unit.onTile.room != null && !visitedRooms.Contains(unit.onTile.room)) visitedRooms.Add(unit.onTile.room);
        }
    }

    private void setPriorityTarget() {
        List<HexagonTile> monstersInRange = lookForMonstersInRange();
        if (monstersInRange.Count > 0){
            attacking = true;
            setMonsterInRangeAsTarget(monstersInRange);
            return;
        }
        
        attacking = false;
        HexagonTile currentTile = unit.onTile;

        List<HexagonTile> possibleTargets = new List<HexagonTile>();
        if (currentTile.originalTileType == TileType.Door) {
            List<Room> roomsAvailable = currentTile.GetComponent<Door>().roomsAvailable;
            foreach (Room room in roomsAvailable) {
                foreach (Door door in room.doors) {
                    possibleTargets.Add(door.GetComponent<HexagonTile>());
                }
            }            
        }
        else
        {
            Room room = currentTile.GetComponent<HexagonTile>().room;
            foreach (Door door in room.doors) {
                possibleTargets.Add(door.GetComponent<HexagonTile>());
            }
        }

        //Shuffle the list
        possibleTargets = Shuffle(possibleTargets);
        /*for (int i = 0; i < possibleTargets.Count; i++) {
            HexagonTile temp = possibleTargets[i];
            int randomIndex = Random.Range(i, possibleTargets.Count);
            possibleTargets[i] = possibleTargets[randomIndex];
            possibleTargets[randomIndex] = temp;
        }*/
        
        foreach(HexagonTile possibleTarget in possibleTargets) {
            targetTile = possibleTarget;
            if (goTowardsTarget()) {
                Debug.Log($"Changed my target to the tile at {targetTile.HexagonCoordinates}!");
                //targetTileInRange = targetTile;
                return;
            }
        }
        
        //If every target has failed, move randomly
        possibleTargets = Shuffle(unitManager.getTilesAvailable());
        foreach(HexagonTile possibleTarget in possibleTargets) {
            targetTile = possibleTarget;
            if (goTowardsTarget()) {
                Debug.Log($"Changed my target to the tile at {targetTile.HexagonCoordinates}!");
                //targetTileInRange = targetTile;
                return;
            }
        }
        unit.spendActionPoint();
        unitManager.handleUnitSelection(this.gameObject);

    }

    private List<HexagonTile> Shuffle(List<HexagonTile> tileList) {
        for (int i = 0; i < tileList.Count; i++) {
            HexagonTile temp = tileList[i];
            int randomIndex = Random.Range(i, tileList.Count);
            tileList[i] = tileList[randomIndex];
            tileList[randomIndex] = temp;
        }
        return tileList;
    }

    private bool goTowardsTarget() {
        if (unitManager.Neighbours(unit.onTile, targetTile) && attacking) return true;
        
        try {
            targetTileInRange = unitManager.findPath(targetTile.HexagonCoordinates);
        }
        catch(System.StackOverflowException e) {
            return false;
        }
        if (targetTileInRange == null) return false;

        return true;
    }

    IEnumerator selectCharacter() {
        characterSelected = false;
        unitManager.handleUnitSelection(this.gameObject);
        if (firstSelection){
            yield return new WaitForSeconds(CHARACTER_SELECT_WAITING_TIME*2);
            firstSelection = false;
        } 
        else {
            yield return new WaitForSeconds(CHARACTER_SELECT_WAITING_TIME/2);
            firstSelection = true;
        }
        characterSelected = true;
    }

    IEnumerator selectTile() {
        tileSelected = false;
        if (targetTile.hasPickUp() || targetTile.isOccupied() && notWalkableTargetTileInRange()) targetTileInRange = targetTile;
        unitManager.handleTileSelection(targetTileInRange.gameObject);
        yield return new WaitForSeconds(TILE_SELECT_WAITING_TIME);
        tileSelected = true;
    }

    private bool notWalkableTargetTileInRange() {
        return unitManager.Neighbours(targetTile, targetTileInRange);
    }
}
