using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	public CanvasGroup currentCanvas;

	public CanvasGroup mainCanvas;
	public CanvasGroup instructionCanvas;

	private void Start() {
		UIManager.Instance.fade.canvas = currentCanvas;
		UIManager.Instance.fade.FadeInWithCallback(delegate {
			StartCoroutine(FadeToMenu(2f));
		}, 4f);
	}

	public void StartGame() {
		UIManager.Instance.fade.canvas = currentCanvas;
		UIManager.Instance.fade.FadeOutWithCallback(delegate {
			SceneManager.LoadScene("SongSelection");
		});
	}

	public void ShowMain() {
		UIManager.Instance.fade.canvas = instructionCanvas;
		UIManager.Instance.fade.FadeOut(0.5f);
	}

	public void ShowInstructions() {
		UIManager.Instance.fade.canvas = instructionCanvas;
		UIManager.Instance.fade.FadeIn(0.5f);
	}

	public void Quit() {
		Application.Quit();
	}

	private IEnumerator FadeToMenu(float seconds) {
		yield return new WaitForSeconds(seconds);
		mainCanvas.interactable = true;
		mainCanvas.blocksRaycasts = true;
		UIManager.Instance.SwitchCanvas(currentCanvas, mainCanvas, 1.5f);
		currentCanvas = mainCanvas;
	}
}
