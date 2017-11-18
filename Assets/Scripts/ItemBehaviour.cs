using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemBehaviour : MonoBehaviour
{
	[SerializeField]
	public Item	item;

	bool playerNear = false;

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
			playerNear = true;
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
			playerNear = false;
	}
	
	public void Update()
	{
		if (playerNear && Input.GetKeyDown(KeyCode.Space))
		{
			item.sprite = GetComponent< SpriteRenderer >().sprite;
			PlayerStorage.instance.AddItem(item);
			Destroy(gameObject);
		}
	}
}
