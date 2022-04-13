using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSearch
{
    public static BFSearch BFSGetRange(HexGrid hexGrid, Vector3Int startTile, int points, bool isMonster) {
        Dictionary<Vector3Int, Vector3Int?> VisitedNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisit = new Queue<Vector3Int>();

        List<Vector3Int> Units = new List<Vector3Int>();
        List<Vector3Int> PickUps = new List<Vector3Int>();

        nodesToVisit.Enqueue(startTile);
        costSoFar.Add(startTile, 0);
        VisitedNodes.Add(startTile, null);

        while (nodesToVisit.Count > 0) {
            Vector3Int currentNode = nodesToVisit.Dequeue();
            foreach(Vector3Int neihbourPosition in hexGrid.getNeightbours(currentNode)) {
                HexagonTile tile = hexGrid.getTileAt(neihbourPosition);
                if (tile.isWalkable()) {
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
                else if (tile.isOccupied() && neihbourPosition != startTile) {
                    if (isMonster && tile.unitOn.GetComponent<Character>().side != Side.Monsters) 
                             Units.Add(tile.HexagonCoordinates);
                    else if (!isMonster && tile.unitOn.GetComponent<Character>().side != Side.Adventurers) 
                        Units.Add(tile.HexagonCoordinates);
                    
                }
                else if (tile.hasPickUp()) {
                    PickUps.Add(tile.HexagonCoordinates);
                }
            }
        }

        return new BFSearch {visitedNodes = VisitedNodes, units = Units, pickUps = PickUps};
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
    public  List<Vector3Int> units;
    public List<Vector3Int> pickUps;

    public List<Vector3Int> getPathTo(Vector3Int destination) {
        if (!visitedNodes.ContainsKey(destination) && !units.Contains(destination) && !pickUps.Contains(destination)) {
            return new List<Vector3Int>();
        } 
        return GraphSearch.GeneratePathBFS(destination, visitedNodes);
    }

    public List<Vector3Int> checkPathTo(Vector3Int destination) {
        if (!visitedNodes.ContainsKey(destination)) {
            return new List<Vector3Int>();
        } 
        return GraphSearch.GeneratePathBFS(destination, visitedNodes);
    }

    public bool tileInRange(Vector3Int position){
        return visitedNodes.ContainsKey(position) || units.Contains(position) || pickUps.Contains(position);
    }

    public IEnumerable<Vector3Int> getRangePositions() {
        if (visitedNodes == null) return null;
        return visitedNodes.Keys;
    } 

    public IEnumerable<Vector3Int> getUnitsPositions() {
        if (units == null) return null;
        return units;
    }

    public IEnumerable<Vector3Int> getPickUpsPositions() {
        if (pickUps == null) return null;
        return pickUps;
    }

    public void RemoveFromRange(Vector3Int tilePosition) {
        visitedNodes.Remove(tilePosition);
    }
}
