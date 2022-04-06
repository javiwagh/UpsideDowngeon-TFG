using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryDecisionTree
{
    private BehaviorNode root;

    public BinaryDecisionTree(BehaviorNode root) {
        this.root = root;
    }

    public void Insert(BehaviorNode node, bool isAffirmative) {
        root.Insert(node, isAffirmative);
    }

    public void Evaluate() {
        root.Evaluate();
    }
}
