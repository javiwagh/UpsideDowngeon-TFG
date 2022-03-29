using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class HexagonTile : MonoBehaviour
{
    private HexCoord hexCoord;
    public Vector3Int HexagonCoordinates => hexCoord.getHexCoordinates();

    private void Awake(){
        hexCoord = GetComponent<HexCoord>();
    }
}
