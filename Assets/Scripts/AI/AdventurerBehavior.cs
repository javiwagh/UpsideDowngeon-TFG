using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerBehavior : MonoBehaviour
{
    BinaryDecisionTree behaviorTree;
    public void buildBehaviorTree() {
        //Level 0
        behaviorTree = new BinaryDecisionTree(new CheckKeyNode(this.GetComponent<Unit>()));
        //Level 1
        behaviorTree.Insert(new TestNode(true), true);
        behaviorTree.Insert(new TestNode(false), false);
    }
    public void executeAction() {
        Unit logicalUnit = this.GetComponent<Unit>();        
        Debug.Log($"{logicalUnit.character.name} is executing an action");

        /*if (logicalUnit.hasKey) {
            if (Is end in this room?) GoTo(End);
            else GoTo(Some Door);
        }            
        else if (Is Key in this Room?)
        */
        behaviorTree.Evaluate();        

        this.GetComponent<Unit>().actionPoints -= 1;
    }
}