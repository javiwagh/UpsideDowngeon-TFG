using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerBehavior : MonoBehaviour
{
    public HexagonTile currentTarget = null;
    public List<Room> lastKnownRoom = null;
    BinaryDecisionTree behaviorTree;
    [SerializeField]
    public HexagonTile endTile;
    [SerializeField]
    public HexagonTile keyTile;
    [SerializeField]
    private UnitManager unitManager;
    private Unit logicalUnit;

    private HexagonTile selectedTile;
    private bool treeBuilt = false;
    private bool unitSelectionDone = false;
    private bool pathSelectionDone = false;
    public bool actionEnded = false;
    public Dictionary<Room, TileType> roomsInfo;

    private void Awake() {
        logicalUnit = this.GetComponent<Unit>();
        roomsInfo = new Dictionary<Room, TileType>();
    }

    public void buildBehaviorTree() {
        Unit me = logicalUnit;
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
            //endRoomNode children
            //Yes. Go to end tile!
            GoTowardsNode goToEnd = new GoTowardsNode(TileType.End, this);
            endRoomNode.Insert(goToEnd);
            //No. Explore towards another room. Save empty. 
            ExploreNode goExploreAfterEmpty = new ExploreNode(TileType.Default, this);     
            endRoomNode.Insert(goExploreAfterEmpty);

            //keyRoomNode children
            //Yes. Am I next to the key?
            NextToKeyNode nextToKey = new NextToKeyNode(this);
            keyRoomNode.Insert(nextToKey);
            //No. Is end in this room?
            CheckItemInRoomNode endRoomNode2 = new CheckItemInRoomNode(TileType.End, me);
            keyRoomNode.Insert(endRoomNode2);

        //LEVEL 3
            //nextToKey children
            //Yes. Take the key!
            PickUpNode takeKey = new PickUpNode(this, keyTile);
            nextToKey.Insert(takeKey);
            //No. Go towards the key!
            GoTowardsNode goToKey = new GoTowardsNode(TileType.Key, this);
            nextToKey.Insert(goToKey);

            //endRoomNode2 children
            //Yes. Save end!
            ExploreNode goExploreAfterEnd = new ExploreNode(TileType.End, this);  
            endRoomNode2.Insert(goExploreAfterEnd);
            //No. Save empty.
            endRoomNode2.Insert(goExploreAfterEmpty);
    }
    public void executeAction() {
        unitSelectionDone = false;
        pathSelectionDone = false;
        actionEnded = false;
        updateKey();
        
        lastKnownRoom = logicalUnit.onTile.rooms;

        if (!treeBuilt) buildBehaviorTree();
        Debug.Log($"{logicalUnit.character.name} is executing an action");

        behaviorTree.Evaluate();
    }

    public void updateKey() {
        keyTile = unitManager.UpdateKey();
    }

    public void GoTowards()
    {
        StartCoroutine(GoTowardsCoroutine());
    }

    public void PickUp() {
        StartCoroutine(PickUpCoroutine());
    }

    public bool NextToKey() {
        return unitManager.checkNextToKey(logicalUnit.onTile.HexagonCoordinates);
    }

    private IEnumerator GoTowardsCoroutine() {
        StartCoroutine(selectUnit());
        while (!unitSelectionDone) yield return null;
        currentTarget = unitManager.correctTargetTile(this.transform.position, currentTarget);
        StartCoroutine(selectPath());
        while (!pathSelectionDone) yield return null;
        StartCoroutine(followPath()); 
        while (!actionEnded) yield return null;
    }

    private IEnumerator PickUpCoroutine() {
        StartCoroutine(selectUnit());
        while (!unitSelectionDone) yield return null;
        StartCoroutine(selectTile());
        while (!actionEnded) yield return null;
    }

    private IEnumerator selectUnit() {
        unitManager.handleUnitSelection(this.gameObject);
        yield return new WaitForSeconds(1f);
        unitSelectionDone = true;
    }
    private IEnumerator selectPath() {
        selectedTile = unitManager.selectTileTowards(this.transform.position, currentTarget);
        if (selectedTile == currentTarget) currentTarget = null;
        yield return new WaitForSeconds(1f);
        pathSelectionDone = true;
    }
    private IEnumerator followPath() {
        unitManager.handleTileSelection(selectedTile.gameObject);
        while (this.GetComponent<Unit>().isMoving) yield return null;
        actionEnded = true;
    }

    private IEnumerator selectTile() {
        unitManager.handleTileSelection(currentTarget.gameObject);
        actionEnded = true;
        currentTarget = null;
        yield return null;
    }
}