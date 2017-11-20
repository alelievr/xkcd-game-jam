using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

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

public enum DeathType
{
	Drawn,
	Crashed,
	CloudedBalloon,
	CloudedKite,
	CloudedFeather,
}

[System.Serializable]
public struct DeathTypeScreen
{
	public DeathType	type;
	public Sprite		sprite;
	public string		text;
}

public class PlayerController : MonoBehaviour
{
	[Header("Global settings")]
	public PlayerControl	control;
	public float			defaultWindForce = -1f;
	public float			maxXVelocity = 3f;

	[Space, Header("Falppy control settings")]
	public float			flappyUpForce = 50;
	public float			flappyTorque = 4;

	[Space, Header("Balloon control settings")]
	public float			balloonMaxHeight = 10;
	public float			balloonStep = 2;
	public GameObject		balloon;
	public float			featherPower = 5;
	public bool				haveFeather = false;
	public float			maxBalloonVelocity = 60;
	public float			minBalloonVelocity = 40;

	[Space, Header("Kite control settings")]
	public Transform		kiteAnchor;
	public float			kiteWirlRange = 3;
	public float			kitePower = 2;
	public float			kiteUpControlPower = 5;
	public LineRenderer		kiteLine;

	[Space, Header("Tortoise control settings")]
	public float			tortoiseSpeed = 10;
	public bool				haveCarrotOnStick;
	
	[Space, Header("Catapult control settings")]
	public bool				haveLeaf = false;
	public float			baseCatapultPower = 20;
	public GameObject		catapult;
	public GameObject		catapultSpoon;
	public GameObject		arrow;
	public Transform		doubleStickTop;
	public Transform		spoonEndPosition;
	public Transform		squirrelBallTransform;

	[Space, Header("Squirrel animations")]
	public GameObject		glidingSquirrelLeaf;
	public GameObject		glidingSuirrelKite;
	public GameObject		flappySquirrel;
	public GameObject		halfFlappySquirrel;
	public GameObject		balloonSquirrel;
	public GameObject		balloonSquirrelLeaf;
	public GameObject		balloonSquirrelFeather;
	public GameObject		tortoiseSquirrel;
	public GameObject		tortoiseSquirrelCarrotStick;
	public GameObject		squirrelBall;

	[Space, Header("Dead screens"), SerializeField]
	public List< DeathTypeScreen >	deadScreens;

	[Space, Header("Virtual camera")]
	public CinemachineVirtualCamera	virtualCamera;

	[Space, Header("Audio clips")]
	public AudioClip		waterSplash;
	public AudioClip		crash;

	float					balloonUpVelocity = 50f;

	float					defaultGravityScale;

	bool					catapultThrowed = false;
	bool					parachuteTriggered = false;
	Vector2					catapultDirection = Vector2.right;
	GameObject				instantiatedSquirrelBall;
	
	Rigidbody2D				tortoiseRigidbody;
	
	Vector3					defaultSpoonPosition;
	Vector3					defaultSquirrelbalPosition;
	AudioSource				audioSource;
	float					catapultPower = 40;

	Rigidbody2D				rbody;
	Animator				animator;
	public bool				dead { get; private set; }
	DeathType				deathType;

	Dictionary< PlayerControl, Action >	controlActions = new Dictionary< PlayerControl, Action >();
	Dictionary< PlayerControl, Action >	fixedControlActions = new Dictionary< PlayerControl, Action >();

	void Start ()
	{
		SetControlFromEquipedItems();
		
		rbody = GetComponent< Rigidbody2D >();

		audioSource = GetComponent< AudioSource >();
		
		controlActions[PlayerControl.Flappy] = UpdateFlappy;
		controlActions[PlayerControl.Balloon] = UpdateBalloon;
		controlActions[PlayerControl.Catapult] = UpdateCatapult;
		controlActions[PlayerControl.Kite] = UpdateKite;
		controlActions[PlayerControl.Tortoise] = UpdateTortoise;
		controlActions[PlayerControl.None] = UpdateNone;
		controlActions[PlayerControl.FlappyOne] = UpdateFlappyOne;

		fixedControlActions[PlayerControl.Balloon] = FixedUpdateBalloon;

		balloon.SetActive(control == PlayerControl.Balloon);
		kiteAnchor.gameObject.SetActive(control == PlayerControl.Kite);
		kiteLine.SetPosition(0, kiteAnchor.position);

		switch (control)
		{
			case PlayerControl.Flappy:
				rbody.constraints = RigidbodyConstraints2D.None;
				InstantiateSquirrel(flappySquirrel);
				break ;
			case PlayerControl.FlappyOne:
				rbody.constraints = RigidbodyConstraints2D.None;
				InstantiateSquirrel(halfFlappySquirrel);
				break ;
			case PlayerControl.Balloon:
				if (haveLeaf)
					InstantiateSquirrel(balloonSquirrelLeaf);
				else if (haveFeather)
					InstantiateSquirrel(balloonSquirrelFeather);
				else
					InstantiateSquirrel(balloonSquirrel);
				break ;
			case PlayerControl.Tortoise:
				defaultWindForce = 0;
				if (haveCarrotOnStick)
					InstantiateSquirrel(tortoiseSquirrelCarrotStick);
				else
					InstantiateSquirrel(tortoiseSquirrel);
				break ;
			case PlayerControl.Kite:
				rbody.gravityScale = 0;
				defaultWindForce = 0;
				rbody.drag = 2;
				InstantiateSquirrel(glidingSuirrelKite);
				break ;
			case PlayerControl.Catapult:
				catapult.SetActive(true);
				instantiatedSquirrelBall = InstantiateSquirrel(squirrelBall);
				rbody.isKinematic = true;
				break ;
		}

		defaultSpoonPosition = catapultSpoon.transform.position;
		defaultSquirrelbalPosition = squirrelBallTransform.position;

		defaultGravityScale = rbody.gravityScale;
	}

