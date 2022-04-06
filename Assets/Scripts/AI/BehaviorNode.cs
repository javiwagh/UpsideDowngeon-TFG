using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorNode {
    protected (BehaviorNode, BehaviorNode) children;
    private int index;
    public abstract void Evaluate();

    public void Insert(BehaviorNode node, bool isAffirmative) {
        if (isAffirmative) {
            if (children.Item1 == null) children.Item1 = node;
            else children.Item1.Insert(node, isAffirmative);
        }
        else {
            if (children.Item2 == null) children.Item2 = node;
            else children.Item2.Insert(node, isAffirmative);
        }
    }
}
