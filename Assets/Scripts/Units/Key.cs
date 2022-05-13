using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField]
    private GameObject onKeyPickedTile;
    /*private void Awake() {
        onKeyPickedTile.SetActive(false);
    }*/
    public void Pick() {
        onKeyPickedTile.SetActive(true);
        this.gameObject.SetActive(false);
        this.GetComponent<HexagonTile>().room.hasKeyTile = false;
    }

    public void setTile(GameObject tile) {
        onKeyPickedTile = tile;
        onKeyPickedTile.gameObject.SetActive(false);
    }
}
