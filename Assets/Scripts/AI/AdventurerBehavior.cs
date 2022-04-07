using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerBehavior : MonoBehaviour
{
    BinaryDecisionTree behaviorTree;
    [SerializeField]
    private HexagonTile endTile;
    [SerializeField]
    private HexagonTile keyTile;
    [SerializeField]
    private UnitManager unitManager;

    private HexagonTile selectedTile;
    private bool unitSelectionDone = false;
    private bool pathSelectionDone = false;
    public bool actionEnded = false;

    public void buildBehaviorTree() {
        Unit me = this.GetComponent<Unit>();
        //Level 0
        //Do I have the key?
        CheckKeyNode root = new CheckKeyNode(me);
        behaviorTree = new BinaryDecisionTree(root);

        //Level 1
        //Yes. Is end tile in this room?
        CheckItemInRoomNode endTileNode = new CheckItemInRoomNode(TileType.End, me);
        root.Insert(endTileNode);
        //No. Is key in this room?
        CheckItemInRoomNode keyTileNode = new CheckItemInRoomNode(TileType.Key, me);
        root.Insert(keyTileNode);

        //Level 2
        //endTileRoom children
        //Yes. Go to end tile
        GoTowardsNode goToEnd = new GoTowardsNode(endTile, this);
        endTileNode.Insert(goToEnd);
        //No. Explore towards another room
        GoTowardsNode goExplore = new GoTowardsNode(endTile, this);
        endTileNode.Insert(goExplore);
        //keyRoomNode children
        //Yes. Am I next to the key?
        keyTileNode.Insert(goExplore);
        //No. Explore towards another room
        keyTileNode.Insert(goExplore);
    }
    public void executeAction() {
        unitSelectionDone = false;
        pathSelectionDone = false;
        actionEnded = false;
        Unit logicalUnit = this.GetComponent<Unit>();        
        Debug.Log($"{logicalUnit.character.name} is executing an action");

        behaviorTree.Evaluate();
    }

    public void GoTowards(HexagonTile targetTile)
    {
        StartCoroutine(GoTowardsCoroutine(targetTile));
    }

    private IEnumerator GoTowardsCoroutine(HexagonTile targetTile) {
        StartCoroutine(selectUnit());
        while (!unitSelectionDone) yield return null;
        StartCoroutine(selectPath(targetTile));
        while (!pathSelectionDone) yield return null;
        StartCoroutine(followPath()); 
        while (!actionEnded) yield return null;
    }
    private IEnumerator selectUnit() {
        unitManager.handleUnitSelection(this.gameObject);
        yield return new WaitForSeconds(1f);
        unitSelectionDone = true;
    }
    private IEnumerator selectPath(HexagonTile targetTile) {
        selectedTile = unitManager.selectTileTowards(this.transform.position, targetTile);
        yield return new WaitForSeconds(1f);
        pathSelectionDone = true;
    }
    private IEnumerator followPath() {
        unitManager.handleTileSelection(selectedTile.gameObject);
        while (this.GetComponent<Unit>().isMoving) yield return null;
        actionEnded = true;
    }
}