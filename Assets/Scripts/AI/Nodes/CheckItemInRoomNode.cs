using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckItemInRoomNode : BehaviorNode
{
    private Unit me;
    private TileType target;

    public CheckItemInRoomNode(TileType targetType, Unit me) {
        this.target = targetType;
        this.me = me;
    }

    public override void Evaluate()
    {
        Debug.Log($"Is there any {target} in this room?");
        //Figure out in which room am I
        //Every tile should be in a room
        List<Room> currentRooms = me.GetComponent<Unit>().onTile.rooms;
        //Check
        foreach (Room room in currentRooms) {
            if (room.checkTileTypeInRoom(target)) {
                Yes();
                return;
            }
        }
        No();
    }
}
