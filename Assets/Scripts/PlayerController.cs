using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerControl
{
	Flappy,

}

public class PlayerController : MonoBehaviour
{
	[Header("Global settings")]
	public PlayerControl	control;
	public float			defaultWindForce = 1f;
	public float			maxXVelocity = 3f;

	[Space]
	[Header("Falppy control settings")]
	public float			flappyUpForce = 50;

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

		fixedControlActions[PlayerControl.Flappy] = FixedUpdateFlappy;
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

	void FixedUpdateFlappy()
	{
		rbody.AddForce(Vector2.right * defaultWindForce);
	}
}
