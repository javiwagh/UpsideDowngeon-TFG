using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{
    private const int POISON_DURATION = 2;
    private const int PARALYSE_DURATION = 1;
    private int movementPoints;
    public int actionPoints;
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
    private int poisonCounter = 0;
    private bool paralysed = false;
    private int poisonTimer = 0;
    private int paralysedTimer = 0;

    private void Awake() {
        glowHighlight = GetComponent<GlowHighlight>();
        character = GetComponent<Character>();
        gameManager = FindObjectOfType<GameManager>();
        movementPoints = character.speed;
        actionPoints = 0;
    }

    internal void Deselect() {
        glowHighlight.ToggleGlow(false);
    }

    internal void Select() {
        glowHighlight.ToggleGlow(true);
    }

    public void moveThroughPath(List<Vector3> currentPath){
        if (!spendActionPoint()) return;
        pathPositions = new Queue<Vector3>(currentPath);
        Vector3 firstTarget = pathPositions.Dequeue();
        StartCoroutine(movingRotationCoroutine(firstTarget, rotationDuration));
    }

    public void Attack(Unit target, bool primaryAttack) {
        if (!spendActionPoint()) return;
        Debug.Log($"Attacking {target.GetComponent<Character>().characterName}!");
        StartCoroutine(attackingRotationCoroutine(target.transform.position, rotationDuration));
        if (this.character.unitType == UnitType.Monster) {
            switch (this.character.monsterType){
                case MonsterType.Goblin:
                    target.getStab(this.transform.rotation);
                break;
                case MonsterType.Troll:
                    target.recieveDamage(this.character.meleeDamage);
                break;
                case MonsterType.Spider:
                    target.getSting();
                break;
                case MonsterType.Rat:
                    target.getPoisoningBite();
                break;
            }
        }
        else target.recieveDamage(this.character.meleeDamage);
    }

    public void PickKey() {
        if (!spendActionPoint()) return;
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

    private bool spendActionPoint() {
        if (actionPoints > 0) actionPoints -= 1;
        else return false;
        character.toolTip.updateActionPoints(actionPoints);
        return true;
    }

    public void restoreActionPoints() {
        actionPoints = 2;
        character.toolTip.updateActionPoints(actionPoints);
    }

    public void Exhaust() {
        actionPoints = 0;
        character.toolTip.updateActionPoints(actionPoints);
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

    public void getStab(Quaternion attackerRotation){
        int damage = 2;
        float dotRotation = Quaternion.Dot(attackerRotation, this.transform.rotation);
        if (dotRotation > 0.6f || dotRotation < -0.6f) damage = damage * 2;
        
        recieveDamage(damage);
    }

    public void getSting(){
        //A paralysed adventurer will not to move a single tile the next turn
        paralysed = true;
        paralysedTimer = 1;
        int damage = 1;
        recieveDamage(damage);
    }

    public void getPoisoningBite(){
        //A poisoned adventurer will receive low damage the next two turn shifts
        poisonCounter += 1;
        poisonTimer = 2;
        int damage = 1;     
        recieveDamage(damage);
    }

    public void TurnShiftUnitActions() {
        recieveDamage(poisonCounter);
        poisonTimer -= 1;
        if (poisonTimer == 0) {
            poisonCounter -= 1;
            if (poisonCounter > 0) poisonTimer = POISON_DURATION;
        }
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
