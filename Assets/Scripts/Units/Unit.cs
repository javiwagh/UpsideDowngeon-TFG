using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[SelectionBase]
public class Unit : MonoBehaviour
{
    public Animator animator;

    private AudioSource audioSource;
    [SerializeField] public List<AudioClip> spawnSounds = new List<AudioClip>();
    [SerializeField] public List<AudioClip> selectionSounds = new List<AudioClip>();
    [SerializeField] public List<AudioClip> attackSounds = new List<AudioClip>();
    [SerializeField] public List<AudioClip> strikeSounds = new List<AudioClip>();
    [SerializeField] public List<AudioClip> damageSounds = new List<AudioClip>();
    [SerializeField] public AudioClip deathSound;
    [SerializeField] public AudioClip keySound;
    [SerializeField] public AudioClip knockSound;
    [SerializeField] public AudioSource secondaryAudioSource;
    [SerializeField] public AudioSource strikeSoundSource;
    
    private int soundIndex = 0;
    private bool playSound = true;
    [SerializeField]
    private float MOVEMENT_DURATION = 0.5f, ROTATION_DURATION = 0.1f;
    private const int POISON_DURATION = 2;
    private const int PARALYSE_DURATION = 2;
    private const int BASE_ADVENTURER_ACTION_POINTS = 2;
    private const int BASE_MONSTER_ACTION_POINTS = 2;
    private const int BASE_POPUP_MODE = 1;
    private const int CRITICAL_POPUP_MODE = 2;
    private const int POISON_POPUP_MODE = 3;
    private const int PARALYSE_POPUP_MODE = 4;
    private Unit lastPoisonAttacker;
    private int movementPoints;
    public int actionPoints;
    public int MovementPoints {get => movementPoints;}

    [SerializeField]
    public GameManager gameManager;
    private GlowHighlight glowHighlight;
    public Character character;
    public HexagonTile onTile;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();
    public event System.Action<Unit> MovementFinished;
    public bool hasKey = false;
    public bool isMoving = false;
    public int poisonCounter = 0;
    public bool paralysed = false;
    public int poisonTimer = 0;
    private int paralysedTimer = 0;
    public GameObject keyInstance;
    [SerializeField] 
    private Transform damagePopup;

    private void Awake() {
        glowHighlight = GetComponent<GlowHighlight>();
        character = GetComponent<Character>();
        gameManager = FindObjectOfType<GameManager>();
        movementPoints = character.speed;
        audioSource = this.GetComponent<AudioSource>();
        actionPoints = 0;
        selectionSounds = Shuffle(selectionSounds);
        attackSounds = Shuffle(attackSounds);
        strikeSounds = Shuffle(strikeSounds);
        damageSounds = Shuffle(damageSounds);

        if (this.character.side == Side.Monsters) {
            spawnSounds = Shuffle(spawnSounds);
            PlaySound(spawnSounds);
        }
    }

    internal void Deselect() {
        animator.SetBool("Selected", false);
        glowHighlight.ToggleGlow(false);
    }

    internal void Select() {
        if (this.character.side == Side.Monsters) PlaySound(selectionSounds);
        else {
            if (playSound) {
                playSound = false;
                PlaySound(selectionSounds);
            }
            else playSound = true;
        }
        animator.SetBool("Selected", true);
        glowHighlight.ToggleGlow(true);
    }

    internal void Highlight() {
        glowHighlight.ToggleGlow(true);
    }

    public void HighlightTarget() {
        glowHighlight.HighlightTarget();
    }

    public void ResetHighlight() {
        glowHighlight.ResetHighlight();
    }

    public void moveThroughPath(List<Vector3> currentPath){
        if (!spendActionPoint()) return;
        gameManager.Move();
        pathPositions = new Queue<Vector3>(currentPath);
        Vector3 firstTarget = pathPositions.Dequeue();
        StartCoroutine(movingRotationCoroutine(firstTarget, ROTATION_DURATION));
    }

    private void updateSoundIndex() {
        ++soundIndex;
        if (soundIndex > 3) soundIndex = 0;
    }

