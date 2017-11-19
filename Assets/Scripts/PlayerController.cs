using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerControl
{
	None,
	Flappy,
	FlappyOne,
	Balloon,
	Catapult,
	Tortoise,
	Kite,
}

public class PlayerController : MonoBehaviour
{
	[Header("Global settings")]
	public PlayerControl	control;
	public float			defaultWindForce = 1f;
	public float			maxXVelocity = 3f;

	[Space, Header("Falppy control settings")]
	public float			flappyUpForce = 50;
	public float			flappyTorque = 4;

	[Space, Header("Balloon control settings")]
	public float			balloonMaxHeight = 10;
	public float			balloonStep = 2;
	public GameObject		balloon;
	public GameObject		feather;
	public float			featherPower = 5;
	public bool				haveFeather = false;
	public float			maxBalloonVelocity = 60;
	public float			minBalloonVelocity = 40;

	[Space, Header("Kite control settings")]
	public GameObject		kite;
	public Transform		kiteAnchor;
	public float			kiteWirlRange = 3;
	public float			kitePower = 2;
	public float			kiteUpControlPower = 5;

	[Space, Header("Tortoise control settings")]
	public float			tortoiseSpeed = 10;
	public GameObject		tortoise;
	public bool				haveCarrotOnStick;
	
	[Space, Header("Catapult control settings")]
	public bool				haveLeaf = false;
	public float			baseCatapultPower = 30;
	public GameObject		catapult;
	public GameObject		catapultSpoon;
	public GameObject		squirrelBall;
	public GameObject		arrow;
	public Transform		doubleStickTop;
	public Transform		spoonEndPosition;

	float					balloonUpVelocity = 50f;

	bool					catapultThrowed = false;
	bool					parachuteTriggered = false;
	Vector2					catapultDirection = Vector2.right;
	
	Rigidbody2D				tortoiseRigidbody;
	
	Rigidbody2D				squirrelBallRigidbody;
	Vector3					defaultSpoonPosition;
	Vector3					defaultSquirrelbalPosition;
	float					catapultPower = 40;

	Rigidbody2D				rbody;
	SpriteRenderer			spriteRenderer;
	new Collider2D			collider;
	public bool				dead { get; private set; }

	Dictionary< PlayerControl, Action >	controlActions = new Dictionary< PlayerControl, Action >();
	Dictionary< PlayerControl, Action >	fixedControlActions = new Dictionary< PlayerControl, Action >();

	void Start ()
	{
		if (dead)
			return ;

		// SetControlFromEquipedItems();
		
		rbody = GetComponent< Rigidbody2D >();
		collider = GetComponent< Collider2D >();
		
		controlActions[PlayerControl.Flappy] = UpdateFlappy;
		controlActions[PlayerControl.Balloon] = UpdateBalloon;
		controlActions[PlayerControl.Catapult] = UpdateCatapult;
		controlActions[PlayerControl.Kite] = UpdateKite;
		controlActions[PlayerControl.Tortoise] = UpdateTortoise;
		controlActions[PlayerControl.None] = UpdateNone;
		controlActions[PlayerControl.FlappyOne] = UpdateFlappyOne;

		fixedControlActions[PlayerControl.Balloon] = FixedUpdateBalloon;

		balloon.SetActive(control == PlayerControl.Balloon);
		kite.SetActive(control == PlayerControl.Kite);
		tortoise.SetActive(control == PlayerControl.Tortoise);

		tortoiseRigidbody = tortoise.GetComponent< Rigidbody2D >();
		squirrelBallRigidbody = squirrelBall.GetComponent< Rigidbody2D >();
		spriteRenderer = GetComponent< SpriteRenderer >();

		defaultSpoonPosition = catapultSpoon.transform.position;
		defaultSquirrelbalPosition = squirrelBall.transform.position;

		if (control == PlayerControl.Flappy || control == PlayerControl.FlappyOne)
		{
			rbody.constraints = RigidbodyConstraints2D.None;
			collider.sharedMaterial.friction = 1;
		}

		if (control == PlayerControl.Kite)
		{
			rbody.gravityScale = 0;
			defaultWindForce = 0;
			rbody.drag = 2;
		}

		if (control == PlayerControl.Catapult)
		{
			catapult.SetActive(true);
			squirrelBallRigidbody.isKinematic = true;
			spriteRenderer.enabled = false;
			rbody.isKinematic = true;
		}
	}

