using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverManager : MonoBehaviour {

    public CanvasGroup mainGroup;
    public Image background;
    public Button restartButton;
    public TMP_Text turnText;

    private void Awake() {
        restartButton.onClick.AddListener(delegate {
            this.EnsureCoroutineStopped(ref showGameOverRoutine);
            background.color = Color.black;
            mainGroup.alpha = 1;
            StartCoroutine(FadeToRestart());
        });
    }

    private const float GAME_OVER_FADE_TIME = 0.8f;
    private Coroutine showGameOverRoutine = null;
    public void ShowGameOver(int turnsSurvived) {
        mainGroup.alpha = 0;
        gameObject.SetActive(true);
        turnText.text = "Survived " + turnsSurvived.ToString() + " Turns";
        showGameOverRoutine = StartCoroutine(_ShowGameOver());
    }
    private IEnumerator _ShowGameOver() {
        float progress = 0;
        float elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / GAME_OVER_FADE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            background.color = Color.Lerp(Color.clear, Color.black, progress);
            mainGroup.alpha = progress;
            yield return null;
        }
        background.color = Color.black;
        mainGroup.alpha = 1;
        showGameOverRoutine = null;
    }

    private IEnumerator FadeToRestart() {
        float progress = 0;
        float elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / GAME_OVER_FADE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            background.color = Color.Lerp(Color.black, Color.white, progress);
            mainGroup.alpha = 1 - progress;
            yield return null;
        }
        background.color = Color.white;
        mainGroup.alpha = 0;
        yield return null;
        SceneManager.LoadScene("TitleScene");
    }
}
