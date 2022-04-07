using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    Dictionary<Vector3Int, HexagonTile> hexagonTileDictionary = new Dictionary<Vector3Int, HexagonTile>();
    Dictionary<Vector3Int, List<Vector3Int>> hexagonTileNeighboursDictionary = new Dictionary<Vector3Int, List<Vector3Int>>();

    Room[] romsInBoard;

    private void Awake() {
        UpdateTiles();
        romsInBoard = GetComponentsInChildren<Room>();
    }

    public void UpdateTiles() {
        foreach (HexagonTile tile in FindObjectsOfType<HexagonTile>()) {
            //Debug.Log(tile.gameObject.name);
            if (tile.isActiveAndEnabled) hexagonTileDictionary[tile.HexagonCoordinates] = tile;
        }
    }

    public HexagonTile getTileAt(Vector3Int coords){
        HexagonTile tile = null;
        hexagonTileDictionary.TryGetValue(coords, out tile);
        return tile;
    }

    public List<Vector3Int> getNeightbours (Vector3Int coords) {
        if (hexagonTileDictionary.ContainsKey(coords) == false) //If not found, return empty list
            return new List<Vector3Int>();

        if (hexagonTileNeighboursDictionary.ContainsKey(coords)) //If neighbours have been already listed
            return hexagonTileNeighboursDictionary[coords];

        //Else, neighbours must be listed
        hexagonTileNeighboursDictionary.Add(coords, new List<Vector3Int>());
        foreach (Vector3Int direction in Direction.getDirectionList(coords.z)) {
            if (hexagonTileDictionary.ContainsKey(coords + direction)) 
                hexagonTileNeighboursDictionary[coords].Add(coords + direction);
        }
        return hexagonTileNeighboursDictionary[coords];
    }

    public HexagonTile findClosestNeighbor(Vector3 origin, HexagonTile tile) {
        List<Vector3Int> neighbors = getNeightbours(tile.HexagonCoordinates);
        
        float distance;
        float shortestDistance = float.PositiveInfinity;
        HexagonTile closestNeighbor = null;

        foreach (Vector3Int tilePosition in neighbors) {
            HexagonTile neighbor = getTileAt(tilePosition);
            if (neighbor.isWalkable()){
                distance = Vector3.Distance(origin, neighbor.transform.position);
                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    closestNeighbor = neighbor;
                }
            }
        }
        return closestNeighbor;
    }

    public Vector3Int GetClosestTile(Vector3 worldPosition) {
        worldPosition.y = 0;
        return HexCoord.calculateConvertPosition(worldPosition);
    }

    public List<HexagonTile> GetEverySpawnTiles() {
        List<HexagonTile> spawnTiles = new List<HexagonTile>();
        foreach (Room room in romsInBoard) {
            if (room.checkAvailableRoom()){
                foreach (HexagonTile tile in room.tilesInRoom) {
                    if (tile.IsSpawn()) spawnTiles.Add(tile);
                }
            }
        }
        return spawnTiles;
    }
}

public static class Direction {
    public static List<Vector3Int> directionOffsetEven = new List<Vector3Int> {
        
        new Vector3Int(0, 0, 1), //NorthWest
        new Vector3Int(1, 0, 1), //NorthEast
        new Vector3Int(1, 0, 0), //East        
        new Vector3Int(1, 0, -1), //SouthEast
        new Vector3Int(0, 0, -1), //SouthWest
        new Vector3Int(-1, 0, 0) //West
    };

    public static List<Vector3Int> directionOffsetOdd = new List<Vector3Int> {
        new Vector3Int(-1, 0, 1), //NorthWest
        new Vector3Int(0, 0, 1), //NorthEast
        new Vector3Int(1, 0, 0), //East
        new Vector3Int(0, 0, -1), //SouthEast
        new Vector3Int(-1, 0, -1), //SouthWest
        new Vector3Int(-1, 0, 0) //West
    };

    public static List<Vector3Int> getDirectionList(int z) {
        if (z % 2 == 0) return directionOffsetEven;
        return directionOffsetOdd;
    }
    
}
