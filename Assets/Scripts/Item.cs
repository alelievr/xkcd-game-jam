using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
	Carrot,
	Tortoise,
	String,
	kite,
	FriendSquirel,
	Spoon,
	Feathers,
	Elastic,
	BigLeaf,
	Baloon,
}

[System.Serializable]
public class Item
{
	public string	name;
	public ItemType	type;
	public Sprite	sprite;
}
