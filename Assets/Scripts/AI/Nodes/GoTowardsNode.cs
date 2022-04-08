using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoTowardsNode : BehaviorNode
{
    private AdventurerBehavior me;
    TileType target;

    public GoTowardsNode(TileType targetType, AdventurerBehavior me) {
        this.target = targetType;
        this.me = me;
    }

    public override void Evaluate()
    {
        if (target == TileType.End) me.currentTarget = me.endTile;
        else me.currentTarget = me.keyTile;
        me.GoTowards();
    }
}
