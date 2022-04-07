using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoTowardsNode : BehaviorNode
{
    private AdventurerBehavior me;
    HexagonTile target;

    public GoTowardsNode(HexagonTile targetTile, AdventurerBehavior me) {
        this.target = targetTile;
        this.me = me;
    }

    public override void Evaluate()
    {
        me.currentTarget = target;
        me.GoTowards(target);
    }
}
