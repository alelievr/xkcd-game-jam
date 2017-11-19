using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum TravelType
{
	None,
	Balloon,
	BalloonAndFeather1,
	BalloonAndFeather2,
	BalloonAndLeaf,
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
		{TravelType.Balloon, new List< ItemType >(){ItemType.Baloon}},
		{TravelType.BalloonAndFeather1, new List< ItemType >(){ItemType.Baloon, ItemType.Feather1}},
		{TravelType.BalloonAndFeather2, new List< ItemType >(){ItemType.Baloon, ItemType.Feather2}},
		{TravelType.BalloonAndLeaf, new List< ItemType >(){ItemType.Baloon, ItemType.BigLeaf}},
		{TravelType.Feathers, new List< ItemType >(){ItemType.Feather1, ItemType.Feather2}},
		{TravelType.Feather1, new List< ItemType >(){ItemType.Feather1}},
		{TravelType.Feather2, new List< ItemType >(){ItemType.Feather2}},
		{TravelType.SpoonAndElastic, new List< ItemType >(){ItemType.Spoon, ItemType.Elastic, ItemType.Any}},
		{TravelType.SpoonAndElasticAndLeaf, new List< ItemType >(){ItemType.Spoon, ItemType.Elastic, ItemType.BigLeaf}},
		{TravelType.KiteAndFriendAndString, new List< ItemType >(){ItemType.kite, ItemType.FriendSquirel, ItemType.String}},
		{TravelType.Kite, new List< ItemType >(){ItemType.kite}},
		{TravelType.TortoiseAndCarrotAndString, new List< ItemType >(){ItemType.Tortoise, ItemType.Carrot, ItemType.String}},
		{TravelType.Tortoise, new List< ItemType >(){ItemType.Tortoise}},
	};

	// Use this for initialization
	void Start () {
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

	public void ShowTravel()
	{
		audioSource.PlayOneShot(craftClip);
		//item configs:
		var equipedItems = PlayerStorage.instance.equipedItems;

		PlayerStorage.instance.travelType = TravelType.None;
		foreach (var kp in itemsToTravelType)
		{
			if (kp.Value.All(itemType => equipedItems.Any(e => itemType == ItemType.Any || itemType == e.Value.type)))
			{
				PlayerStorage.instance.travelType = kp.Key;
				break ;
			}
		}

		if (PlayerStorage.instance.travelType == TravelType.SpoonAndElastic)
		{
			PlayerStorage.instance.catapultSecondItem = null;

			foreach (var ei in equipedItems)
				if (ei.Value.type != ItemType.Spoon && ei.Value.type != ItemType.Elastic)
					PlayerStorage.instance.catapultSecondItem = ei.Value;
		}

		var transitionSprites = travelTransitions.Where(t => t.type == PlayerStorage.instance.travelType).Select(t => t.sprite);
		Sprite transitionSprite = (transitionSprites.Count() != 0) ? transitionSprites.First() : null; 

		if (transitionSprite == null)
			transitionSprite = notWorkingSprite;

		SceneSwitcher.instance.ShowTravel(transitionSprite);
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
