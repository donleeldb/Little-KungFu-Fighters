using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class fighterScript : MonoBehaviour {

	[Space(20)]
	[Header("Fighter settings")]

	[Tooltip("health points at the start")] public float health;
	[Tooltip("movement speed")] public float speed;
	[Tooltip("hit damage")] public float damage;
	[Tooltip("direction, where beated character flyes")] public Vector2 hitDirection;
	[Tooltip("if fighter can use guns")] public bool canShoot;
	[Tooltip("jump power")] public float jumpPower;
	[Tooltip("time before walk animation changes to run animation")] public float timeBeforeRun;
	[Tooltip("number of hit animations in animator controller")] public int numberOfHits;
	[Tooltip("number of block animations in animator controller")]public int numberOfBlocks;
	[Tooltip("how long fighter is demobilized because of hitting")]public float hitTime;
	[Tooltip("how long fighter is demobilized because of blocking")]public float blockTime;
	[Tooltip("how long fighter is demobilized because of getting super punch")]public float flyTime;
	[Tooltip("how far can fighter check for usable objects")]public float usableObjectsRadius;
	[Tooltip("layer of usable objects")]public LayerMask usableObjectsLayers;
	[Tooltip("material, that would be applied for a hit time period in case of getting attacked")]public Material hittedMaterial;
	[Tooltip("where bullets appear in case of shooting")]public Vector2 bulletOffset;
	[Tooltip("how much score would be gained in case of killing this fighter (can be negative)")]public float scoreForKilling;
	[Tooltip("where enemy would stand in case of fighting")]public Vector2 enemyOffset;
	[Tooltip("one shot audio clips")] public AudioClip[] sounds; // 0 - punch, 1 - super punch, 2 - shoot

	[Space(10)]
	[Header("AI settings")]

	[Tooltip("minimum time before fighter attacks")] public float minTimeBeforeAttack;
	[Tooltip("maximum time before fighter attacks")] public float maxTimeBeforeAttack;
	[Tooltip("chance of blocking attack")][Range(0,100)] public float chanceOfBlock;
	[Tooltip("chance of making super punch")][Range(0,100)] public float chanceOfSuperPunch;
	[Tooltip("layer of a enemy fighters")]public LayerMask enemyLayer;
	[Tooltip("layer of a friendly fighters")]public LayerMask friendLayer;
	[Tooltip("how far can fighter see his enemy")]public float checkForEnemyRadius;
	[Tooltip("how far can fighter see his friends")]public float checkForFriendRadius;
	[Tooltip("maximum distance for a fighter to shoot")]public float shootDistance;
	[Tooltip("distance, that punch hits")] public Vector2 hitRaycastOffset;
	[Tooltip("height of a vertical punch raycast")]public float hitRaycastLenght;
	[Tooltip("distance, that is close enought to hit enemy")]public float minDistanceBetweenFighters;
	[Tooltip("distance between following friends")]public float minDistanceBetweenFriends;
	[Tooltip("position, where fighter will go and than go back")]public Vector3 patrolPosition;
	[Tooltip("how long will fighter stay at patrol position")]public float waitAtPosition;


	[HideInInspector] public bool underControl; // shows if player is controling this fighter

	GameObject closestEnemy; // stores closest enemy
	GameObject closestFriend; // stores closest friend
	GameObject target; // enemy to attack
	GameObject friend; // friend to follow
	Vector3 startPosition; // first patrol position
	Vector3 targetPosition; // enemy position
	Vector3 friendPosition; // friend position

	bool protectingPosition; // shows if fighter is protecting position
	Vector3 center; // center of a physical body
	Material defaultMaterial; // default material
	bool grounded; // shows if fighter is standing of ground
	bool demobilized; // shows if fighter is demobilized
	bool isBlocking; // shows if fighter is blocking
	Rigidbody2D rb; // fighter's rigidbody
	Animator an; // animator
	SpriteRenderer sr; // sprite renderer
	AudioSource au;
	int k; // factor of a sight
	float floorLevel; // y coordinate of a figter
	bool nearEnemy; // shows if fighter is near enemy
	bool invoked; // shows if run animation is invoked
	bool dead; // shows if fighter is dead

	List<GameObject> ammunition; // list of an ammo, that fighter has
	int fightState; // fight state for an animator
	float shootPower; // forces, which applies to a bullet in case of shooting

	GameObject healthBar; // stores UI element, that shows health
	GamePlayManager gpm; // stores game play manager script

	[HideInInspector] public bool isMainCharacter;

	// Use this for initialization
	void Start () {

	}

	void Awake () 
	{
		prepareCharacter ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		checkForLanding (); // checks for landing
		changeSortingLayer(); // changes sorting layer to look natural with other objects
		if (!underControl && !demobilized && !dead) { // checks if fighter is alive and not under player's control
			checkForEnemy (); // looks for enemy
			if (target != null) // checks if enemy is around
			{
				goToEnemy(); // goes to enemy
			} else if (gameObject.layer == 8 && !underControl) // checks if it is ally
			{
				checkForFriend (); // looks for friend
				if (friend != null) // checks if friend is around
				{
					goToFriend (); // goes to friend
				}
			} else // checks if it not ally
			{
				patrol (); // patrols from start position to patrol position
			}
		}
	}

	void prepareCharacter () // determinies variables
	{
		rb = GetComponent<Rigidbody2D> ();
		an = GetComponent<Animator>();
		an.SetInteger ("fightState", -1);
		au = GetComponent<AudioSource> ();
		k = 1;
		rb.gravityScale = 0;
		startPosition = transform.position;
		patrolPosition += startPosition;
		floorLevel = - 10000;
		sr = GetComponent<SpriteRenderer> ();
		grounded = true;
		defaultMaterial = sr.material;
		sr.sortingOrder = - Mathf.FloorToInt (transform.position.y);
		center = GetComponent<BoxCollider2D> ().offset;
		ammunition = new List<GameObject>();
		gpm = GameObject.FindGameObjectWithTag ("gamePlayManager").GetComponent<GamePlayManager>();
	}

	public void move (float x, float y) // move function
	{
		if (!demobilized && !dead && !isBlocking) { // checks if fighter is alive, not demobilized and not blocking
			rb.velocity = new Vector2 (x, rb.velocity.y / speed) * speed; // applies horizontal movement
			if (grounded) { // checks if fighter is grounded
				
				rb.velocity = new Vector2 (rb.velocity.x / speed, y) * speed; // applies vertical movement
				if (rb.velocity == Vector2.zero) { // checks if fighter is standing
					an.SetInteger ("movingState", 0); // sets idle animation
					CancelInvoke ("run"); // disables invoking run animation
					invoked = false;
				} else { // if fighter is moving
					if (!invoked) { // checks if invoke is not called yet
						an.SetInteger ("movingState", 1);
						Invoke ("run", timeBeforeRun); // invokes run animation
						invoked = true;
					}
				}
			} 
				if (x < 0) { // checks if moving left
					k = -1; // changes line of sight to left
					transform.rotation = Quaternion.Euler (transform.rotation.x, -180, transform.rotation.z); // rotates gameObject to left
				} else if (x > 0) { // checks if moving right
					k = 1; // changes line of sight to right
					transform.rotation = Quaternion.Euler (transform.rotation.x, 0, transform.rotation.z); // rotates gameObject to right
				}
		}
	}

	void run() // starts run animation
	{
		changeAnimatorState ("movingState", 2);
	}

	public void idle() 
	{
		changeAnimatorState ("movingState", 0); // idle animation
		changeAnimatorState ("fightState", 0); // no fight animation
		changeAnimatorState("blockState", 0); // no block animation
		changeAnimatorState("getAttackedState", 0); // no get attacked animation
		invoked = false;
		grounded = true;
		if (dead) 
		{
			an.SetBool("dead", true); // starts lying animation
			demobilized = true;
		}
	}

	void changeAnimatorState(string variable,int i) // changes animator states
	{
		an.SetInteger (variable, i);
	}

	void checkForEnemy() // checks for enemy around
	{
		Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position + center, checkForEnemyRadius, enemyLayer); // creates array of enemies around
		if (enemies.Length > 0) { // if there are enemies
			closestEnemy = enemies [0].gameObject; // defines closest enemy
			for (int i = 0; i < enemies.Length; i++) { // check other enemies if some of them are closer
				if (Vector3.Distance (enemies [i].gameObject.transform.position, transform.position) < Vector3.Distance (closestEnemy.transform.position, transform.position)) {
					closestEnemy = enemies [i].gameObject; // defines new closest enemy
				}
			}
			target = closestEnemy.gameObject; // definies target to attack
			if (Vector3.Distance(closestEnemy.transform.position + new Vector3(closestEnemy.GetComponent<fighterScript> ().enemyOffset.x * 1, closestEnemy.GetComponent<fighterScript> ().enemyOffset.y, 0), transform.position) > Vector3.Distance(closestEnemy.transform.position + new Vector3(closestEnemy.GetComponent<fighterScript> ().enemyOffset.x * -1, closestEnemy.GetComponent<fighterScript> ().enemyOffset.y, 0), transform.position)) // defines which enemy's side to go
			{
				targetPosition = closestEnemy.transform.position + new Vector3(closestEnemy.GetComponent<fighterScript> ().enemyOffset.x * -1, closestEnemy.GetComponent<fighterScript> ().enemyOffset.y, 0);
			} else 
			{
				targetPosition = closestEnemy.transform.position + new Vector3(closestEnemy.GetComponent<fighterScript> ().enemyOffset.x * 1, closestEnemy.GetComponent<fighterScript> ().enemyOffset.y, 0);
			}
		} else // if there are no enemies
		{
			target = null;
		}
	}

	void goToEnemy () // go to enemy
	{
		Vector3 direction = targetPosition - transform.position; // defines direction to go
		if (Vector3.Distance (targetPosition, transform.position) > minDistanceBetweenFighters) { // checks if fighter is near enemy already
			if (ammunition.Count > 0) { // checks if fighter has weapon
				if (!nearEnemy && Mathf.Abs(targetPosition.y - transform.position.y) < 0.5f && Mathf.Abs(targetPosition.x - transform.position.x) <= shootDistance) { // checks if 
					nearEnemy = true;
					if (targetPosition.x > transform.position.x) { // checks if enemy is right from fighter
						transform.rotation = Quaternion.Euler (transform.rotation.x, 0, transform.rotation.z);
						k = 1;
					} else { // checks if enemy is left from fighter
						transform.rotation = Quaternion.Euler (transform.rotation.x, -180, transform.rotation.z);
						k = -1;
					}
					shoot (); // shoot
				} else { // checks if enemy far
					move (0, direction.normalized.y.CompareTo(0)); // move vertically
					nearEnemy = false;
				}
			} else {
				move (direction.normalized.x, direction.normalized.y); // move to enemy
				nearEnemy = false; 
				CancelInvoke ("attackEnemy");
			}
		} else if (!nearEnemy) { // checks if enemy is near
			nearEnemy = true;
			stop (); // stops fighter
			idle (); // sets idle animation
			if (closestEnemy.transform.position.x > transform.position.x) { // checks if closest enemy is right
				k = 1;
				transform.rotation = Quaternion.Euler (transform.rotation.x, 0, transform.rotation.z);
			} else { // checks if closest enemy is left
				k = -1;
				transform.rotation = Quaternion.Euler (transform.rotation.x, -180, transform.rotation.z);
			}
			Invoke ("attackEnemy", Random.Range (minTimeBeforeAttack,maxTimeBeforeAttack)); // Invokes attack
		}
	}

	void checkForFriend () // checks if friends are around
	{
		Collider2D[] friends = Physics2D.OverlapCircleAll(transform.position + center, checkForFriendRadius, friendLayer); // creates array of friend fighters around
		if (friends.Length > 1) // checks if there are friends around
		{
			for (int i = 0; i < friends.Length; i++) // looks for closest friend
			{
				if (friends[i].gameObject != gameObject) // checks if it is not fighter himself
				{
					closestFriend = friends [i].gameObject; // stores closest friend
					break;
				}
			}
			for (int i = 0; i < friends.Length; i++) // checks if there is a friend closer
			{
				if (Vector3.Distance (friends [i].gameObject.transform.position, transform.position) < Vector3.Distance (closestFriend.transform.position, transform.position) && friends [i].gameObject != gameObject) { // checks if other friend is closer
					closestFriend = friends [i].gameObject; // stores closest friend
				}
			}
			friend = closestFriend.gameObject; // determines target to follow
			friendPosition = closestFriend.transform.position; // determines position to go
		} else // if there are no friends
		{
			friend = null; 
			idle ();
		}
	}

	void goToFriend () // go to friend
	{
		Vector3 direction = friendPosition - transform.position; // determines direction to go
		if (Vector3.Distance (friendPosition, transform.position) > minDistanceBetweenFriends) { // checks if friend is far
			move (direction.normalized.x, direction.normalized.y); // moves to friend
		} else
		{
			stop (); // stops fighter
			idle (); // sets idle animation
		}
	}

	void patrol () // patrol from A position to B
	{
		if (!protectingPosition) { // checks if did not change target position yet
			Vector3 direction = patrolPosition - transform.position; // sets direction
			if (Vector3.Distance (patrolPosition, transform.position) > 2) { // checks if fighter is not on target position yet
				move (direction.normalized.x, direction.normalized.y); // moves to target position
			} else { // if on target position
				StartCoroutine ("protectPosition"); // waits at position 
			}
		}
	}

	IEnumerator protectPosition () // waits at position
	{
		stop (); // stops
		idle ();
		protectingPosition = true;
		CancelInvoke ("run");
		Vector3 start;
		start = patrolPosition;
		patrolPosition = startPosition;
		startPosition = start;
		yield return new WaitForSeconds (waitAtPosition + Random.Range (-0.5f, 0.5f)); // waits
		protectingPosition = false; // go
	}

	void attackEnemy () 
	{
		fighterScript fighter = closestEnemy.GetComponent<fighterScript>();
		if (!fighter.dead && !fighter.demobilized) { // checks if fighter is alive and not demobilized
			attack ();
		}
		Invoke ("attackEnemy", Random.Range (minTimeBeforeAttack,maxTimeBeforeAttack)); // repeat attack after time
	}

	public void jump ()  // jump
	{
		if (grounded && !demobilized && !isBlocking && !dead) { // checks if fighter is alive and not demobilized
			grounded = false;
			rb.gravityScale = 10; // adds gravity to make fighter fall
			rb.velocity = new Vector2 (rb.velocity.x, jumpPower); // applies force
			floorLevel = transform.position.y - 1;
			CancelInvoke ("run");
			invoked = false;
			GetComponent<BoxCollider2D> ().enabled = false; // disables collisions
			an.SetInteger ("movingState", 3);
		}
	}

	public void checkForUsableObjects () // use all usable objects around
	{
		Collider2D[] usableObjects = Physics2D.OverlapCircleAll(transform.position + center, usableObjectsRadius, usableObjectsLayers); // creates array of usable objects
		if (usableObjects.Length > 0) // if usable objects > 0 
		{
			usableObjects[0].gameObject.SendMessage("disableArrow", SendMessageOptions.DontRequireReceiver); // disable arrow
			use (usableObjects[0].gameObject); // use usable object
		}
	}

	void checkForLanding () // checks for landing
	{
		if (transform.position.y <= floorLevel) // checks if fighter hits the floot
		{
			land (); // land
		}
	}

	public void land () // hit the ground
	{
		rb.gravityScale = 0; // disables gravity
		grounded = true;
		rb.velocity = new Vector2 (rb.velocity.x, 0);
		floorLevel = - 10000;
		GetComponent<BoxCollider2D> ().enabled = true;
		if (demobilized) { // checks if demobilized
			changeAnimatorState ("getAttackedState", 3); // don't get up
		} else {
			idle (); // get up
		}
	}

	public void attack () // attack
	{
		if (grounded && !demobilized && !isBlocking && !dead) { // checks if fighter is alive, on ground, not demobilized and not blocking
			rb.velocity = Vector2.zero; // stops fighter
			CancelInvoke ("run"); 
			if (ammunition.Count == 0) { // if fighter has not weapon
				hit (); // punch
			} else { // if fighter has weapon
				shoot (); // shoot
			}
		}
	}

	void hit () // punch
	{
		an.SetInteger ("movingState", 4); // sets the transition animation state
		int randomHit = Random.Range (0, numberOfHits); // selects random hit animation
		an.SetInteger ("fightState", randomHit); // enables proper hit animation

		RaycastHit2D[] enemies = Physics2D.RaycastAll(transform.position + center + new Vector3 (hitRaycastOffset.x * k, hitRaycastOffset.y, 0), Vector2.up, hitRaycastLenght, enemyLayer); // checks for a receiver of a punch
		if (enemies.Length > 0) // if there are receivers of a punch
		{
			for (int i = 0; i < enemies.Length; i++) 
			{
				bool superPunch = chanceOfSuperPunch > Random.Range(0, 100) ? true:false; // randomly decides if make super punch
				enemies[i].transform.gameObject.GetComponent<fighterScript>().getAttacked(true, damage, superPunch, k, hitDirection); // sends attack information to a receiver
				if (superPunch) 
				{
					au.PlayOneShot (sounds[1]);
				} else 
				{
					au.PlayOneShot (sounds[0]);
				}
			}
		}

		StartCoroutine ("getDemobilized", hitTime); // demobilizes for a time of a punch
	}

	void shoot() // shoot
	{
			an.SetInteger ("fightState", fightState); // sets shoot animation
			ammunition [0].transform.position = transform.position + new Vector3 (bulletOffset.x * k, bulletOffset.y, 0); // puts ammo to fighter's position
			if (k == 1) { // if fighter shoots right
				ammunition [0].transform.rotation = Quaternion.Euler (transform.rotation.x, 0, transform.rotation.z); // rotates ammo right
			} else { // if fighter shoots left
			ammunition [0].transform.rotation = Quaternion.Euler (transform.rotation.x, -180, transform.rotation.z); // rotates ammo left
			}
			ammunition [0].SetActive (true); // enables ammo
			ammunition [0].GetComponent<Rigidbody2D> ().velocity = new Vector2 (shootPower * k, 0); //applies force to a ammo
			ammunition [0].GetComponent<ammoBehaviour> ().k = k; // sends shoot direction to ammo script
			ammunition.RemoveAt (0); // removes ammo from array
			au.PlayOneShot (sounds[2]);
			StartCoroutine ("getDemobilized", hitTime); // demobilizes fighter for a time of shooting
	}

	public IEnumerator block() // block
	{
		if (grounded && !demobilized && !isBlocking) { // checks if fighter is on ground, not demobilized and not blocking
			rb.velocity = Vector2.zero;
			CancelInvoke ("run"); 
			changeAnimatorState("blockState", Random.Range (1, numberOfBlocks + 1)); // sets block animation
			isBlocking = true;
			yield return new WaitForSeconds (blockTime); // demobilizes for a time of blocking
			isBlocking = false;
			idle();
		}
	}

	public void getAttacked (bool cantBeBlocked, float damage, bool superPunch, int s, Vector2 hitPower) // get attacked 
	{
		if (grounded && !dead) // checks if fighter is on ground and alive
		{
			if (!underControl && Random.Range(0,101) > chanceOfBlock) // if fighter is not under control, randomly block
			{
				StartCoroutine ("block");
			}
			if (cantBeBlocked || !cantBeBlocked && !isBlocking)
			{
				getHitted (damage, superPunch, s, hitPower);
			}
		}
	}

	void getHitted (float damage, bool superPunch, int s, Vector2 hitPower) // get hitted 
	{
		rb.velocity = Vector2.zero; // stops fighter 
		CancelInvoke ("run");
		setHittedMaterial (); // sets red color
		if (!superPunch && health > damage) { // receive simple attack
			changeAnimatorState ("getAttackedState", 1); // set get attacked animation
			StartCoroutine ("getDemobilized", hitTime); // get demobilized
		} else // receive super punch
		{
			changeAnimatorState ("getAttackedState", 2); // set get attacked animation
			rb.velocity = new Vector2 (hitPower.x * s, hitPower.y); // throws body to a hit direction
			grounded = false;
			rb.gravityScale = 10;
			floorLevel = transform.position.y - 1;
		//	CancelInvoke ();
			invoked = false;
			GetComponent<BoxCollider2D> ().enabled = false;
			StartCoroutine ("getDemobilized", flyTime);
			Invoke ("stop", flyTime);
			if (s == 1)
			{
				transform.rotation = Quaternion.Euler (transform.rotation.x, -180, transform.rotation.z);
				k = 1;
			} else 
			{
				transform.rotation = Quaternion.Euler (transform.rotation.x, 0, transform.rotation.z);
				k = -1;
			}
		}
		applyHealth (-damage);
	}

	void stop () // stop
	{
		rb.velocity = new Vector2 (0, 0);
	}

	public void applyHealth (float plusHealth) // adds plusHealth to health (can be negative)
	{
		health += plusHealth;
		if (health <= 0) // if health is 0 or less
		{
			die(); // die
		}
		if (underControl) // if player controls this fighter
		{
			refreshHealthBar(); // refresh health in health bar UI element
		}
	}

	void refreshHealthBar() // refresh health in health bar UI element
	{
		healthBar.GetComponent<Text>().text = "" + health;
	}

	void die () // due
	{
		demobilized = true;
		an.SetBool ("dead", true);
		dead = true;
		if (gameObject.layer == 8 && gpm.pc.Contains(gameObject)) 
		{
			gpm.pc.Remove(gameObject);
			if (gpm.pc.Count == 0) // checks if it is the last playable fighter
			{
				gpm.fail ();
			} else 
			{
				gpm.changePlayer();
			}
		}
		gpm.applyDeath (gameObject);
		gameObject.layer = 12; // sets "dead" layer
	}

	void setHittedMaterial () // sets hitted material
	{
		sr.material = hittedMaterial; // sets hitted material
		Invoke ("removeHittedMaterial", hitTime - 0.05f); // invokes changing hitted material
	}

	void removeHittedMaterial () // remove hitted material
	{
		sr.material = defaultMaterial;
	}

	public void use(GameObject usableObject) // use usable object
	{
		if (usableObject.GetComponent<weaponBehaviour>() != null) // if it is a weapon
		{
			takeWeapon (usableObject.GetComponent<weaponBehaviour>()); // take this weapon
			usableObject.SetActive(false); // disables object 
		} else if (usableObject.GetComponent<boxBehaviour>() != null) //if it is box 
		{
			usableObject.GetComponent<boxBehaviour>().throwBox(k); // throw box
		} else if (usableObject.GetComponent<usableObjectScript>() != null) // if it is simple usable object
		{
			usableObject.GetComponent<usableObjectScript>().use(gameObject); // use this game object
		}
	}

	public void getUnderControl (GameObject hb) // select this fighter to control
	{
		underControl = true; 
		healthBar = hb; // determines health bar UI element
		refreshHealthBar (); // refreshes health bar
	}

	void takeWeapon (weaponBehaviour weapon) // take weapon
	{
		ammunition = weapon.ammunition; // take ammunition
		fightState = weapon.fightState; // stores fight animation number
		shootPower = weapon.shootPower; // stores shoot power
	}

	void changeSortingLayer () 
	{
		if (Mathf.Abs (rb.velocity.y) > 0 && grounded) // if fighter moves vertical
		{
			sr.sortingOrder = - Mathf.FloorToInt (transform.position.y); // change sorting layer
		}
	}

	IEnumerator getDemobilized (float time) // demobilize for a time
	{
		demobilized = true;
		yield return new WaitForSeconds(time);
		idle ();
		isBlocking = false;
		demobilized = false;
	}
}