	void SetControlFromEquipedItems()
	{
		switch (PlayerStorage.instance.travelType)
		{
			case TravelType.Balloon:
				control = PlayerControl.Balloon;
				break ;
			case TravelType.BalloonAndFeather1:
			case TravelType.BalloonAndFeather2:
				control = PlayerControl.Balloon;
				haveFeather = true;
				break ;
			case TravelType.BalloonAndLeaf:
				control = PlayerControl.Balloon;
				haveLeaf = true;
				break ;
			case TravelType.Feather1:
			case TravelType.Feather2:
				control = PlayerControl.FlappyOne;
				break ;
			case TravelType.Feathers:
				control = PlayerControl.Flappy;
				break ;
			case TravelType.KiteAndFriendAndString:
				control = PlayerControl.Kite;
				break ;
			case TravelType.SpoonAndElastic:
				control = PlayerControl.Catapult;
				break ;
			case TravelType.SpoonAndElasticAndLeaf:
				control = PlayerControl.Catapult;
				haveLeaf = true;
				break ;
			case TravelType.Tortoise:
				control = PlayerControl.Tortoise;
				defaultWindForce = 0;
				break ;
			case TravelType.TortoiseAndCarrotAndString:
				control = PlayerControl.Tortoise;
				defaultWindForce = 0;
				haveCarrotOnStick = true;
				break ;
			default:
				control = PlayerControl.None;
				break ;
		}
	}
	
	void Update ()
	{
		if (dead)
			return ;
		controlActions[control]();
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		switch (other.collider.tag)
		{
			case "Water":
				Death();
				break ;
			case "Obstacle":
				Death();
				break ;
		}
	}

	void Death()
	{
		Debug.Log("you're dead");
		dead = true;
	}

	void FixedUpdate()
	{
		if (fixedControlActions.ContainsKey(control))
			fixedControlActions[control]();

		//add wind force
		rbody.AddForce(Vector2.right * defaultWindForce);

		rbody.velocity = new Vector2(Mathf.Clamp(rbody.velocity.x, -maxXVelocity, maxXVelocity), rbody.velocity.y);
	}

