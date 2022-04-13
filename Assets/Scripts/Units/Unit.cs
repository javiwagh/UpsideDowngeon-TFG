using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{
    [SerializeField]
    private float MOVEMENT_DURATION = 0.5f, ROTATION_DURATION = 0.1f;
    private const int POISON_DURATION = 2;
    private const int PARALYSE_DURATION = 1;
    private Unit lastPoisonAttacker;
    private int movementPoints;
    public int actionPoints;
    public int MovementPoints {get => movementPoints;}

    [SerializeField]
    private GameManager gameManager;
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
    public GameObject keyInstance;

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
        StartCoroutine(movingRotationCoroutine(firstTarget, ROTATION_DURATION));
    }

    public bool isBard() {
        return (this.character.unitType == UnitType.Adventurer && this.character.adventurerType == AdventurerType.Bard);
    }

    public void Attack(Unit target, bool spend) {
        if (spend && !spendActionPoint()) return;
        Debug.Log($"Attacking {target.GetComponent<Character>().characterName}!");
        StartCoroutine(attackingCoroutine(target, ROTATION_DURATION));
    }

    public void PickKey(Key key, bool spend) {
        if (spend && !spendActionPoint()) return;
        StartCoroutine(Pick(key));
    }
    IEnumerator Pick(Key key) {
        while (isMoving) yield return null;
        hasKey = true;
        key.Pick();
        gameManager.KeyPicked();
        key.GetComponent<HexagonTile>().room.hasKeyTile = false;
        key.GetComponent<HexagonTile>().room.keyTile = null;
        keyInstance.SetActive(true);
        Debug.Log("YAY! Got the key!");
    }

    public void GiveKey() {
        hasKey = true;
        gameManager.KeyPicked();
        keyInstance.SetActive(true);
        Debug.Log("YAY! Got the key!");    
    }

    public bool spendActionPoint() {
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

    public void recieveDamage(int damage, Unit attacker){
        this.character.healthPoints -= damage;
        if (this.character.healthPoints <= 0) {
            this.character.healthPoints = 0;
            onTile.resetTileType();
            attacker.GiveKey();
            this.gameObject.SetActive(false);
        }
        else character.toolTip.updateHealth(character.healthPoints);
    }

    public void getStab(Quaternion attackerRotation, Unit attacker){
        int damage = 2;
        float dotRotation = Quaternion.Dot(attackerRotation, this.transform.rotation);
        if (dotRotation > 0.6f || dotRotation < -0.6f) damage = damage * 2;
        
        recieveDamage(damage, attacker);
    }

    public void getSting(Unit attacker){
        //A paralysed adventurer will not to move a single tile the next turn
        paralysed = true;
        paralysedTimer = PARALYSE_DURATION;
        int damage = 1;
        recieveDamage(damage, attacker);
    }

    public void getPoisoningBite(Unit attacker){
        //A poisoned adventurer will receive low damage the next two turn shifts
        poisonCounter += 1;
        poisonTimer = POISON_DURATION;
        int damage = 1;
        lastPoisonAttacker = attacker;  
        recieveDamage(damage, attacker);
    }

    public void TurnShiftUnitActions() {
        //POISON
        recieveDamage(poisonCounter, lastPoisonAttacker);
        poisonTimer -= 1;
        if (poisonTimer == 0) {
            poisonCounter -= 1;
            if (poisonCounter > 0) poisonTimer = POISON_DURATION;
        }
        //PARALYSE
        if (paralysed) movementPoints = 0;
        else movementPoints = character.speed;
        paralysedTimer -= 1;
        if (paralysedTimer == 0) paralysed = false;
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

    private IEnumerator attackingCoroutine(Unit target, float rotationDuration) {
        while(isMoving) yield return null;
        Quaternion startRotation = transform.rotation;
        Vector3 targetPosition = target.transform.position;
        targetPosition.y = transform.position.y;
        Vector3 direction = targetPosition - transform.position;
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

        if (this.character.unitType == UnitType.Monster) {
            switch (this.character.monsterType){
                case MonsterType.Goblin:
                    target.getStab(this.transform.rotation, this);
                break;
                case MonsterType.Troll:
                    target.recieveDamage(this.character.meleeDamage, this);
                break;
                case MonsterType.Spider:
                    target.getSting(this);
                break;
                case MonsterType.Rat:
                    target.getPoisoningBite(this);
                break;
                default:
                    Debug.Log($"Monster type not supported: {this.character.monsterType}");
                break;
            }
        }
        else {
            switch (this.character.adventurerType){
                case AdventurerType.Warrior:
                    target.recieveDamage(this.character.meleeDamage, this);
                break;
                case AdventurerType.Rogue:
                    target.getStab(this.transform.rotation, this);
                break;
                default:
                    Debug.Log($"Adventurer type not supported: {this.character.monsterType}");
                break;
            }
        }        
    }

    private IEnumerator MovementCoroutine(Vector3 endPosition) {
        Vector3 startPosition = transform.position;
        endPosition.y = startPosition.y;
        float timeElapsed = 0;

        while (timeElapsed < MOVEMENT_DURATION) {
            timeElapsed += Time.deltaTime;
            float lerpstep = timeElapsed / MOVEMENT_DURATION;
            transform.position = Vector3.Lerp(startPosition, endPosition, lerpstep);
            yield return null;
        }
        transform.position = endPosition;

        if (pathPositions.Count > 0) {
            StartCoroutine(movingRotationCoroutine(pathPositions.Dequeue(), MOVEMENT_DURATION));
        }
        else {
            MovementFinished?.Invoke(this);
            HexGrid hexGrid = FindObjectOfType<HexGrid>();
            hexGrid.getTileAt(hexGrid.GetClosestTile(this.transform.position)).stepOnTile(this);
            if (this.hasKey && this.onTile.isEnd()) gameManager.AdventurersWin();
            this.isMoving = false;
        }
    }    
}
