using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayIndicator : MonoBehaviour {

    public RectTransform mainT;
    public TMP_Text textPrefab;

    private const float SPACE_PER_NUMBER = 30;
    private Coroutine moveRoutine;
    public void GoToDayNumber(int dayNumber) {
        float xPos = ((float)dayNumber * SPACE_PER_NUMBER);
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
}
