using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	public CanvasGroup currentCanvas;

	public CanvasGroup mainCanvas;

	private void Awake() {
		UIManager.Instance.fade.canvas = currentCanvas;
		UIManager.Instance.fade.FadeInWithCallback(delegate {
			StartCoroutine(FadeToMenu(2f));
		}, 4f);
	}

	public void StartGame() {
		UIManager.Instance.fade.canvas = currentCanvas;
		UIManager.Instance.fade.FadeOutWithCallback(delegate {
			SceneManager.LoadScene("SampleScene");
		});
	}

	public void Quit() {
		Application.Quit();
	}

	private IEnumerator FadeToMenu(float seconds) {
		yield return new WaitForSeconds(seconds);
		UIManager.Instance.SwitchCanvas(currentCanvas, mainCanvas, 1.5f);
		currentCanvas = mainCanvas;
	}
}
