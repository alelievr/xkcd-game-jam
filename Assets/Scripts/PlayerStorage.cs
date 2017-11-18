using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerStorage : MonoBehaviour
{
	List< Item >		playerItems = new List< Item >();

	void Awake()
	{
		instance = this;
	}

	public void AddItem(Item item)
	{
		if (playerItems.Any(i => i.type == item.type))
			return ;
		
		playerItems.Add(item);
	}

	public List< Item > GetItems()
	{
		return playerItems;
	}
	
	public static PlayerStorage instance { get; private set; }
}