    private List<AudioClip> Shuffle(List<AudioClip> audioList) {
        for (int i = 0; i < audioList.Count; i++) {
            AudioClip temp = audioList[i];
            int randomIndex = Random.Range(i, audioList.Count);
            audioList[i] = audioList[randomIndex];
            audioList[randomIndex] = temp;
        }
        return audioList;
    }

    private void PlaySound(List<AudioClip> clips){
        updateSoundIndex();
        audioSource.clip = clips[soundIndex];
        audioSource.Play(0);
    }

    private void PlayDeathSound(){
        audioSource.clip = deathSound;
        audioSource.Play(0);
    }

    private void PlayStrikeSound(){
        updateSoundIndex();
        strikeSoundSource.clip = strikeSounds[soundIndex];
        strikeSoundSource.Play(0);
    }

    private void PlayKnock(){
        secondaryAudioSource.clip = knockSound;
        secondaryAudioSource.Play();
    }

    private void PlayKey(){
        secondaryAudioSource.clip = keySound;
        secondaryAudioSource.Play();
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
        PlayKey();
        hasKey = true;
        
        key.Pick();
        keyInstance.SetActive(true);
        Debug.Log("YAY! Got the key!");
        gameManager.KeyPicked();
        key.GetComponent<HexagonTile>().room.PickKey();
    }

    public void GiveKey() {
        PlayKey();
        hasKey = true;
        keyInstance.SetActive(true);
        Debug.Log("YAY! Got the key!");
        //gameManager.KeyPicked();
    }

    public bool spendActionPoint() {
        if (actionPoints > 0) actionPoints -= 1;
        else return false;
        character.toolTip.updateActionPoints(actionPoints);
        return true;
    }

    public void restoreActionPoints() {
        if (this.character.side == Side.Monsters) actionPoints = BASE_MONSTER_ACTION_POINTS;
        else actionPoints = BASE_ADVENTURER_ACTION_POINTS;
        character.toolTip.updateActionPoints(actionPoints);
    }

    public void Exhaust() {
        actionPoints = 0;
        character.toolTip.updateActionPoints(actionPoints);
    }

    public void recieveDamage(int damage, Unit attacker, int mode){
        
        this.character.healthPoints -= damage;
        popup("-" + damage, mode);
        
        if (this.character.healthPoints <= 0) {
            this.character.healthPoints = 0;
            character.toolTip.updateHealth(character.healthPoints);
            onTile.resetTileType();

            this.keyInstance.SetActive(false);
            if (hasKey) attacker.GiveKey();

            gameManager.RemoveUnit(this.gameObject);
            StartCoroutine(animateDeath());
        }
        else  {
            PlaySound(damageSounds);
            animator.SetTrigger("Strike");
            character.toolTip.updateHealth(character.healthPoints); 
        }
    }

    private IEnumerator animateDeath() {
        gameManager.followAdventurer(gameObject);
        gameManager.Move();

        this.HighlightTarget();
        animator.SetTrigger("Die");
        
        float waitingTime = 0f;
        if (this.character.side == Side.Monsters) {
            PlaySound(damageSounds);
            waitingTime = 2.5f;
        }
        else {
            PlayDeathSound();
            waitingTime = 3.5f;
        }
        yield return new WaitForSeconds(waitingTime);
        this.gameObject.SetActive(false);

        if (gameManager.monstersTurn) gameManager.freeCamera();
        
        gameManager.Wait();
    }

    private void popup(string text, int mode) {
        Vector3 position = keyInstance.transform.position;
        Transform damagePopupInstance = Instantiate(damagePopup, position, gameManager.player.transform.rotation);
        
        damagePopupInstance.GetComponent<DMGPopup>().setText(text, mode);
    }

    public void getStab(int damage, Quaternion attackerRotation, Unit attacker){
        int critical = BASE_POPUP_MODE;
        float dotRotation = Quaternion.Dot(attackerRotation, this.transform.rotation);
        if (dotRotation > 0.6f || dotRotation < -0.6f) {
            damage = damage * 2;
            critical = CRITICAL_POPUP_MODE;
        }
        
        
        recieveDamage(damage, attacker, critical);
    }

