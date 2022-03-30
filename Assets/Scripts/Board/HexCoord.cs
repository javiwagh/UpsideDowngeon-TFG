using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoord : MonoBehaviour
{
    public static float xOffset = 2f, yOffset = 1f, zOffset = 1.73f;
    [Header("Offset coordinates")]
    [SerializeField]
    private Vector3Int offsetCoordinates;

    private void Awake() {
        offsetCoordinates = calculateConvertPosition(transform.position);
    }

    public static Vector3Int calculateConvertPosition(Vector3 position) {
        int x = Mathf.CeilToInt(position.x / xOffset);
        int y = Mathf.RoundToInt(position.y / yOffset);
        int z = Mathf.RoundToInt(position.z / zOffset);
        return new Vector3Int(x, y, z);
    }

    internal Vector3Int getHexCoordinates(){
        return offsetCoordinates;
    }
}
