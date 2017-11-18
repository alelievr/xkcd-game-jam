using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerStorage : MonoBehaviour
{
	List< Item >					playerItems = new List< Item >();
	public Dictionary< int, Item >	equipedItems = new Dictionary< int, Item >();

	void Awake()
	{
		Item i = new Item();
		i.name = "f";
		i.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath< Sprite >("Assets/Sprites/items/balloon.png");
		i.type = ItemType.Baloon;

		playerItems.Add(i);
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
