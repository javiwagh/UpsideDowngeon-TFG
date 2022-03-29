using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class HexagonTile : MonoBehaviour
{
    [SerializeField]
    private GlowHighlight highlight;
    private HexCoord hexCoord;
    public Vector3Int HexagonCoordinates => hexCoord.getHexCoordinates();

    private void Awake(){
        hexCoord = GetComponent<HexCoord>();
        highlight = GetComponent<GlowHighlight>();
    }

    public void EnableHighlight() {
        highlight.ToggleGlow(true);
    }

    public void DisableHighlight() {
        highlight.ToggleGlow(false);
    }
}
