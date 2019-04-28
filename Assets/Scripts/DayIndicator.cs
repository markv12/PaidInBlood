using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayIndicator : MonoBehaviour {

    public RectTransform mainT;
    public GameObject textPrefab;
    private List<TextParts> numberTexts = new List<TextParts>(NUMBER_COUNT);

    private const int NUMBER_COUNT = 24;
    private void Awake() {
        for (int i = 0; i < NUMBER_COUNT; i++) {
            GameObject newTextObj = Instantiate(textPrefab);
            TMP_Text textComp = newTextObj.GetComponent<TMP_Text>();
            textComp.text = i.ToString();
            textComp.color = GetColorForDay(i);

            RectTransform textTrans = newTextObj.GetComponent<RectTransform>();
            textTrans.SetParent(mainT, false);
            textTrans.anchoredPosition = new Vector2(i * SPACE_PER_NUMBER, 0);

            newTextObj.SetActive(true);
            numberTexts.Add(new TextParts { theText = textComp, theTransform = textTrans });
        }
    }

    private const float SPACE_PER_NUMBER = 110;
    private Coroutine moveRoutine;
    public void GoToDayNumber(int dayNumber) {
        int startNumber = dayNumber - NUMBER_COUNT / 2;
        for (int i = 0; i < numberTexts.Count; i++) {
            int theNumber = startNumber + i;

            TextParts theText = numberTexts[i];
            theText.theText.text = theNumber.ToString();
            theText.theText.color = GetColorForDay(theNumber);

            theText.theTransform.anchoredPosition = new Vector2(theNumber * SPACE_PER_NUMBER, 0);
        }

        float xPos = -((float)dayNumber * SPACE_PER_NUMBER);
        this.EnsureCoroutineStopped(ref moveRoutine);
        moveRoutine = StartCoroutine(MoveNumberLine(xPos));
    }

    private const float MOVE_TIME = 0.6f;
    private IEnumerator MoveNumberLine(float xPos) {
        Vector2 startPos = mainT.anchoredPosition;
        Vector2 endPos = new Vector2(xPos, startPos.y);
        float progress = 0;
        float elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / MOVE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            float easedProgress = Easing.easeInOutSine(0, 1, progress);
            mainT.anchoredPosition = Vector2.Lerp(startPos, endPos, easedProgress);
            yield return null;
        }
        mainT.anchoredPosition = endPos;
        moveRoutine = null;
    }

    private static Color GetColorForDay(int day) {
        if(day < 0) {
            return Color.clear;
        } else if (GameMain.IsDaySacrificeDay(day)) {
            return Color.red;
        } else {
            return Color.white;
        }
    }

    private struct TextParts {
        public TMP_Text theText;
        public RectTransform theTransform;
    }

}
