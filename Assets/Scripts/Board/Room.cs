using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Door> doors;
    public HexagonTile[] tilesInRoom;
    public bool hasEndTile;
    public bool hasKeyTile;
    public HexagonTile endTile;
    public HexagonTile keyTile;
    /*private void Awake() {
        UpdateTiles();
    }*/

    public void UpdateTiles() {
        tilesInRoom = GetComponentsInChildren<HexagonTile>();
        foreach (HexagonTile tile in tilesInRoom) {
            if (tile.originalTileType == TileType.End) {
                hasEndTile = true;
                endTile = tile;
            } 
            if (tile.originalTileType == TileType.Key) {
                hasKeyTile = true;
                keyTile = tile;
            }
            tile.room = this;
        }
    }

    public bool checkAvailableRoom() {
        foreach (Door door in doors) {
            HexagonTile tile = door.GetComponent<HexagonTile>();
            if (tile.isOccupied() && tile.unitOn.GetComponent<Character>().side == Side.Adventurers) return false;
        }
        foreach (HexagonTile tile in tilesInRoom) {
            if (tile.isOccupied() && tile.unitOn.GetComponent<Character>().side == Side.Adventurers) return false;
        }
        return true;
    }

    public List<HexagonTile> monstersInRoom() {
        List<HexagonTile> result = new List<HexagonTile>();
        foreach (HexagonTile tile in tilesInRoom) {
            if (tile.isOccupied() && tile.unitOn.GetComponent<Character>().side == Side.Monsters) result.Add(tile);
        }
        return result;
    }
}
