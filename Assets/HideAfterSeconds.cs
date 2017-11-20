using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAfterSeconds : MonoBehaviour {

	public float		time = 3f;

	void Start () {
		StartCoroutine(Hide());
	}

	IEnumerator Hide()
	{
		yield return new WaitForSeconds(time);

		gameObject.SetActive(false);
	}
}
