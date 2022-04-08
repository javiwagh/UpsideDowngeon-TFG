using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextToKeyNode : BehaviorNode
{
    private AdventurerBehavior me;
    private TileType target;

    public NextToKeyNode(AdventurerBehavior me) {
        this.me = me;
    }

    public override void Evaluate()
    {
        Debug.Log("Am I next to the key tile?");
        if (me.NextToKey()) Yes();
        else No();
    }
}
