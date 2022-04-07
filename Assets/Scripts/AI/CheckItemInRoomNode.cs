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
        //Figure out in which room am I
        //Every tile should be in a room
        //Door rooms will be set in one or another room at convenience of the design
        Room currentRoom = me.GetComponent<Unit>().onTile.GetComponentInParent<Room>();
        //Check
        if (currentRoom.checkTileTypeInRoom(target)) Yes();
        else No();
    }
}
