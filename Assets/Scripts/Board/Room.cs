using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Room : MonoBehaviour
{
    public List<HexagonTile> tilesInRoom;
    public List<HexagonTile> doors;
    private void Awake() {
        tilesInRoom = GetComponentsInChildren<HexagonTile>().ToList();
    }

    public void UpdateTiles() {
        tilesInRoom = GetComponentsInChildren<HexagonTile>().ToList();
    }

    public bool checkAvailableRoom() {
        foreach (HexagonTile tile in tilesInRoom) {
            if (tile.isOccupied() && tile.unitOn.GetComponent<Character>().side == Side.Adventurers) return false;
        }
        return true;
    }

    public bool checkTileTypeInRoom(TileType target) {
        foreach (HexagonTile tile in tilesInRoom) {
            if (tile.originalTileType == target) return true;
        }
        return false;
    }
}
