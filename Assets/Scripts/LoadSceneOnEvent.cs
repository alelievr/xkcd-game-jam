using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneOnEvent : MonoBehaviour
{
	public void LoadScene(string sceneName)
	{
		SceneSwitcher.instance.ShowScene(sceneName);
	}
}
