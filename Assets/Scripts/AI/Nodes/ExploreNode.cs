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
        if (me.currentTarget == null || me.currentTarget == me.GetComponent<Unit>().onTile) {
            List<HexagonTile> doorsAvailable = new List<HexagonTile>();
            foreach (Room room in me.lastKnownRoom) {
                foreach (HexagonTile door in room.doors) doorsAvailable.Add(door);
            }
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
        else this.target = me.currentTarget;
        Debug.Log($"OK! I'll explore door at {me.currentTarget.HexagonCoordinates}");
    }

    public override void Evaluate()
    {        
        setTarget();       
        me.GoTowards();
    }
}
