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
			SceneSwitcher.instance.ShowScene(scene);
	}
}
