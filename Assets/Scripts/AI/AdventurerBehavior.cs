using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerBehavior : MonoBehaviour
{
    public HexagonTile currentTarget = null;
    public Room lastKnownRoom = null;
    BinaryDecisionTree behaviorTree;
    [SerializeField]
    private HexagonTile endTile;
    [SerializeField]
    private HexagonTile keyTile;
    [SerializeField]
    private UnitManager unitManager;

    private HexagonTile selectedTile;
    private bool treeBuilt = false;
    private bool unitSelectionDone = false;
    private bool pathSelectionDone = false;
    public bool actionEnded = false;

    public void buildBehaviorTree() {
        Unit me = this.GetComponent<Unit>();
        //LEVEL 0
            //Do I have the key?
            CheckKeyNode root = new CheckKeyNode(me);
            behaviorTree = new BinaryDecisionTree(root);

        //LEVEL 1
            //Yes. Is end tile in this room?
            CheckItemInRoomNode endRoomNode = new CheckItemInRoomNode(TileType.End, me);
            root.Insert(endRoomNode);
            //No. Is key in this room?
            CheckItemInRoomNode keyRoomNode = new CheckItemInRoomNode(TileType.Key, me);
            root.Insert(keyRoomNode);

        //LEVEL 2
            //endTileRoom children
            //Yes. Go to end tile
            GoTowardsNode goToEnd = new GoTowardsNode(endTile, this);
            endRoomNode.Insert(goToEnd);
            //No. Explore towards another room
            ExploreNode goExplore = new ExploreNode(this);     
            endRoomNode.Insert(goExplore);

            //keyRoomNode children
            //Yes. Am I next to the key?
            GoTowardsNode goToKey = new GoTowardsNode(keyTile, this);
            keyRoomNode.Insert(goToKey);
            //No. Explore towards another room
            keyRoomNode.Insert(goExplore);
    }
    public void executeAction() {
        unitSelectionDone = false;
        pathSelectionDone = false;
        actionEnded = false;

        Unit logicalUnit = this.GetComponent<Unit>();
        if (logicalUnit.onTile.originalTileType != TileType.Door) lastKnownRoom = logicalUnit.onTile.rooms[0];

        if (!treeBuilt) buildBehaviorTree();
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