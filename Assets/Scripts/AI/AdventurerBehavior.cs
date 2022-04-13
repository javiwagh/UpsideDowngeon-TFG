using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerBehavior : MonoBehaviour
{
    [SerializeField]
    private float WAITING_TIME = 0.5f;
    public UnitManager unitManager;
    private Unit unit;
    private Character character;
    private HexagonTile targetTile;
    private HexagonTile targetTileInRange;
    public bool Performing;
    public bool characterSelected;
    public bool tileSelected;

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
        checkRoom();
        foreach (Room room in visitedRooms) Debug.Log($"I have already visited {room}");
        StartCoroutine(PerformTurnCoroutine());
    }

    IEnumerator PerformTurnCoroutine() {
        //SELECT THE CHARACTER
        StartCoroutine(selectCharacter());
        while (!characterSelected) yield return null;

        //BINARY DECISION BT////////////////////////////////////////////////////////////////////
        Debug.Log("Do I have the key?");
        if (gotTheKey()) {
            Debug.Log("Yes, I got the key");
            Room endRoom = alreadyVisitedEndRoom();
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
            Room keyRoom = alreadyVisitedKeyRoom();
            if (keyRoom != null) {
                Debug.Log("YES! Gotta take that key!");
                setTarget(keyRoom.keyTile);
            }
            else {
                Debug.Log("Nope, gotta find that key!");
                Debug.Log("Is there a monster with the key in the room?");
                HexagonTile tileWithKeyMonster = lookForKeyMonstersInRoom();
                if (tileWithKeyMonster != null) {
                    Debug.Log("YES! GOTTA KILL IT!");
                    setTarget(tileWithKeyMonster);
                }
                else{
                    Debug.Log("Nope, there is no monster with key here!");
                    explore();
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
        Performing = false;
    }

    //ACTIONS////////////////////////////////////////////////////////////////////
    private void explore() {
        if (targetTile == null) setPriorityTarget(); 
        else setTarget(targetTile);
    }

    private void setTarget(HexagonTile tile) {        
        targetTile = tile;
        if (!goTowardsTarget()) setPriorityTarget(); 
        else Debug.Log($"My target is the door at {targetTile.HexagonCoordinates}!");
    }

    //CHECKING METHODS////////////////////////////////////////////////////////////////////
    private bool gotTheKey() {
        if (unit.hasKey) return true;
        return false;
    }

    private Room alreadyVisitedEndRoom() {
        foreach (Room room in visitedRooms) {
            if (room.hasEndTile) return room;
        }
        return null;
    }

    private Room alreadyVisitedKeyRoom() {
        foreach (Room room in visitedRooms) {
            if (room.hasKeyTile) return room;
        }
        return null;
    }

    private HexagonTile lookForKeyMonstersInRoom() {
        if (unit.onTile.originalTileType == TileType.Door) {
            foreach (Room room in unit.onTile.GetComponent<Door>().roomsAvailable) {
                foreach (HexagonTile tile in room.monstersInRoom()) {
                    if (tile.unitOn.hasKey) return tile;
                }
            } 
        }
        else {
            foreach (HexagonTile tile in unit.onTile.room.monstersInRoom()) {
                if (tile.unitOn.hasKey) return tile;
            }
        }
        return null;
    }

    //AUX METHODS////////////////////////////////////////////////////////////////////
    private void checkRoom() {
        if (unit.onTile.originalTileType == TileType.Door) {
            foreach (Room room in unit.onTile.GetComponent<Door>().roomsAvailable) {
                if (!visitedRooms.Contains(room)) visitedRooms.Add(room);
            } 
        }
        else {
            if (!visitedRooms.Contains(unit.onTile.room)) visitedRooms.Add(unit.onTile.room);
        }
    }

    private void setPriorityTarget() {
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
        for (int i = 0; i < possibleTargets.Count; i++) {
            HexagonTile temp = possibleTargets[i];
            int randomIndex = Random.Range(i, possibleTargets.Count);
            possibleTargets[i] = possibleTargets[randomIndex];
            possibleTargets[randomIndex] = temp;
        }
        
        foreach(HexagonTile possibleTarget in possibleTargets) {
            targetTile = possibleTarget;
            if (goTowardsTarget()) {
                Debug.Log($"Changed my target to the tile at {targetTile.HexagonCoordinates}!");
                return;
            }
        }
        
        //If every target has failed, deselect unit
        unit.spendActionPoint();
        unitManager.handleUnitSelection(this.gameObject);
    }

    private bool goTowardsTarget() {
        targetTileInRange = unitManager.findPath(targetTile.HexagonCoordinates);
        if (targetTileInRange == null) return false;

        return true;
    }

    IEnumerator selectCharacter() {
        characterSelected = false;
        unitManager.handleUnitSelection(this.gameObject);
        yield return new WaitForSeconds(WAITING_TIME);
        characterSelected = true;
    }

    IEnumerator selectTile() {
        tileSelected = false;
        if (targetTile.hasPickUp() || targetTile.isOccupied() && notWalkableTargetTileInRange()) targetTileInRange = targetTile;
        unitManager.handleTileSelection(targetTileInRange.gameObject);
        yield return new WaitForSeconds(WAITING_TIME);
        tileSelected = true;
    }

    private bool notWalkableTargetTileInRange() {
        return unitManager.Neighbours(targetTile, targetTileInRange);
    }
}
