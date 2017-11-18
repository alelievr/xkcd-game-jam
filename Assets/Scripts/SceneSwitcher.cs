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

	public string titleScreenSceneName = "TitleScreen";
	public string historySceneName = "History";
	public string explorationSceneName = "Exploration";
	public string creditsSceneame = "Credits";
	public string craftSceneName = "CraftScreen";
	public string travelSceneName = "Travel";

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

	public void ShowTravel()
	{
		StartCoroutine(FadeScene(travelSceneName));
	}

	public void ShowCraft()
	{
		StartCoroutine(FadeScene(craftSceneName));
	}

	public void ShowCredits()
	{
		StartCoroutine(FadeScene(creditsSceneame));
	}

	IEnumerator FadeOut(Image panel)
	{
		float	startTime = Time.time;
		float	alpha;
		
		do
		{
			alpha = 1 - ((Time.time - startTime) / fadeTime);
			alpha = fadeCurve.Evaluate(alpha);
			if (fadeAudioSource != null)
				fadeAudioSource.volume = 1 - alpha;
			if (panel != null)
				panel.color = new Color(0, 0, 0, alpha);
			yield return null;
		} while (alpha > 0f);
	}

	IEnumerator FadeIn(Image panel)
	{
		float	startTime = Time.time;
		float	alpha;
		
		do
		{
			alpha = ((Time.time - startTime) / fadeTime);
			alpha = fadeCurve.Evaluate(alpha);
			if (fadeAudioSource != null)
				fadeAudioSource.volume = 1 - alpha;
			if (panel != null)
				panel.color = new Color(0, 0, 0, alpha);
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

}
