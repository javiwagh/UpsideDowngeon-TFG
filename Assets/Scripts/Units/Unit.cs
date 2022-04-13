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

    public bool isBard() {
        return (this.character.unitType == UnitType.Adventurer && this.character.adventurerType == AdventurerType.Bard);
    }

    public void Attack(Unit target, bool spend) {
        if (spend && !spendActionPoint()) return;
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
                default:
                    Debug.Log($"Monster type not supported: {this.character.monsterType}");
                break;
            }
        }
        else {
            switch (this.character.adventurerType){
                case AdventurerType.Warrior:
                    target.recieveDamage(this.character.meleeDamage);
                break;
                case AdventurerType.Rogue:
                    target.getStab(this.transform.rotation);
                break;
                default:
                    Debug.Log($"Adventurer type not supported: {this.character.monsterType}");
                break;
            }
        }        
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
        paralysedTimer = PARALYSE_DURATION;
        int damage = 1;
        recieveDamage(damage);
    }

    public void getPoisoningBite(){
        //A poisoned adventurer will receive low damage the next two turn shifts
        poisonCounter += 1;
        poisonTimer = POISON_DURATION;
        int damage = 1;     
        recieveDamage(damage);
    }

    public void TurnShiftUnitActions() {
        //POISON
        recieveDamage(poisonCounter);
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

    private IEnumerator attackingRotationCoroutine(Vector3 target, float rotationDuration) {
        while(isMoving) yield return null;
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
            StartCoroutine(movingRotationCoroutine(pathPositions.Dequeue(), rotationDuration));
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
