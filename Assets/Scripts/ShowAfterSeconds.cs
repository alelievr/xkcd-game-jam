using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowAfterSeconds : MonoBehaviour
{
	public float		timeout = 3f;
	public float		fadeTime = .5f;

	Image[]				images;
	Text[]				texts;

	void Start () {
		images = GetComponentsInChildren< Image >();
		texts = GetComponentsInChildren< Text >();
		foreach (var image in images)
			image.color = new Color(0, 0, 0, 0);
		foreach (var text in texts)
			text.color = new Color(0, 0, 0, 0);
		StartCoroutine(ShowButton());
	}
	
	IEnumerator ShowButton()
	{
		yield return new WaitForSeconds(timeout);

		float	startTime = Time.time;
		float	alpha;
		
		do
		{
			alpha = ((Time.time - startTime) / fadeTime);
			foreach (var image in images)
				image.color = new Color(1, 1, 1, alpha);
			foreach (var text in texts)
				text.color = new Color(1, 1, 1, alpha);
			yield return null;
		} while (alpha < 1f);
	}
}
