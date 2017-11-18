using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerControl
{
	Flappy,
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

	float					balloonUpVelocity = 50f;

	bool					catapultThrowed = false;
	Vector2					catapultDirection = Vector2.right;
	
	Rigidbody2D				tortoiseRigidbody;

	Rigidbody2D				rbody;
	new Collider2D			collider;
	public bool				dead { get; private set; }

	Dictionary< PlayerControl, Action >	controlActions = new Dictionary< PlayerControl, Action >();
	Dictionary< PlayerControl, Action >	fixedControlActions = new Dictionary< PlayerControl, Action >();

	void Start ()
	{
		if (dead)
			return ;
		
		rbody = GetComponent< Rigidbody2D >();
		collider = GetComponent< Collider2D >();
		
		controlActions[PlayerControl.Flappy] = UpdateFlappy;
		controlActions[PlayerControl.Balloon] = UpdateBalloon;
		controlActions[PlayerControl.Catapult] = UpdateCatapult;
		controlActions[PlayerControl.Kite] = UpdateKite;
		controlActions[PlayerControl.Tortoise] = UpdateTortoise;

		fixedControlActions[PlayerControl.Balloon] = FixedUpdateBalloon;

		balloon.SetActive(control == PlayerControl.Balloon);
		kite.SetActive(control == PlayerControl.Kite);
		tortoise.SetActive(control == PlayerControl.Tortoise);

		tortoiseRigidbody = tortoise.GetComponent< Rigidbody2D >();

		if (control == PlayerControl.Kite)
		{
			rbody.gravityScale = 0;
			defaultWindForce = 0;
			rbody.drag = 2;
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
			float s = 1 + (balloonUpVelocity + (Physics.gravity.y * rbody.gravityScale)) / 10;
			balloon.transform.localScale = Vector3.one * s;
		}
		if (haveFeather)
		{
			v = horizontalKeyDown;
			if (v != 0)
				rbody.AddForce(Vector2.right * -v * featherPower, ForceMode2D.Impulse);
		}
	}

	void UpdateCatapult()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			catapultThrowed = true;
			rbody.AddForce(catapultDirection, ForceMode2D.Impulse);
		}
	}

	void UpdateTortoise()
	{
		Vector3 force = Vector3.right * tortoiseSpeed * Time.deltaTime;
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
