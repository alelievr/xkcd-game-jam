using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Scene
{
	TitleScreen,
	History,
	Exploration,
	Credits,
	Craft,
	Travel,
}

public class SceneSwitcher : MonoBehaviour {

	public float			fadeTime = 1f;
	public AnimationCurve	fadeCurve;
	public AudioSource		fadeAudioSource;

	const string titleScreenSceneName = "TitleScreen";
	const string historySceneName = "History";
	const string explorationSceneName = "Exploration";
	const string creditsSceneame = "Credits";
	const string craftSceneName = "CreaftScreen";
	const string travelSceneName = "Travel";

	public static SceneSwitcher instance;

	void Awake()
	{
		instance = this;
		DontDestroyOnLoad(this);
	}

	public void ShowTitleScreen(Sprite deadScreen, string text)
	{
		if (deadScreen == null)
			StartCoroutine(FadeScene(titleScreenSceneName));
		else
			StartCoroutine(FadeSceneWithSprite(titleScreenSceneName, deadScreen, 4f, text));
	}

	public void ShowHistory()
	{
		StartCoroutine(FadeScene(historySceneName));
	}

	public void ShowExploration()
	{
		StartCoroutine(FadeScene(explorationSceneName));
	}

	public void ShowTravel(Sprite transition, string text)
	{
		if (transition == null)
			StartCoroutine(FadeScene(travelSceneName));
		else
			StartCoroutine(FadeSceneWithSprite(travelSceneName, transition, 2, text));
	}

	public void ShowCraft(Sprite transition, string text)
	{
		if (transition == null)
			StartCoroutine(FadeScene(craftSceneName));
		else
			StartCoroutine(FadeSceneWithSprite(craftSceneName, transition, 2, text));
	}

	public void ShowCredits()
	{
		StartCoroutine(FadeScene(creditsSceneame));
	}

	public void ShowScene(string sceneName)
	{
		StartCoroutine(FadeScene(sceneName));
	}

	public void ShowScene(Scene scene)
	{
		switch (scene)
		{
			case Scene.Craft:
				SceneSwitcher.instance.ShowCraft(null, null);
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
				SceneSwitcher.instance.ShowTitleScreen(null, null);
				break ;
			case Scene.Travel:
				SceneSwitcher.instance.ShowTravel(null, null);
				break ;
		}
	}

	IEnumerator FadeOut(Image panel, Image spriteImage = null, Text text = null)
	{
		float	startTime = Time.time;
		float	alpha;
		Color	defaultColor = panel.color;
		Color	spriteColor = (spriteImage != null) ? spriteImage.color : Color.white;
		Color	defaultTextColor = (text != null) ? text.color : Color.white;
		
		do
		{
			alpha = 1 - ((Time.time - startTime) / fadeTime);
			alpha = fadeCurve.Evaluate(alpha);
			spriteColor.a = alpha;
			defaultColor.a = alpha;
			if (spriteImage != null)
				spriteImage.color = spriteColor;
			if (fadeAudioSource != null)
				fadeAudioSource.volume = 1 - alpha;
			if (panel != null)
				panel.color = defaultColor;
			yield return null;
		} while (alpha > 0f);
	}

	IEnumerator FadeIn(Image panel, Text text = null)
	{
		float	startTime = Time.time;
		float	alpha;
		Color	defaultColor = panel.color;
		Color	defaultTextColor = (text != null) ? text.color : Color.white;
		
		do
		{
			alpha = ((Time.time - startTime) / fadeTime);
			alpha = fadeCurve.Evaluate(alpha);
			defaultTextColor.a = alpha;
			defaultColor.a = alpha;
			if (text != null)
				text.color = defaultTextColor;
			if (fadeAudioSource != null)
				fadeAudioSource.volume = alpha;
			if (panel != null)
				panel.color = defaultColor;
			yield return null;
		} while (alpha < 1f);
	}

	IEnumerator FadeScene(string sceneName)
	{
		Image panel = GameObject.Find("fullScreenPanel").GetComponent< Image >();
		yield return FadeIn(panel);
		
		SceneManager.LoadScene(sceneName);

		panel = GameObject.Find("fullScreenPanel").GetComponent< Image >();
		yield return FadeOut(panel);
	}

	IEnumerator FadeSceneWithSprite(string sceneName, Sprite sprite, float time = 2, string text = null)
	{
		Image panel = GameObject.Find("fullScreenPanel").GetComponent< Image >();
		yield return FadeIn(panel);

		Image spritePanel = GameObject.Find("fullScreenSprite").GetComponent< Image >();
		Text textComp = GameObject.Find("fullScreenText").GetComponent< Text >();
		textComp.text = text;
		spritePanel.sprite = sprite;
		yield return FadeIn(spritePanel, textComp);

		yield return new WaitForSeconds(time);
		
		SceneManager.LoadScene(sceneName);

		panel = GameObject.Find("fullScreenPanel").GetComponent< Image >();
		yield return FadeOut(panel, spritePanel, textComp);
	}

}
