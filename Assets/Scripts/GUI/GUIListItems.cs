using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum TravelType
{
	None,
	Balloon,
	BalloonAndStringAndFeather1,
	BalloonAndStringAndFeather2,
	BalloonAndStringAndLeaf,
	Feathers,
	Feather1,
	Feather2,
	SpoonAndElastic,
	SpoonAndElasticAndLeaf,
	KiteAndFriendAndString,
	Kite,
	TortoiseAndCarrotAndString,
	Tortoise,
}

[System.Serializable]
public struct TravelTypeTransition
{
	public TravelType	type;
	public Sprite		sprite;
	public string		text;
}

public class GUIListItems : MonoBehaviour
{
	public Image[]	equipedImages;

	public Sprite	notWorkingSprite;

	[SerializeField]
	public List< TravelTypeTransition > travelTransitions;

	public Button	tryButton;
	public Button	backToResearch;

	public AudioClip	craftClip;
	public AudioClip	craftClickClip;

	Image[]	images;

	Sprite	defaultEquipedSprite;

	AudioSource	audioSource;

	Dictionary< TravelType, List< ItemType > > itemsToTravelType = new Dictionary< TravelType, List< ItemType > >()
	{
		{TravelType.Balloon, new List< ItemType >(){ItemType.Baloon, ItemType.String}},
		{TravelType.BalloonAndStringAndFeather1, new List< ItemType >(){ItemType.Baloon, ItemType.String, ItemType.Feather1}},
		{TravelType.BalloonAndStringAndFeather2, new List< ItemType >(){ItemType.Baloon, ItemType.String, ItemType.Feather2}},
		{TravelType.BalloonAndStringAndLeaf, new List< ItemType >(){ItemType.Baloon, ItemType.String, ItemType.BigLeaf}},
		{TravelType.Feathers, new List< ItemType >(){ItemType.Feather1, ItemType.Feather2}},
		{TravelType.Feather1, new List< ItemType >(){ItemType.Feather1, ItemType.Any}},
		{TravelType.Feather2, new List< ItemType >(){ItemType.Feather2, ItemType.Any}},
		{TravelType.SpoonAndElastic, new List< ItemType >(){ItemType.Spoon, ItemType.Elastic}},
		{TravelType.SpoonAndElasticAndLeaf, new List< ItemType >(){ItemType.Spoon, ItemType.Elastic, ItemType.BigLeaf}},
		{TravelType.KiteAndFriendAndString, new List< ItemType >(){ItemType.kite, ItemType.FriendSquirel, ItemType.String}},
		{TravelType.Kite, new List< ItemType >(){ItemType.kite}},
		{TravelType.TortoiseAndCarrotAndString, new List< ItemType >(){ItemType.Tortoise, ItemType.Carrot, ItemType.String}},
		{TravelType.Tortoise, new List< ItemType >(){ItemType.Tortoise, ItemType.Any, ItemType.Any}},
	};

	// Use this for initialization
	void Start ()
	{
		images = GetComponentsInChildren< Image >();

		int i = 0;
		PlayerStorage.instance.equipedItems.Clear();
		foreach (var item in PlayerStorage.instance.GetItems())
			images[i++].sprite = item.sprite;

		defaultEquipedSprite = equipedImages[0].sprite;

		tryButton.onClick.AddListener(() => ShowTravel());
		tryButton.interactable = false;
		backToResearch.onClick.AddListener(() => SceneSwitcher.instance.ShowExploration());

		audioSource = GetComponent< AudioSource >();
	}

	public int CheckValidCraft(List< ItemType > craftRecipe)
	{
		var equipedItems = PlayerStorage.instance.equipedItems;
		int	itemCount = 0;
		
		if (craftRecipe.Count == equipedItems.Count())
		{
			foreach (var itemType in craftRecipe)
			{
				if (itemType == ItemType.Any)
					continue ;

				int count = craftRecipe.Count(it => it == itemType);
				int	count2 = equipedItems.Count(it => it.Value.type == itemType);

				if (count != count2)
					return -1;
				
				itemCount++;
			}
			return itemCount;
		}
		return -1;
	}

	public void ShowTravel()
	{
		audioSource.PlayOneShot(craftClip);
		int	max = -1;
		int i;

		var equipedItems = PlayerStorage.instance.equipedItems;

		PlayerStorage.instance.travelType = TravelType.None;
		foreach (var kp in itemsToTravelType)
		{
			if ((i = CheckValidCraft(kp.Value)) > 0)
			{
				if (i > max)
				{
					PlayerStorage.instance.travelType = kp.Key;
					max = i;
				}
			}
		}

		if (PlayerStorage.instance.travelType == TravelType.SpoonAndElastic)
		{
			PlayerStorage.instance.catapultSecondItem = null;

			foreach (var ei in equipedItems)
				if (ei.Value.type != ItemType.Spoon && ei.Value.type != ItemType.Elastic)
					PlayerStorage.instance.catapultSecondItem = ei.Value;
		}

		var transition = travelTransitions.Where(t => t.type == PlayerStorage.instance.travelType);
		TravelTypeTransition transitionSprite = (transition.Count() != 0) ? transition.First() : default(TravelTypeTransition); 

		Debug.Log("picked sprit: " + transitionSprite + " for type: " + PlayerStorage.instance.travelType);

		if (transitionSprite.sprite == null)
		{
			string	explanation = "default explanation";

			foreach (var eq in equipedItems)
			{
				if (eq.Value.type == ItemType.Carrot)
				{
					explanation = "What did you expect from a carrot ?";
					break ;
				}

				if (eq.Value.type == ItemType.String)
				{
					explanation = "String can only link other objects !";
					break ;
				}

				if (eq.Value.type == ItemType.Elastic)
				{
					explanation = "An elastic without structure is useless... ";
					break ;
				}

				if (eq.Value.type == ItemType.Spoon)
				{
					explanation = "Without elasticity it won't propulse you !";
					break ;
				}
			}

			SceneSwitcher.instance.ShowCraft(notWorkingSprite, explanation);
			return ;
		}

		SceneSwitcher.instance.ShowTravel(transitionSprite.sprite, transitionSprite.text);
	}

	public void OnItemClick(int index)
	{
		audioSource.PlayOneShot(craftClickClip);
		var items = PlayerStorage.instance.GetItems();

		if (index >= items.Count)
			return ;
		
		if (items[index] != null)
		{
			var item = items[index];
			var d = PlayerStorage.instance.equipedItems;

			foreach (var kp in d)
				if (kp.Value.type == items[index].type)
	 				return ;

			if (!d.ContainsKey(0))
			{
				d[0] = item;
				equipedImages[0].sprite = item.sprite;
			}
			else if (!d.ContainsKey(1))
			{
				d[1] = item;
				equipedImages[1].sprite = item.sprite;
			}
			else if (!d.ContainsKey(2))
			{
				d[2] = item;
				equipedImages[2].sprite = item.sprite;
			}

			if (d.Count > 0)
				tryButton.interactable = true;
		}
	}

	public void OnEquipedClick(int index)
	{
		audioSource.PlayOneShot(craftClickClip);
		PlayerStorage.instance.equipedItems.Remove(index);
		equipedImages[index].sprite = defaultEquipedSprite;
	}

	void Update()
	{
	}
}
