using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNode : BehaviorNode {
    private bool affirmative;

    public TestNode(bool affirmative) {
        this.affirmative = affirmative;
    }

    public override void Evaluate()
    {
        if (affirmative) Debug.Log("Yes!");
        else Debug.Log("No!");
    }
}
