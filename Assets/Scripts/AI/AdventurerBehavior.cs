using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerBehavior : MonoBehaviour
{
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
        StartCoroutine(PerformTurnCoroutine());
    }

    IEnumerator PerformTurnCoroutine() {
        //Try to go towards a door
        StartCoroutine(selectCharacter());
        while (!characterSelected) yield return null;

        if (gotTheKey()) {
            Room endRoom = alreadyVisitedEndRoom();
            if (alreadyVisitedEndRoom() != null) {
                setTarget(endRoom.endTile);
            }
            else if (targetTile == null) setRandomTarget();
            else setTarget(targetTile);
        }
        else if (targetTile == null) setRandomTarget(); 
        else setTarget(targetTile);

        checkTarget();
        ////////////////////////////////////////////////////////////////////

        if (targetTileInRange != null) {
            StartCoroutine(selectTile());
            while (!tileSelected) yield return null;

            StartCoroutine(selectTile());
            while (!tileSelected) yield return null;

            if (targetTile == targetTileInRange) targetTile = null;
        }

        Performing = false;
    }

    private void setTarget(HexagonTile tile) {        
        targetTile = tile;
        Debug.Log($"My target is the door at {targetTile.HexagonCoordinates}!");
    }

    private void setRandomTarget() {
        HexagonTile currentTile = unit.onTile;
        
        if (currentTile.originalTileType == TileType.Door) {
            List<Room> roomsAvailable = currentTile.GetComponent<Door>().roomsAvailable;
            Room targetRoom = roomsAvailable[Random.Range(0, roomsAvailable.Count)];
            Door targetDoor = targetRoom.doors[Random.Range(0, targetRoom.doors.Count)];
            targetTile = targetDoor.GetComponent<HexagonTile>();
            
        }
        else
        {
            Room targetRoom = currentTile.GetComponent<HexagonTile>().room;
            Door targetDoor = targetRoom.doors[Random.Range(0, targetRoom.doors.Count)];
            targetTile = targetDoor.GetComponent<HexagonTile>();
        }
        Debug.Log($"My target is the door at {targetTile.HexagonCoordinates}!");
    }

    private void checkTarget() {
        int timesTriedToReachTarget = 0;
        
        while (timesTriedToReachTarget < 6) {
            if (goTowardsTarget()) return;
            setRandomTarget();
            ++timesTriedToReachTarget;
        }
        
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
        yield return new WaitForSeconds(1f);
        characterSelected = true;
    }

    IEnumerator selectTile() {
        tileSelected = false;
        unitManager.handleTileSelection(targetTileInRange.gameObject);
        yield return new WaitForSeconds(1f);
        tileSelected = true;
    }

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
}
