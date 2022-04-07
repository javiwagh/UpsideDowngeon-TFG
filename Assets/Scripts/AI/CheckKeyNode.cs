using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckKeyNode : BehaviorNode {
    private Unit logicalUnit;

    public CheckKeyNode(Unit logicalUnit) {
        this.logicalUnit = logicalUnit;
    }

    public override void Evaluate()
    {
        Debug.Log("Do I have the Key?");
        if (logicalUnit.hasKey) Yes();
        else No();
    }
}
