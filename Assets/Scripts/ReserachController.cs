using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ReserachController : MonoBehaviour
{
	[Header("Control settings")]
	public float		speed = 10;
	public float		jumpPower = 40;

	[Space, Header("Other settings")]
	public Transform	groundCheckCenter;
	public Vector2		groundCheckSize;
	public float		groundCheckRadius = .1f;

	[Space, Header("GUI settings")]
	public GameObject	pickHelpText;
	public float		popupShowTime = 1f;
	public GameObject	pickUpPanel;
	public Image		pickUpImage;
	public Text			pickUpText;

	Rigidbody2D			rbody;
	bool				grounded = false;
	bool				wantsJump = false;

	Collider2D[]		overlapResults = new Collider2D[10];

	List< Collider2D >	pickableObjects = new List< Collider2D >();
	Animator			animator;

	void Start ()
	{
		rbody = GetComponent< Rigidbody2D >();
		animator = GetComponent< Animator >();
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

		if (wantsJump)
		{
			wantsJump = false;
			if (grounded)
			{
				rbody.velocity = new Vector2(rbody.velocity.x, 0);
				rbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
			wantsJump = true;
		
		pickHelpText.SetActive(pickableObjects.Count != 0);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Obstacle")
				SceneSwitcher.instance.ShowExploration();
		
		if (other.GetComponent< ItemBehaviour >() != null)
			pickableObjects.Add(other);
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.GetComponent< ItemBehaviour >() != null)
			pickableObjects.Remove(other);
	}

	public void ShowPickedObjectPopup(Item item)
	{
		pickUpPanel.SetActive(true);
		pickUpImage.sprite = item.sprite;
		pickUpText.text = "You picked " + item.name;

		StartCoroutine(HidePopup());
	}

	IEnumerator HidePopup()
	{
		yield return new WaitForSeconds(popupShowTime);
		pickUpPanel.SetActive(false);
	}

	void GroundCheck()
	{
		grounded = false;
		int cols = Physics2D.OverlapCircleNonAlloc((Vector2)groundCheckCenter.position, groundCheckRadius, overlapResults);
		for (int i = 0; i < cols; i++)
		{
			var res = overlapResults[i];

			if (res.tag == "Ground")
			{
				grounded = true;
				break ;
			}
		}
		animator.SetBool("isJumping", !grounded);
		animator.SetBool("isMoving", Mathf.Abs(rbody.velocity.x) > .2f);
		if (rbody.velocity.x != 0)
		{
			Vector3 scale = transform.localScale;
			scale.x = Mathf.Abs(scale.x) * -Mathf.Sign(rbody.velocity.x);
			transform.localScale = scale;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere(groundCheckCenter.position, groundCheckRadius);
	}
}
