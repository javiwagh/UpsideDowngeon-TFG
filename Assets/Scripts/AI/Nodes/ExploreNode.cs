using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreNode : BehaviorNode
{
    private AdventurerBehavior me;
    private HexagonTile target;
    private TileType foundInRoom;

    public ExploreNode(TileType foundInRoom, AdventurerBehavior me) {
        target = null;
        this.me = me;
        this.foundInRoom = foundInRoom;
    }

    private void setTarget() {
        if (me.currentTarget == null || me.currentTarget == me.GetComponent<Unit>().onTile) {
            List<HexagonTile> doorsAvailable = new List<HexagonTile>();
            foreach (Room room in me.lastKnownRoom) {
                if (me.roomsInfo.ContainsKey(room)) me.roomsInfo[room] = foundInRoom;
                else foreach (HexagonTile door in room.doors) doorsAvailable.Add(door);
            }
            if (doorsAvailable != new List<HexagonTile>()) {
                Debug.Log($"Ok, I got {doorsAvailable.Count} doors available in this room.");
                int random = Random.Range(0, doorsAvailable.Count);
                this.target = doorsAvailable[random];
                me.currentTarget = target;
                while (me.currentTarget == me.GetComponent<Unit>().onTile) {
                    random = Random.Range(0, doorsAvailable.Count);
                    this.target = doorsAvailable[random];
                    me.currentTarget = target;
                }
            }
            else me.currentTarget = null;
        }
        else this.target = me.currentTarget;
        Debug.Log($"OK! I'll explore door at {me.currentTarget.HexagonCoordinates}");
    }

    public override void Evaluate()
    {
        setTarget();       
        me.GoTowards();
    }
}
