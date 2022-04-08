using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpNode : BehaviorNode
{
    private AdventurerBehavior me;
    private HexagonTile target;

    public PickUpNode(AdventurerBehavior me, HexagonTile target) {
        this.me = me;
        this.target = target;
    }

    public override void Evaluate()
    {
        Debug.Log("Cool! I will take it!");
        me.currentTarget = target;
        me.PickUp();
    }
}
