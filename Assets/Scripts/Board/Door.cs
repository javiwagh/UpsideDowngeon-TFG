using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<Room> roomsAvailable = new List<Room>();
    private HexGrid hexGrid;
    private void Awake() {
        hexGrid = FindObjectOfType<HexGrid>();
    }
    private void Start() {
        List<Vector3Int> neighbors = hexGrid.getNeightbours(this.GetComponent<HexagonTile>().HexagonCoordinates);
        foreach (Vector3Int neighbor in neighbors) {
            Room room = hexGrid.getTileAt(neighbor).room;
            if (!roomsAvailable.Contains(room)) roomsAvailable.Add(room);
        }
    }
}
