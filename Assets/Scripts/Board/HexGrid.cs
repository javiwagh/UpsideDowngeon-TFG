using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    Dictionary<Vector3Int, HexagonTile> hexagonTileDictionary = new Dictionary<Vector3Int, HexagonTile>();
    Dictionary<Vector3Int, List<Vector3Int>> hexagonTileNeighboursDictionary = new Dictionary<Vector3Int, List<Vector3Int>>();

    private void Awake() {
        UpdateTiles();
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

    public Vector3Int GetClosestTile(Vector3 worldPosition) {
        worldPosition.y = 0;
        return HexCoord.calculateConvertPosition(worldPosition);
    }

    public List<HexagonTile> GetEveryWalkableTiles() {
        List<HexagonTile> walkableTiles = new List<HexagonTile>();
        foreach (Vector3Int tilePosition in hexagonTileDictionary.Keys) {
            HexagonTile tile = getTileAt(tilePosition);
            if (tile.isWalkable()) walkableTiles.Add(tile);
        }
        return walkableTiles;
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
