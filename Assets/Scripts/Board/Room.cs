using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public HexagonTile[] tilesInRoom;
    private void Awake() {
        tilesInRoom = GetComponentsInChildren<HexagonTile>();
    }

    public bool checkAvailableRoom() {
        foreach (HexagonTile tile in tilesInRoom) {
            if (tile.isOccupied() && tile.unitOn.GetComponent<Character>().side == Side.Adventurers) return false;
        }
        return true;
    }
}
