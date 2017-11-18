using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIListItems : MonoBehaviour
{
	Image[]	images;

	// Use this for initialization
	void Start () {
		images = GetComponentsInChildren< Image >();

		int i = 0;
		if (PlayerStorage.instance != null)
			foreach (var item in PlayerStorage.instance.GetItems())
				images[i++].sprite = item.sprite;
	}

	public void OnItemClick(int index)
	{
		Debug.Log("Clicked " + index);
	}

	void Update()
	{
		
	}
}
