using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{
    private int movementPoints;
    public int MovementPoints {get => movementPoints;}

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private float movementDuration = 0.5f, rotationDuration = 0.1f;

    private GlowHighlight glowHighlight;
    public Character character;
    public HexagonTile onTile;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();
    public event System.Action<Unit> MovementFinished;
    public bool hasKey = false;
    public bool isMoving = false;

    private void Awake() {
        glowHighlight = GetComponent<GlowHighlight>();
        character = GetComponent<Character>();
        gameManager = FindObjectOfType<GameManager>();
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

    public void Attack(Unit target, bool primaryAttack) {
        Debug.Log($"Attacking {target.GetComponent<Character>().characterName}!");
        StartCoroutine(attackingRotationCoroutine(target.transform.position, rotationDuration));
        if (this.character.unitType == UnitType.Monster) {
            switch (this.character.monsterType){
                case MonsterType.Goblin:
                    target.getStab(this.character.meleeDamage, this.transform.rotation);
                break;
                case MonsterType.Troll:
                    target.recieveDamage(this.character.meleeDamage);
                break;
                case MonsterType.Spider:
                break;
                case MonsterType.Rat:
                break;
            }
        }
        else target.recieveDamage(this.character.meleeDamage);
    }

    public void PickKey() {
        hasKey = true;
        gameManager.KeyPicked();
        Debug.Log("YAY! Got the key!");
    }

    public void DropKey() {
        if (hasKey) {
            hasKey = false;
            gameManager.keyDropped(this.onTile);
        }
    }

    public void recieveDamage(int damage){
        this.character.healthPoints -= damage;
        if (this.character.healthPoints <= 0) {
            this.character.healthPoints = 0;
            onTile.resetTileType();
            DropKey();
            this.gameObject.SetActive(false);
        }
        else character.toolTip.updateHealth(character.healthPoints);
    }

    public void getStab(int damage, Quaternion attackerRotation){
        float dotRotation = Quaternion.Dot(attackerRotation, this.transform.rotation);
        if (dotRotation > 0.6f || dotRotation < -0.6f) damage = damage * 2;
        
        recieveDamage(damage);
    }

    private IEnumerator movingRotationCoroutine(Vector3 endPosition, float rotationDuration) {
        this.isMoving = true;
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
            }
            transform.rotation = endRotation;
        }
        yield return null;
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
            if (this.hasKey && this.onTile.isEnd()) gameManager.AdventurersWin();
            this.isMoving = false;
        }
    }    
}
