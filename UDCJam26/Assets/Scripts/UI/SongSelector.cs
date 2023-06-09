using UnityEngine;
using UnityEngine.SceneManagement;

public class SongSelector : MonoBehaviour {
	[SerializeField] private CanvasGroup fadeCanvas;
	public void StartSong(string sceneName) {
		UIManager.Instance.fade.canvas = fadeCanvas;
		UIManager.Instance.fade.FadeInWithCallback(delegate {
			SceneManager.LoadScene(sceneName);
		}, 3f);
	}

	public void ToMainMenu() {
		UIManager.Instance.fade.canvas = fadeCanvas;
		UIManager.Instance.fade.FadeInWithCallback(delegate {
			SceneManager.LoadScene("MainMenu");
		}, 3f);
	}
}
