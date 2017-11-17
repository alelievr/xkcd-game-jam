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

	Rigidbody2D				rbody;
	public bool				dead { get; private set; }

	float					balloonUpVelocity = 50f;

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

	void UpdateFlappy()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			rbody.velocity = new Vector2(rbody.velocity.x, 0);
			rbody.AddForce(Vector2.up * flappyUpForce, ForceMode2D.Impulse);
		}
	}

	void UpdateBalloon()
	{
		balloonSprite.SetActive(true);
		if (Input.GetKeyDown(KeyCode.UpArrow))
			balloonUpVelocity += balloonStep;
		if (Input.GetKeyDown(KeyCode.DownArrow))
			balloonUpVelocity -= balloonStep;
	}

	void UpdateCatapult()
	{

	}

	void UpdateTortoise()
	{

	}

	void UpdateKite()
	{

	}

	void FixedUpdateBalloon()
	{
		rbody.AddForce(Vector2.up * balloonUpVelocity, ForceMode2D.Force);
	}
}
