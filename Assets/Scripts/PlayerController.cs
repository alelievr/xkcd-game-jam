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

	[Space, Header("Balloon control settings")]
	public float			balloonMaxHeight = 10;
	public float			balloonStep = 2;
	public GameObject		balloonSprite;
	public GameObject		feather;
	public float			featherPower = 5;
	public bool				haveFeather = false;

	[Space, Header("Kite control settings")]
	public GameObject		kite;

	float					balloonUpVelocity = 50f;

	bool					catapultThrowed = false;
	Vector2					catapultDirection = Vector2.right;

	Rigidbody2D				rbody;
	public bool				dead { get; private set; }

	Dictionary< PlayerControl, Action >	controlActions = new Dictionary< PlayerControl, Action >();
	Dictionary< PlayerControl, Action >	fixedControlActions = new Dictionary< PlayerControl, Action >();

	void Start ()
	{
		if (dead)
			return ;
		
		rbody = GetComponent< Rigidbody2D >();
		
		controlActions[PlayerControl.Flappy] = UpdateFlappy;
		controlActions[PlayerControl.Balloon] = UpdateBalloon;
		controlActions[PlayerControl.Catapult] = UpdateCatapult;
		controlActions[PlayerControl.Kite] = UpdateKite;
		controlActions[PlayerControl.Tortoise] = UpdateTortoise;

		fixedControlActions[PlayerControl.Balloon] = FixedUpdateBalloon;

		balloonSprite.SetActive(false);
	}
	
	void Update ()
	{
		if (dead)
			return ;
		controlActions[control]();
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		Debug.Log("tag: " + other.collider.tag);
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

	float verticalKeyDown { get { if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.D)) return 1; if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A)) return -1; return 0; } }
	float horizontalKeyDown { get { if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) return 1; if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) return -1; return 0; } }

	void UpdateFlappy()
	{
		float v = verticalKeyDown;
		if (v != 0)
		{
			rbody.velocity = new Vector2(Mathf.Abs(rbody.velocity.x) * -v, 0);
			Debug.DrawLine(transform.position, transform.position + transform.up, Color.blue, 1);
			rbody.AddForce(transform.up * flappyUpForce, ForceMode2D.Impulse);
			rbody.AddTorque(v * 4, ForceMode2D.Impulse);
		}
	}

	void UpdateBalloon()
	{
		balloonSprite.SetActive(true);
		float v = horizontalKeyDown;
		if (v != 0)
			balloonUpVelocity += balloonStep * v;
		if (haveFeather)
		{
			v = verticalKeyDown;
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

	}

	void UpdateKite()
	{
		Vector2 startDirection = Vector3.zero - transform.position;
		float v = verticalKeyDown;
		rbody.gravityScale = 0;
		
		if (v != 0)
		{
		}
	}

	void FixedUpdateBalloon()
	{
		rbody.AddForce(Vector2.up * balloonUpVelocity, ForceMode2D.Force);
	}
}
