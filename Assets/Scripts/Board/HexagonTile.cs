using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[SelectionBase]
public class HexagonTile : MonoBehaviour
{
    [SerializeField]
    private GlowHighlight highlight;
    [SerializeField]
    private TextMeshPro costTMP;

    [SerializeField]
    private TileType tileType;

    public TileType originalTileType;

    private HexCoord hexCoord;
    public Unit unitOn;
    public Vector3Int HexagonCoordinates => hexCoord.getHexCoordinates();

    public bool isSpawn = true;
    public Room room;

    private void Awake(){
        hexCoord = GetComponent<HexCoord>();
        highlight = GetComponent<GlowHighlight>();        
        setOriginalType();
        updateCost();
    }

    private void updateCost() {
        int cost = getCost();
        if (cost < 10) costTMP.SetText(getCost().ToString());
        else costTMP.SetText(string.Empty);
        costTMP.enabled = false;
    }

    public void setOriginalType() {
        tileType = originalTileType;
    }

    public int getCost() {
        int cost = 0;
        switch (tileType)
        {
            case TileType.Default:
                cost = 1;
                break;
            case TileType.Start:
                cost = 1;
                break;
            case TileType.Door:
                cost = 2;
                break;
            case TileType.EndAvailable:
                cost = 2;
                break;
            default:
                cost = 10;
                Debug.Log($"Not suppoted tile type ({tileType})");
                break;            
        }
        return cost;
    }

    public bool isWalkable() {
        return !(this.tileType == TileType.Obstacle || this.tileType == TileType.Occupied || this.tileType == TileType.Key || this.tileType == TileType.End);
    }

    public bool IsSpawn() {
        return (isWalkable() && isSpawn);
    }

    public bool isOccupied() {
        return this.tileType == TileType.Occupied;
    }

    public bool isStageEnd() {
        return this.tileType == TileType.End;
    }

    public bool hasPickUp() {
        return this.tileType == TileType.Key;
    }

    public void stepOnTile(Unit unit) {
        this.unitOn = unit;
        unit.onTile = this;
        tileType = TileType.Occupied;
    }

    public void resetTileType() {
        this.unitOn = null;
        this.tileType = originalTileType;
        updateCost();
    }

    public void enableEnd() {
        if (tileType == TileType.End) tileType = TileType.EndAvailable;
        updateCost();
    }

    public bool isEnd() {
        return originalTileType == TileType.End;
    }

    public void EnableHighlight() {
        highlight.ToggleGlow(true);
        costTMP.enabled = true;
    }

    public void DisableHighlight() {
        highlight.ToggleGlow(false);
        costTMP.enabled = false;
    }

    public void ResetHighlight() {
        highlight.ResetHighlight();
    }

    public void HighlightPath() {
        highlight.HighlightPath();
    }

    public void HighlightTarget() {
        highlight.HighlightTarget();
    }
}

public enum TileType {
    None,
    Default,
    Door,
    Start,
    End,
    EndAvailable, 
    Obstacle,
    Occupied,
    Key
}