	GameObject InstantiateSquirrel(GameObject squirrelPrefab)
	{
		var visual = Instantiate(squirrelPrefab, transform) as GameObject;
		visual.transform.localPosition = Vector3.zero;
		visual.transform.localScale = Vector3.one * .2f;
		animator = visual.GetComponent< Animator >();
		return visual;
	}

	void SetControlFromEquipedItems()
	{
		Debug.Log("playerStorgae instance: " + PlayerStorage.instance);
		if (PlayerStorage.instance == null)
			return ;
		
		switch (PlayerStorage.instance.travelType)
		{
			case TravelType.Balloon:
				control = PlayerControl.Balloon;
				break ;
			case TravelType.BalloonAndStringAndFeather1:
			case TravelType.BalloonAndStringAndFeather2:
				control = PlayerControl.Balloon;
				haveFeather = true;
				break ;
			case TravelType.BalloonAndStringAndLeaf:
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

		Debug.Log("control type: " + control + ", travelType: " + PlayerStorage.instance.travelType);
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
			case "Obstacle":
				Death();
				break ;
			case "Ground":
				Debug.Log("hit ground with velocity: " + other.relativeVelocity.magnitude);
				if (other.relativeVelocity.magnitude > 8)
				{
					audioSource.PlayOneShot(crash);
					deathType = DeathType.Crashed;
					Death();
				}
				break ;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		switch (other.tag)
		{
			case "Water":
				if (control == PlayerControl.Tortoise)
				{
					rbody.velocity = Vector2.zero;
					rbody.gravityScale = 0;
					return ;
				}
				audioSource.PlayOneShot(waterSplash);
				deathType = DeathType.Drawn;
				rbody.drag = 50;
				rbody.gravityScale = defaultGravityScale;
				Death();
				break ;
			case "Cloud":
				if (control == PlayerControl.Catapult)
					return ;
				// audioSource.PlayOneShot();
				switch (control)
				{
					case PlayerControl.Balloon:
						deathType = DeathType.CloudedBalloon;
						break ;
					case PlayerControl.Flappy:
					case PlayerControl.FlappyOne:
						deathType = DeathType.CloudedFeather;
						break ;
					default:
						deathType = DeathType.CloudedKite;
						break ;
				}
				Death();
				break ;
		}
	}

	void Death()
	{
		Debug.Log("you're dead");
		dead = true;
		StartCoroutine(TitleScreenTimeout());
	}

	IEnumerator TitleScreenTimeout()
	{
		yield return new WaitForSeconds(1);

		var deathInfo = deadScreens.Find(d => d.type == deathType);

		SceneSwitcher.instance.ShowTitleScreen(deathInfo.sprite, deathInfo.text);
	}

	void FixedUpdate()
	{
		if (dead)
		{
			if (control == PlayerControl.Kite)
				kiteLine.SetPosition(1, transform.position);
			return ;
		}
		if (fixedControlActions.ContainsKey(control))
			fixedControlActions[control]();

		//add wind force
		if (defaultWindForce > Mathf.Epsilon)
			rbody.AddForce(Vector2.right * defaultWindForce);

		if (control != PlayerControl.Catapult)
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
			if (v > 0)
				animator.SetTrigger("flapRight");
			else
				animator.SetTrigger("flapLeft");
			rbody.velocity = new Vector2(Mathf.Abs(rbody.velocity.x) * -v, 0);
			Debug.DrawLine(transform.position, transform.position + transform.up, Color.blue, 1);
			rbody.AddForce(transform.up * flappyUpForce, ForceMode2D.Impulse);
			rbody.AddTorque(v * flappyTorque, ForceMode2D.Impulse);
		}
	}

