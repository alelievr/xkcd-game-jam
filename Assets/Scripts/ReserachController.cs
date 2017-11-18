using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ReserachController : MonoBehaviour
{
	[Header("Control settings")]
	public float		speed = 10;
	public float		jumpPower = 40;

	[Space, Header("Other settings")]
	public Transform	groundCheckCenter;
	public Vector2		groundCheckSize;

	Rigidbody2D			rbody;
	bool				grounded = false;

	Collider2D[]		overlapResults = new Collider2D[10];

	void Start ()
	{
		rbody = GetComponent< Rigidbody2D >();
		GetComponent< Collider2D >().sharedMaterial.friction = 0f;
	}

	void FixedUpdate ()
	{
		float v;
		rbody.velocity = new Vector2(0, rbody.velocity.y);
		if ((v = Input.GetAxisRaw("Horizontal")) != 0)
		{
			rbody.velocity = new Vector2(speed * v, rbody.velocity.y);
		}
		GroundCheck();

		if (grounded && Input.GetKeyDown(KeyCode.UpArrow))
		{
			rbody.velocity = new Vector2(rbody.velocity.x, 0);
			rbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
		}
	}

	void GroundCheck()
	{
		grounded = false;
		int cols = Physics2D.OverlapCapsuleNonAlloc((Vector2)groundCheckCenter.position, groundCheckSize, CapsuleDirection2D.Horizontal, 0, overlapResults);
		for (int i = 0; i < cols; i++)
		{
			var res = overlapResults[i];

			if (res.tag == "Ground")
			{
				grounded = true;
				break ;
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere(groundCheckCenter.position, groundCheckSize.x);
	}
}
