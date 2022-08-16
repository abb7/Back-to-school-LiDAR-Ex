using UnityEngine;
using System.Collections;

public class CSGTFader : MonoBehaviour {

	public CanvasGroup fader;
	public float fadeTime = 0.25f;
	public float fadeFinal = 1.0f;

	void Start()
	{
		fader = GetComponentInChildren<CanvasGroup>();
	}

	void OnEnable()
	{
		if (fader == null)
			fader = GetComponentInChildren<CanvasGroup>();

		fader.alpha = 0;

		StartCoroutine(StartFade());
	}

	private IEnumerator StartFade(bool fade = true)
	{
		float elapsedTime = 0.0f;
		float wait = fadeTime;

		yield return null;

		while (elapsedTime < wait)  
		{
			if (fade) {
				fader.alpha = (elapsedTime / wait) * fadeFinal;
			}
			else
				fader.alpha = 1.0f - (elapsedTime / wait);

			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}
}