    private bool coinFlip() {
        if (Random.value < 0.5f) return true;
        return false;
    }
    public IEnumerator getSting(Unit attacker){
        //A paralysed adventurer will not to move a single tile the next turn
        int mode = BASE_POPUP_MODE;
        if (coinFlip()) {
            paralysed = true;
            paralysedTimer = PARALYSE_DURATION;
            mode = PARALYSE_POPUP_MODE;
        }

        int damage = 1;
        
        recieveDamage(damage, attacker, mode);
        if (mode == PARALYSE_POPUP_MODE) {
            yield return new WaitForSeconds(0.2f);
            popup("PARALYSED", mode);
        }
        yield return null;
    }

    public IEnumerator getPoisoningBite(Unit attacker){
        //A poisoned adventurer will receive low damage the next two turn shifts
        int mode = BASE_POPUP_MODE;

        if (coinFlip()) {
            poisonCounter += 1;
            poisonTimer = POISON_DURATION;
            lastPoisonAttacker = attacker;
            mode = POISON_POPUP_MODE;
        }
        
        int damage = 1;
        
     
        recieveDamage(damage, attacker, mode);

        if (mode == POISON_POPUP_MODE) {
            yield return new WaitForSeconds(0.2f); 
            popup("POISONED", mode);
        }
        yield return null;
    }

    public void TurnShiftUnitActions() {
        //POISON
        if (poisonCounter >= 1 && this.character.healthPoints > 1) {
            recieveDamage(1, lastPoisonAttacker, POISON_POPUP_MODE);
            poisonTimer -= 1;
            if (poisonTimer <= 0) {
                poisonCounter -= 1;
                if (poisonCounter > 0) poisonTimer = POISON_DURATION;
            }
        } 
        //PARALYSE
        if (paralysed) movementPoints = 0;
        else movementPoints = character.speed;
        paralysedTimer -= 1;
        if (paralysedTimer == 0) paralysed = false;

        character.toolTip.updateHealth(character.healthPoints);
    }

    private IEnumerator movingRotationCoroutine(Vector3 endPosition, float rotationDuration) {
        this.isMoving = true;
        Quaternion startRotation = transform.rotation;
        endPosition.y = transform.position.y;
        Vector3 direction = endPosition - transform.position;
        Quaternion endRotation = Quaternion.LookRotation(direction, Vector3.up);
        //PlayKnock();

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
        PlaySound(attackSounds);
        animator.SetTrigger("Attack");
    
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

        yield return new WaitForSeconds(0.5f);
        if (this.character.unitType == UnitType.Monster) {
            switch (this.character.monsterType){
                case MonsterType.Goblin:
                    target.getStab(this.character.meleeDamage, this.transform.rotation, this);
                break;
                case MonsterType.Troll:
                    target.recieveDamage(this.character.meleeDamage, this, BASE_POPUP_MODE);
                break;
                case MonsterType.Spider:
                    StartCoroutine(target.getSting(this));
                break;
                case MonsterType.Rat:
                    StartCoroutine(target.getPoisoningBite(this));
                break;
                default:
                    Debug.Log($"Monster type not supported: {this.character.monsterType}");
                break;
            }
        }
        else {
            switch (this.character.adventurerType){
                case AdventurerType.Warrior:
                    
                    target.recieveDamage(this.character.meleeDamage, this, BASE_POPUP_MODE);
                break;
                case AdventurerType.Rogue:
                    target.getStab(this.character.meleeDamage, this.transform.rotation, this);
                break;
                default:
                    target.getStab(this.character.meleeDamage, this.transform.rotation, this);
                break;
            }
        }
        PlayStrikeSound();
    }

    private IEnumerator MovementCoroutine(Vector3 endPosition) {
        Vector3 startPosition = transform.position;
        endPosition.y = startPosition.y;
        float timeElapsed = 0;
        PlayKnock();

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
            gameManager.Wait();
            //PlayKnock();
        }
    }    
}
