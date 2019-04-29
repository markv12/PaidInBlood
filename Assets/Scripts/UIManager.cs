﻿using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour {
    public CardFrame.CardClickedDelegate cardClickedEvent;

    public GameObject effectUI;
    public CanvasGroup effectUIGroup;
    public TMP_Text effectText;

    public CardFrame leftCard;
    public CardFrame rightCard;

    private void Awake() {
        effectUI.SetActive(false);
        leftCard.cardClickedEvent += LeftCardClickedHandler;
        rightCard.cardClickedEvent += RightCardClickedHandler;
    }

    private void LeftCardClickedHandler(CardData data) {
        cardClickedEvent?.Invoke(data);
        leftCard.clickable = false;
        AnimateSelectedCard(true);
    }

    private void RightCardClickedHandler(CardData data) {
        cardClickedEvent?.Invoke(data);
        rightCard.clickable = false;
        AnimateSelectedCard(false);
    }

    public void ShowCards(CardData cd1, CardData cd2) {
        leftCard.Data = cd1;
        rightCard.Data = cd2;
        StartCoroutine(MoveCardsIn());
    }

    public void ResetCards(CardData cd1, CardData cd2) {
        StartCoroutine(_ResetCards(cd1, cd2));
    }

    private IEnumerator _ResetCards(CardData cd1, CardData cd2) {
        if (leftCard.rectT.anchoredPosition.y > CARD_OFF_SCREEN_Y || rightCard.rectT.anchoredPosition.y > CARD_OFF_SCREEN_Y) {
            yield return StartCoroutine(MoveCardsOut());
        }
        leftCard.Data = cd1;
        rightCard.Data = cd2;
        yield return StartCoroutine(MoveCardsIn());
    }

    public void HideCards() {
        StartCoroutine(MoveCardsOut());
    }

    public IEnumerator ShowEffect(string effectMessage, float waitTime = 0) {
        return FadeEffectPanel(effectMessage, true, waitTime, FADE_TIME, null);
    }
    public IEnumerator CloseEffectPanel(IEnumerator onComplete) {
        return FadeEffectPanel("", false, 0, FADE_TIME, onComplete);
    }

    private void OnDestroy() {
        leftCard.cardClickedEvent -= LeftCardClickedHandler;
        rightCard.cardClickedEvent -= RightCardClickedHandler;
    }

    private const float CARD_MOVE_IN_TIME = 0.7f;
    private const float CARD_OFF_SCREEN_Y = -960f;
    private static readonly Vector2 leftStartPosition = new Vector2(-300, CARD_OFF_SCREEN_Y);
    private static readonly Vector2 rightStartPosition = new Vector2(300, CARD_OFF_SCREEN_Y);
    private static readonly Vector2 leftEndPosition = new Vector2(-300, 0);
    private static readonly Vector2 rightEndPosition = new Vector2(300, 0);
    private IEnumerator MoveCardsIn() {
        float progress = 0;
        float elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / CARD_MOVE_IN_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            float easedProgress = Easing.easeOutSine(0, 1, progress);
            leftCard.rectT.anchoredPosition = Vector2.Lerp(leftStartPosition, leftEndPosition, easedProgress);
            rightCard.rectT.anchoredPosition = Vector2.Lerp(rightStartPosition, rightEndPosition, easedProgress);
            yield return null;
        }
        leftCard.rectT.anchoredPosition = leftEndPosition;
        rightCard.rectT.anchoredPosition = rightEndPosition;
        leftCard.clickable = true;
        rightCard.clickable = true;
    }

    private const float CARD_MOVE_OUT_TIME = 0.5f;
    private IEnumerator MoveCardsOut() {
        Vector2 leftStart = leftCard.rectT.anchoredPosition;
        Vector2 rightStart = rightCard.rectT.anchoredPosition;
        Vector2 leftEnd = new Vector2(leftCard.rectT.anchoredPosition.x, CARD_OFF_SCREEN_Y);
        Vector2 rightEnd = new Vector2(rightCard.rectT.anchoredPosition.x, CARD_OFF_SCREEN_Y);
        float progress = 0;
        float elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / CARD_MOVE_OUT_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            float easedProgress = Easing.easeInSine(0, 1, progress);
            leftCard.rectT.anchoredPosition = Vector2.Lerp(leftStart, leftEnd, easedProgress);
            rightCard.rectT.anchoredPosition = Vector2.Lerp(rightStart, rightEnd, easedProgress);
            yield return null;
        }
        leftCard.rectT.anchoredPosition = leftStartPosition;
        leftCard.rectT.localScale = Vector3.one;
        rightCard.rectT.anchoredPosition = rightStartPosition;
        rightCard.rectT.localScale = Vector3.one;
    }

    private void AnimateSelectedCard(bool leftSelected) {
        StartCoroutine(_AnimateSelectedCard(leftSelected));
    }

    public const float CARD_MOVE_TIME = 0.4f;
    private IEnumerator _AnimateSelectedCard(bool leftSelected) {
        CardFrame showCard = leftSelected ? leftCard : rightCard;
        CardFrame hideCard = leftSelected ? rightCard : leftCard;
        Vector2 hideStartPosition = hideCard.rectT.anchoredPosition;
        Vector2 hideEndPosition = new Vector2(hideCard.rectT.anchoredPosition.x, CARD_OFF_SCREEN_Y);
        Vector2 showStartPosition = showCard.rectT.anchoredPosition;
        Vector2 showEndPosition = new Vector2(755, -50);
        Vector3 showStartScale = Vector3.one;
        Vector3 showEndScale = new Vector3(0.666f, 0.666f, 0.666f);

        float progress = 0;
        float elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / CARD_MOVE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            float easedProgressHide = Easing.easeInSine(0, 1, progress);
            float easedProgressShow = Easing.easeInOutSine(0, 1, progress);
            hideCard.rectT.anchoredPosition = Vector2.Lerp(hideStartPosition, hideEndPosition, easedProgressHide);
            showCard.rectT.anchoredPosition = Vector2.Lerp(showStartPosition, showEndPosition, easedProgressShow);
            showCard.rectT.localScale = Vector3.Lerp(showStartScale, showEndScale, easedProgressShow);
            yield return null;
        }
        hideCard.rectT.anchoredPosition = hideEndPosition;
        showCard.rectT.anchoredPosition = showEndPosition;
        showCard.rectT.localScale = showEndScale;
    }

    private const float FADE_TIME = 0.3f;
    private IEnumerator FadeEffectPanel(string message, bool fadeIn, float waitTime, float fadeTime, IEnumerator onComplete) {
        if (waitTime > 0) {
            yield return new WaitForSeconds(waitTime);
        }
        if (fadeIn) {
            effectUI.SetActive(true);
            effectText.text = message;
        }
        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;
        float progress = 0;
        float elapsedTime = 0;
        while(progress <= 1) {
            progress = elapsedTime / fadeTime;
            elapsedTime += Time.unscaledDeltaTime;
            effectUIGroup.alpha = Easing.easeInOutSine(startAlpha, endAlpha, progress);
            yield return null;
        }
        effectUIGroup.alpha = endAlpha;
        if (!fadeIn) {
            effectUI.SetActive(false);
            effectText.text = message;
        }
        if(onComplete != null) {
            yield return StartCoroutine(onComplete);
        }
    }

    public bool EffectPanelOpen() {
        return effectUI.activeSelf && effectUIGroup.alpha > 0.75f;
    }
}
