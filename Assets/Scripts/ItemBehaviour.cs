using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemBehaviour : MonoBehaviour
{
	[SerializeField]
	public Item	item;

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			PlayerStorage.instance.AddItem(item);
			Destroy(gameObject);
		}
	}
}
