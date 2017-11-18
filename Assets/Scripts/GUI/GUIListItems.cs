using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GUIListItems : MonoBehaviour
{
	public Image[]	equipedImages;

	public Button	tryButton;
	public Button	backToResearch;

	Image[]	images;

	Sprite	defaultEquipedSprite;

	// Use this for initialization
	void Start () {
		images = GetComponentsInChildren< Image >();

		foreach (var a in images)
			Debug.Log(a);

		int i = 0;
		PlayerStorage.instance.equipedItems.Clear();
		foreach (var item in PlayerStorage.instance.GetItems())
			images[i++].sprite = item.sprite;

		defaultEquipedSprite = equipedImages[0].sprite;

		tryButton.onClick.AddListener(() => SceneSwitcher.instance.ShowTravel());
		tryButton.interactable = false;
		backToResearch.onClick.AddListener(() => SceneSwitcher.instance.ShowExploration());
	}

	public void OnItemClick(int index)
	{
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
		PlayerStorage.instance.equipedItems.Remove(index);
		equipedImages[index].sprite = defaultEquipedSprite;
	}

	void Update()
	{
	}
}
