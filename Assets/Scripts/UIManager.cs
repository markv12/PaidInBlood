using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour {
    public CardFrame.CardClickedDelegate cardClickedEvent;

    public GameObject effectUI;
    public CanvasGroup effectUIGroup;
    public TMP_Text effectText;

    public CardFrame card1;
    public CardFrame card2;

    private void Awake() {
        effectUI.SetActive(false);
        card1.cardClickedEvent += CardClickedHandler;
        card2.cardClickedEvent += CardClickedHandler;
    }

    private void CardClickedHandler(CardData data) {
        cardClickedEvent?.Invoke(data);
    }

    public void ShowCards(CardData cd1, CardData cd2) {
        card1.Data = cd1;
        card2.Data = cd2;
    }

    private Coroutine fadeRoutine;
    public void ShowEffect(string effectMessage) {
        effectText.text = effectMessage;
        this.EnsureCoroutineStopped(ref fadeRoutine);
        fadeRoutine = StartCoroutine(FadeEffectPanel(true));
    }

    public void CloseEffectPanel() {
        this.EnsureCoroutineStopped(ref fadeRoutine);
        fadeRoutine = StartCoroutine(FadeEffectPanel(false));
    }

    private void OnDestroy() {
        card1.cardClickedEvent -= CardClickedHandler;
        card2.cardClickedEvent -= CardClickedHandler;
    }

    private const float FADE_TIME = 0.4f;
    private IEnumerator FadeEffectPanel(bool fadeIn) {
        if (fadeIn) {
            effectUI.SetActive(true);
        }
        float startAlpha = effectUIGroup.alpha;
        float endAlpha = fadeIn ? 1 : 0;
        float progress = 0;
        float elapsedTime = 0;
        while(progress <= 1) {
            progress = elapsedTime / FADE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            effectUIGroup.alpha = Easing.easeInOutSine(startAlpha, endAlpha, progress);
            yield return null;
        }
        effectUIGroup.alpha = endAlpha;
        if (!fadeIn) {
            effectUI.SetActive(false);
        }
        fadeRoutine = null;
    }
}