	void UpdateFlappyOne()
	{
		float v = horizontalKeyDown;
		if (v > 0)
		{
			animator.SetTrigger("flapRight");
			rbody.velocity = new Vector2(Mathf.Abs(rbody.velocity.x) * v, 0);
			Debug.DrawLine(transform.position, transform.position + transform.up, Color.blue, 1);
			rbody.AddForce(transform.up * flappyUpForce, ForceMode2D.Impulse);
			rbody.AddTorque(-v * flappyTorque, ForceMode2D.Impulse);
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
			{
				animator.SetTrigger("flap");
				rbody.AddForce(Vector2.right * -v * featherPower, ForceMode2D.Impulse);
			}
		}

		Vector3 scale = transform.localScale;
		scale.x = Mathf.Abs(scale.x) * Mathf.Sign(rbody.velocity.x);
		transform.localScale = scale;
	}

	void UpdateCatapult()
	{
		float v;

		if (!catapultThrowed)
		{
			//calcul spoon position and so catapult power+
			Vector3 decal = new Vector3(1, .5f) * (Mathf.Sin(Time.timeSinceLevelLoad) - 1.5f);
			catapultSpoon.transform.position = defaultSpoonPosition + decal;
			transform.position = defaultSquirrelbalPosition + decal;

			catapultDirection = doubleStickTop.position - transform.position;

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
			catapultPower = baseCatapultPower * (.5f + catapultDirection.magnitude / 8);
			scale.y = catapultDirection.magnitude / 5;
			arrow.transform.localScale = scale;
			
			if (Input.GetKeyDown(KeyCode.Return))
			{
				catapultThrowed = true;
				virtualCamera.m_Lens.FieldOfView = 50;
				StartCoroutine(MoveSpoonToDoubleStick());
			}
		}
		if (catapultThrowed && haveLeaf)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				parachuteTriggered = true;
				Destroy(instantiatedSquirrelBall);
				InstantiateSquirrel(glidingSquirrelLeaf);
			}
		}
		Debug.DrawRay(transform.position, rbody.velocity, Color.green);
		if (parachuteTriggered)
		{
			//decrease velocity and give a bit of left/right control
			
			rbody.drag = 35;
			
			if ((v = Input.GetAxis("Horizontal")) != 0)
				rbody.AddForce(Vector2.right * v * 1, ForceMode2D.Impulse);
		}
	}

	IEnumerator MoveSpoonToDoubleStick()
	{
		while ((catapultSpoon.transform.position - spoonEndPosition.position).magnitude > .1f)
		{
			Vector3 tmp = catapultSpoon.transform.position;
			catapultSpoon.transform.position = Vector3.MoveTowards(catapultSpoon.transform.position, spoonEndPosition.position, .3f);
			transform.position += catapultSpoon.transform.position - tmp;
			yield return null;
		}
		rbody.isKinematic = false;
		Debug.DrawRay(transform.position, catapultDirection * catapultPower, Color.red, 1);
		rbody.velocity = Vector2.zero;
		rbody.AddForce(catapultDirection * catapultPower, ForceMode2D.Impulse);
	}

	void UpdateTortoise()
	{
		Vector3 force;
		
		animator.SetBool("crawling", true);
		if (haveCarrotOnStick)
			force = Vector3.right * tortoiseSpeed * Time.deltaTime;
		else
		{
			force = Vector3.right * tortoiseSpeed * Time.deltaTime;
			if (transform.position.x > 5)
			{
				force += Vector3.down * Time.deltaTime;
				if (transform.position.x > 5.5f)
				{
					deathType = deathType = DeathType.Drawn;
					Death();
				}
			}
		}
		
		transform.position += force;
	}

	void UpdateKite()
	{
		Vector2 startDirection = (kiteAnchor.position - transform.position).normalized;
		float v;
		
		kiteLine.SetPosition(1, transform.position);

		if ((v = Input.GetAxisRaw("Horizontal")) != 0)
			rbody.AddForce(-startDirection * kitePower * v, ForceMode2D.Impulse);

		v = verticalKeyDown;
		if (v != 0)
			rbody.AddForce(Vector2.up * kiteUpControlPower * v, ForceMode2D.Impulse);

		float whirl = Mathf.Sin(Time.timeSinceLevelLoad / .9f) * kiteWirlRange * (1 + Vector3.Distance(kiteAnchor.position, transform.position) / 20);
		rbody.AddForce(Vector2.up * whirl, ForceMode2D.Force);
		Debug.DrawRay(transform.position, startDirection, Color.red);
	}

	void FixedUpdateBalloon()
	{
		rbody.AddForce(Vector2.up * balloonUpVelocity, ForceMode2D.Force);
	}
}
