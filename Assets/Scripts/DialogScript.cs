using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class DialogScript : MonoBehaviour
{
	public string[]		dialogs;
	public GameObject	dialogPanel;
	public Text			text;
	bool				playerNear = false;
	int					dialogIndex = 0;

	void Start()
	{
		dialogPanel.SetActive(false);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			ShowDialog();
			playerNear = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			ShowDialog();
			playerNear = false;
		}
	}

	void ShowDialog()
	{
		if (dialogIndex == dialogs.Length)
		{
			dialogIndex = 0;
			dialogPanel.SetActive(false);
			return ;
		}
		dialogPanel.SetActive(true);
		text.text = dialogs[dialogIndex];
		dialogIndex++;
	}
}
