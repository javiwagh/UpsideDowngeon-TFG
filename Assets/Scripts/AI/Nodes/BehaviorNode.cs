using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorNode {
    protected (BehaviorNode, BehaviorNode) children;
    public abstract void Evaluate();

    public void Insert(BehaviorNode node) {
        if (children.Item1 == null) children.Item1 = node;
        else if (children.Item2 == null) children.Item2 = node;
    }

    public void Yes () {
        Debug.Log("Yes!");
        children.Item1.Evaluate();
    }

    public void No () {
        Debug.Log("Nope");
        children.Item2.Evaluate();
    }
}
