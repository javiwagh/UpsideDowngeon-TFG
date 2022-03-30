using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{
    [SerializeField]
    private int movementPoints = 2;
    public int MovementPoints {get => movementPoints;}

    [SerializeField]
    private float movementDuration = 0.5f, rotationDuration = 0.1f;

    private GlowHighlight glowHighlight;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();
    public event System.Action<Unit> MovementFinished;

    private void Awake() {
        glowHighlight = GetComponent<GlowHighlight>();
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
        StartCoroutine(RotationCoroutine(firstTarget, rotationDuration));
    }

    private IEnumerator RotationCoroutine(Vector3 endPosition, float rotationDuration) {
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
            StartCoroutine(RotationCoroutine(pathPositions.Dequeue(), rotationDuration));
        }
        else {
            Debug.Log("I have reached my end position òwó");
            MovementFinished?.Invoke(this);
        }
    }    
}
