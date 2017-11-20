using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelScroller : MonoBehaviour {

	RectTransform		rt;

	public float scrollSpeed = 1.2f;

	bool			end = false;

	void Start()
	{
		rt = GetComponent< RectTransform >();
	}
	
	// Update is called once per frame
	void Update () {
		var s = rt.offsetMax;
		s.y += scrollSpeed * Time.deltaTime * 40;
		rt.offsetMax = s;

		if (s.y > 1800 && !end)
		{
			end = true;
			SceneSwitcher.instance.ShowTitleScreen(null, null);
		}
	}
}
