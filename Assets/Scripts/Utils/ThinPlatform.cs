using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinPlatform : MonoBehaviour
{
	Rigidbody2D			playerRigidbody;


	// Use this for initialization
	void Start ()
	{
		playerRigidbody = GameObject.FindObjectOfType< ReserachController >().GetComponent< Rigidbody2D >();
	}
	
	void Update ()
	{
		Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("ThroughPlatform"), playerRigidbody.velocity.y > 0);
	}
}
