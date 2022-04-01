using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSearch
{
    public static BFSearch BFSGetRange(HexGrid hexGrid, Vector3Int startTile, int points) {
        Dictionary<Vector3Int, Vector3Int?> VisitedNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisit = new Queue<Vector3Int>();

        nodesToVisit.Enqueue(startTile);
        costSoFar.Add(startTile, 0);
        VisitedNodes.Add(startTile, null);

        while (nodesToVisit.Count > 0) {
            Vector3Int currentNode = nodesToVisit.Dequeue();
            foreach(Vector3Int neihbourPosition in hexGrid.getNeightbours(currentNode)) {
                if (hexGrid.getTileAt(neihbourPosition).isWalkable()) {
                    int nodeCost = hexGrid.getTileAt(neihbourPosition).getCost();
                    int currentCost = costSoFar[currentNode];
                    int newCost = currentCost + nodeCost;

                    if(newCost <= points) {
                        if (!VisitedNodes.ContainsKey(neihbourPosition)) {
                            VisitedNodes[neihbourPosition] = currentNode;
                            costSoFar[neihbourPosition] = newCost;
                            nodesToVisit.Enqueue(neihbourPosition);
                        }
                        else if (costSoFar[neihbourPosition] > newCost) {
                            costSoFar[neihbourPosition] = newCost;
                            VisitedNodes [neihbourPosition] = currentNode;
                        }
                    }
                }
                else continue;
            }
        }

        return new BFSearch {visitedNodes = VisitedNodes};
    }

    public static List<Vector3Int> GeneratePathBFS(Vector3Int currentPosition, Dictionary<Vector3Int, Vector3Int?> visitedNodes) {
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(currentPosition);
        while (visitedNodes[currentPosition] != null) {
            path.Add(visitedNodes[currentPosition].Value);
            currentPosition = visitedNodes[currentPosition].Value;
        }
        path.Reverse();
        return path.Skip(1).ToList();
    }
}

public struct BFSearch {
    public Dictionary<Vector3Int, Vector3Int?> visitedNodes;

    public List<Vector3Int> getPathTo(Vector3Int destination) {
        if (!visitedNodes.ContainsKey(destination)) {
            return new List<Vector3Int>();
        } 
        return GraphSearch.GeneratePathBFS(destination, visitedNodes);
    }

    public bool tileInRange(Vector3Int position){
        return visitedNodes.ContainsKey(position);
    }

    public IEnumerable<Vector3Int> getRangePositions() {
        if (visitedNodes == null) return null;
        return visitedNodes.Keys;
    } 

    public void RemoveFromRange(Vector3Int tilePosition) {
        visitedNodes.Remove(tilePosition);
    }
}
