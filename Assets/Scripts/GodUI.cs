using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GodUI : MonoBehaviour {

    public delegate void SacrificeClickedDelegate(CardData data);
    public SacrificeClickedDelegate cardClickedEvent;

    public Image background;
    public GameObject mistObject;
    public CanvasGroup mistGroup;
    public Image godImage;
    public RectTransform godTransform;
    public GodList godList;
    public CanvasGroup buttonGroup;
    public Button sufferButton;
    public Button sacrificeButton;
    public TMP_Text sacrificeButtonText;
    public TMP_Text sayingText;

    public void ShowGod(God god, bool sacrificePossible) {
        StartCoroutine(ShowUI(god, sacrificePossible));
    }

    private void Awake() {
        mistObject.SetActive(false);
        buttonGroup.alpha = 0;
        sufferButton.onClick.AddListener(delegate {  });
    }

    private const float BACKGROUND_FADE_TIME = 2.2f;
    private const float GOD_FADE_TIME = 3f;
    private const float BUTTON_FADE_TIME = 0.666f;
    private IEnumerator ShowUI(God god, bool sacrificePossible) {
        godImage.sprite = god.mainSprite;
        sacrificeButtonText.text = god.sacrificeButtonText;
        Color startColor = Color.clear;
        Color endColor = new Color(0,0,0,0.8f);
        Color godEndColor = new Color(0.9f, 0.9f, 0.9f, 1);
        float progress = 0;
        float elapsedTime = 0;
        mistObject.SetActive(true);
        while (progress <= 1) {
            progress = elapsedTime / BACKGROUND_FADE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            background.color = Color.Lerp(startColor, endColor, progress);
            mistGroup.alpha = progress;
            yield return null;
        }
        progress = 0;
        elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / GOD_FADE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            float firstHalfProgress = Mathf.Min(1, progress * 2);
            if (progress <= 0.5f) {
                godImage.color = Color.Lerp(startColor, endColor, firstHalfProgress);
            }
            float secondHalfProgress = Mathf.Max(0, (progress * 2)-1);
            if (progress > 0.5f) {
                godImage.color = Color.Lerp(endColor, godEndColor, secondHalfProgress);
            }
            float easedProgress = Easing.easeInOutSine(0, 1, progress);
            godTransform.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, easedProgress);
            yield return null;
        }

        sayingText.text = god.entranceSaying;

        progress = 0;
        elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / BUTTON_FADE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            buttonGroup.alpha = progress;
            yield return null;
        }
    }
}