	float horizontalKeyDown { get { if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.D)) return 1; if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A)) return -1; return 0; } }
	float verticalKeyDown { get { if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) return 1; if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) return -1; return 0; } }

	void UpdateNone()
	{
		
	}

	void UpdateFlappy()
	{
		float v = horizontalKeyDown;
		if (v != 0)
		{
			rbody.velocity = new Vector2(Mathf.Abs(rbody.velocity.x) * -v, 0);
			Debug.DrawLine(transform.position, transform.position + transform.up, Color.blue, 1);
			rbody.AddForce(transform.up * flappyUpForce, ForceMode2D.Impulse);
			rbody.AddTorque(v * flappyTorque, ForceMode2D.Impulse);
		}
	}

	void UpdateFlappyOne()
	{
		float v = horizontalKeyDown;
		if (v < 0)
		{
			rbody.velocity = new Vector2(Mathf.Abs(rbody.velocity.x) * -v, 0);
			Debug.DrawLine(transform.position, transform.position + transform.up, Color.blue, 1);
			rbody.AddForce(transform.up * flappyUpForce, ForceMode2D.Impulse);
			rbody.AddTorque(v * flappyTorque, ForceMode2D.Impulse);
		}
	}

	void UpdateBalloon()
	{
		float v = verticalKeyDown;
		if (v != 0)
		{
			if (v > 0 && balloonUpVelocity > maxBalloonVelocity)
				return ;
			if (v < 0 && balloonUpVelocity < minBalloonVelocity)
				return ;
			balloonUpVelocity += balloonStep * v;
		}

		float s = 1 + (balloonUpVelocity + (Physics.gravity.y * rbody.gravityScale)) / 10;
		balloon.transform.localScale = Vector3.one * s;
		
		if (haveFeather)
		{
			v = horizontalKeyDown;
			if (v != 0)
				rbody.AddForce(Vector2.right * -v * featherPower, ForceMode2D.Impulse);
		}
	}

	void UpdateCatapult()
	{
		float v;

		if (!catapultThrowed)
		{
			//calcul spoon position and so catapult power+
			Vector3 decal = new Vector3(1, .5f) * (Mathf.Sin(Time.timeSinceLevelLoad) - 1.5f);
			catapultSpoon.transform.position = defaultSpoonPosition + decal;
			squirrelBall.transform.position = defaultSquirrelbalPosition + decal;

			catapultDirection = doubleStickTop.position - squirrelBall.transform.position;

			if ((v = verticalKeyDown) != 0)
			{
				decal = Vector3.up * v * 0.2f;
				defaultSpoonPosition += decal;
				defaultSquirrelbalPosition += decal;
			}

			//Update arrow visu:
			float rot_z = Mathf.Atan2(catapultDirection.y, catapultDirection.x) * Mathf.Rad2Deg;
        	arrow.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
			var scale = arrow.transform.localScale;
			catapultPower = baseCatapultPower * catapultDirection.magnitude / 4;
			scale.y = catapultDirection.magnitude / 5;
			arrow.transform.localScale = scale;
			
			if (Input.GetKeyDown(KeyCode.Return))
			{
				catapultThrowed = true;
				StartCoroutine(MoveSpoonToDoubleStick());
			}
		}
		if (catapultThrowed && haveLeaf)
		{
			if (Input.GetKeyDown(KeyCode.Space))
				parachuteTriggered = true;
		}
		if (parachuteTriggered)
		{
			//decrease velocity and give a bit of left/right control
			
			squirrelBallRigidbody.drag = 100;
			
			if ((v = horizontalKeyDown) != 0)
				squirrelBallRigidbody.AddForce(Vector2.right * v * 10, ForceMode2D.Impulse);
		}
		transform.position = squirrelBall.transform.position;
	}

	IEnumerator MoveSpoonToDoubleStick()
	{
		while ((catapultSpoon.transform.position - spoonEndPosition.position).magnitude > .1f)
		{
			Vector3 tmp = catapultSpoon.transform.position;
			catapultSpoon.transform.position = Vector3.MoveTowards(catapultSpoon.transform.position, spoonEndPosition.position, .3f);
			squirrelBall.transform.position += catapultSpoon.transform.position - tmp;
			yield return null;
		}
		squirrelBallRigidbody.isKinematic = false;
		squirrelBallRigidbody.AddForce(catapultDirection * catapultPower, ForceMode2D.Impulse);
	}

	void UpdateTortoise()
	{
		Vector3 force;
		
		if (haveCarrotOnStick)
			force = Vector3.right * tortoiseSpeed * Time.deltaTime;
		else
			force = Vector3.zero;
		
		tortoise.transform.position += force;
		transform.position += force;

		float v;
		if ((v = Input.GetAxis("Horizontal")) != 0)
		{
			collider.sharedMaterial.friction = 0;
			rbody.velocity = new Vector2(v * 10, rbody.velocity.y);
		}
		else
			collider.sharedMaterial.friction = 1;
	}

	void UpdateKite()
	{
		Vector2 startDirection = (kiteAnchor.position - transform.position).normalized;
		float v;
		
		if ((v = Input.GetAxisRaw("Horizontal")) != 0)
			rbody.AddForce(-startDirection * kitePower * v, ForceMode2D.Impulse);

		v = verticalKeyDown;
		if (v != 0)
			rbody.AddForce(Vector2.up * kiteUpControlPower * v, ForceMode2D.Impulse);

		rbody.AddForce(Vector2.up * Mathf.Sin(Time.timeSinceLevelLoad / 1) * kiteWirlRange, ForceMode2D.Force);
		Debug.DrawRay(transform.position, startDirection, Color.red);
	}

	void FixedUpdateBalloon()
	{
		rbody.AddForce(Vector2.up * balloonUpVelocity, ForceMode2D.Force);
	}
}
