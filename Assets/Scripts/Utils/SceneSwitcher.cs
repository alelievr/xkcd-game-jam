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

	public void ShowTitleScreen()
	{
		StartCoroutine(FadeScene(titleScreenSceneName));
	}

	public void ShowHistory()
	{
		StartCoroutine(FadeScene(historySceneName));
	}

	public void ShowExploration()
	{
		StartCoroutine(FadeScene(explorationSceneName));
	}

	public void ShowTravel(Sprite transition)
	{
		if (transition == null)
			StartCoroutine(FadeScene(travelSceneName));
		else
			StartCoroutine(FadeSceneWithSprite(travelSceneName, transition));
	}

	public void ShowCraft()
	{
		StartCoroutine(FadeScene(craftSceneName));
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
				SceneSwitcher.instance.ShowTravel(null);
				break ;
		}
	}

	IEnumerator FadeOut(Image panel)
	{
		float	startTime = Time.time;
		float	alpha;
		Color	defaultColor = panel.color;
		
		do
		{
			alpha = 1 - ((Time.time - startTime) / fadeTime);
			alpha = fadeCurve.Evaluate(alpha);
			if (fadeAudioSource != null)
				fadeAudioSource.volume = 1 - alpha;
			if (panel != null)
				panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, alpha);
			yield return null;
		} while (alpha > 0f);
	}

	IEnumerator FadeIn(Image panel)
	{
		float	startTime = Time.time;
		float	alpha;
		Color	defaultColor = panel.color;
		
		do
		{
			alpha = ((Time.time - startTime) / fadeTime);
			alpha = fadeCurve.Evaluate(alpha);
			if (fadeAudioSource != null)
				fadeAudioSource.volume = 1 - alpha;
			if (panel != null)
				panel.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, alpha);
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

	IEnumerator FadeSceneWithSprite(string sceneName, Sprite sprite)
	{
		Image panel = GameObject.Find("fullScreenPanel").GetComponent< Image >();
		yield return FadeIn(panel);

		Image spritePanel = GameObject.Find("fullScreenSprite").GetComponent< Image >();
		spritePanel.sprite = sprite;
		yield return FadeOut(spritePanel);

		yield return new WaitForSeconds(2);
		
		SceneManager.LoadScene(travelSceneName);

		panel = GameObject.Find("fullScreenPanel").GetComponent< Image >();
		yield return FadeOut(panel);
	}

}
