using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GodUI : MonoBehaviour {

    public delegate void SacrificeClickedDelegate(GodType type);
    public SacrificeClickedDelegate sacrificeClickedEvent;

    public Image background;
    public GameObject mistObject;
    public CanvasGroup mistGroup;
    public Image godImage;
    public RectTransform godTransform;
    public GodList godList;
    public CanvasGroup buttonGroup;
    public Button sacrificeButton;
    public TMP_Text sacrificeButtonText;
    public TMP_Text sayingText;

    public void ShowGod(God god, bool sacrificePossible) {
        StartCoroutine(ShowUI(god, sacrificePossible));
        sacrificeButton.onClick.RemoveAllListeners();
        sacrificeButton.onClick.AddListener( delegate {
            sacrificeClickedEvent?.Invoke(god.type);
        });
    }

    private void Awake() {
        mistObject.SetActive(false);
        buttonGroup.alpha = 0;
    }

    private const float BACKGROUND_FADE_TIME = 2.2f;
    private const float GOD_FADE_TIME = 3f;
    private const float BUTTON_FADE_TIME = 0.666f;
    private static readonly Color TRANS_BLACK = new Color(0,0,0,0.8f);
    private static readonly Color GOD_COLOR = new Color(0.9f, 0.9f, 0.9f, 1);
    private IEnumerator ShowUI(God god, bool sacrificePossible) {
        godImage.sprite = god.mainSprite;
        sacrificeButtonText.text = god.sacrificeButtonText;
        float progress = 0;
        float elapsedTime = 0;
        mistObject.SetActive(true);
        while (progress <= 1) {
            progress = elapsedTime / BACKGROUND_FADE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            background.color = Color.Lerp(Color.clear, TRANS_BLACK, progress);
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
                godImage.color = Color.Lerp(Color.clear, TRANS_BLACK, firstHalfProgress);
            }
            float secondHalfProgress = Mathf.Max(0, (progress * 2)-1);
            if (progress > 0.5f) {
                godImage.color = Color.Lerp(TRANS_BLACK, GOD_COLOR, secondHalfProgress);
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

    public void HideUI() {
        StartCoroutine(_HideUI());
    }

    private IEnumerator _HideUI() {
        sayingText.text = "";
        float progress = 0;
        float elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / BUTTON_FADE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            buttonGroup.alpha = 1-progress;
            yield return null;
        }

        progress = 0;
        elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / GOD_FADE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            float firstHalfProgress = Mathf.Min(1, progress * 2);
            if (progress <= 0.5f) {
                godImage.color = Color.Lerp(GOD_COLOR, TRANS_BLACK, firstHalfProgress);
            }
            float secondHalfProgress = Mathf.Max(0, (progress * 2) - 1);
            if (progress > 0.5f) {
                godImage.color = Color.Lerp(TRANS_BLACK, Color.clear, secondHalfProgress);
            }
         
            float easedProgress = Easing.easeInOutSine(0, 1, progress);
            godTransform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, easedProgress);
            yield return null;
        }
        progress = 0;
        elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / BACKGROUND_FADE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            background.color = Color.Lerp(TRANS_BLACK, Color.clear, progress);
            mistGroup.alpha = 1- progress;
            yield return null;
        }
        mistObject.SetActive(false);
    }
}
