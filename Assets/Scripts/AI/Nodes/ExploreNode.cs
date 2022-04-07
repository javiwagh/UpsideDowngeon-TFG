using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreNode : BehaviorNode
{
    private AdventurerBehavior me;
    HexagonTile target;

    public ExploreNode(AdventurerBehavior me) {
        target = null;
        this.me = me;
    }

    private void setTarget() {
        if (me.currentTarget == null) {
            Debug.Log($"Ok, I got {me.lastKnownRoom.doors.Count} doors available in this room.");
            int random = Random.Range(0, me.lastKnownRoom.doors.Count);
            this.target = me.lastKnownRoom.doors[random];
            me.currentTarget = target;   
        }
        else this.target = me.currentTarget;
        Debug.Log($"OK! I'll explore door at {me.currentTarget.HexagonCoordinates}");
    }

    public override void Evaluate()
    {        
        setTarget();       
        me.GoTowards(target);
    }
}
