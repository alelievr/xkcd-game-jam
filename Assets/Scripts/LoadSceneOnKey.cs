using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneOnKey : MonoBehaviour
{

	public Scene	scene;
	public KeyCode	key;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(key))
		{
			switch (scene)
			{
				case Scene.Craft:
					SceneSwitcher.instance.ShowCraft();
					break ;
				case Scene.Credits:
					SceneSwitcher.instance.ShowCredits();
					break ;
				case Scene.Exploration:
					SceneSwitcher.instance.ShowExploration();
					break ;
				case Scene.History:
					SceneSwitcher.instance.ShowHistory();
					break ;
				case Scene.TitleScreen:
					SceneSwitcher.instance.ShowTitleScreen();
					break ;
				case Scene.Travel:
					SceneSwitcher.instance.ShowTravel();
					break ;
			}
		}
	}
}
