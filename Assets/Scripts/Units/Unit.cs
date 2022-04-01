using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{
    private int movementPoints;
    public int MovementPoints {get => movementPoints;}

    [SerializeField]
    private float movementDuration = 0.5f, rotationDuration = 0.1f;

    private GlowHighlight glowHighlight;
    public Character character;
    public HexagonTile onTile;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();
    public event System.Action<Unit> MovementFinished;

    private void Awake() {
        glowHighlight = GetComponent<GlowHighlight>();
        character = GetComponent<Character>();
        movementPoints = character.speed;
    }

    internal void Deselect() {
        glowHighlight.ToggleGlow(false);
    }

    internal void Select() {
        glowHighlight.ToggleGlow(true);
    }

    public void moveThroughPath(List<Vector3> currentPath){
        pathPositions = new Queue<Vector3>(currentPath);
        Vector3 firstTarget = pathPositions.Dequeue();
        StartCoroutine(movingRotationCoroutine(firstTarget, rotationDuration));
    }

    public void Attack(Unit target) {
        Debug.Log($"Attacking {target.GetComponent<Character>().characterName}!");
        StartCoroutine(attackingRotationCoroutine(target.transform.position, rotationDuration));
        target.recieveDamage(this.character.meleeDamage);        
    }

    public void recieveDamage(int damage){
        this.character.healthPoints -= damage;
        if (this.character.healthPoints <= 0) {
            this.character.healthPoints = 0;
            onTile.resetTileType();
            Debug.Log($"My health is 0! I'm fainting!");
            this.gameObject.SetActive(false);
        }
        Debug.Log($"Ouch! My current health is {this.character.healthPoints}. That hurt!");
    }

    private IEnumerator movingRotationCoroutine(Vector3 endPosition, float rotationDuration) {
        Quaternion startRotation = transform.rotation;
        endPosition.y = transform.position.y;
        Vector3 direction = endPosition - transform.position;
        Quaternion endRotation = Quaternion.LookRotation(direction, Vector3.up);

        if (Mathf.Approximately(Mathf.Abs(Quaternion.Dot(startRotation, endRotation)), 1.0f) == false) {
            float timeElapsed = 0;
            while (timeElapsed < rotationDuration) {
                timeElapsed += Time.deltaTime;
                float lerpstep = timeElapsed / rotationDuration;
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpstep);
                yield return null;
            }
            transform.rotation = endRotation;
        }
        StartCoroutine(MovementCoroutine(endPosition));
    }

    private IEnumerator attackingRotationCoroutine(Vector3 target, float rotationDuration) {
        Quaternion startRotation = transform.rotation;
        target.y = transform.position.y;
        Vector3 direction = target - transform.position;
        Quaternion endRotation = Quaternion.LookRotation(direction, Vector3.up);

        if (Mathf.Approximately(Mathf.Abs(Quaternion.Dot(startRotation, endRotation)), 1.0f) == false) {
            float timeElapsed = 0;
            while (timeElapsed < rotationDuration) {
                timeElapsed += Time.deltaTime;
                float lerpstep = timeElapsed / rotationDuration;
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpstep);
                yield return null;
            }
            transform.rotation = endRotation;
        }
    }

    private IEnumerator MovementCoroutine(Vector3 endPosition) {
        Vector3 startPosition = transform.position;
        endPosition.y = startPosition.y;
        float timeElapsed = 0;

        while (timeElapsed < movementDuration) {
            timeElapsed += Time.deltaTime;
            float lerpstep = timeElapsed / movementDuration;
            transform.position = Vector3.Lerp(startPosition, endPosition, lerpstep);
            yield return null;
        }
        transform.position = endPosition;

        if (pathPositions.Count > 0) {
            Debug.Log("Looking for my next position ...");
            StartCoroutine(movingRotationCoroutine(pathPositions.Dequeue(), rotationDuration));
        }
        else {
            Debug.Log("I have reached my end position òwó");
            MovementFinished?.Invoke(this);
        }
    }    
}
