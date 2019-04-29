using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMain : MonoBehaviour {

    [SerializeField]
    private TMP_Text villagerCountText;
    [SerializeField]
    private TMP_Text goatCountText;
    [SerializeField]
    private TMP_Text maidenCountText;
    [SerializeField]
    private TMP_Text youngLadCountText;

    [SerializeField]
    private OverTimeEffectsUIManager notificationManager;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private DayIndicator dayIndicator;
    [SerializeField]
    private GodUI godUI;

    [SerializeField]
    private GodList godList;

    private int villagers = 0;
    public int Villagers {
        get { return villagers; }
        set {
            villagers = value;
            villagerCountText.text = "Villagers: " + villagers;
        }
    }

    private int goats = 0;
    public int Goats {
        get { return goats; }
        set {
            goats = value;
            goatCountText.text = "Goats: " + goats;
        }
    }

    private int maidens = 0;
    public int Maidens {
        get { return maidens; }
        set {
            maidens = value;
            maidenCountText.text = "Maidens: " + maidens;
        }
    }

    private int youngLads = 0;
    public int YoungLads {
        get { return youngLads; }
        set {
            youngLads = value;
            youngLadCountText.text = "Young Lads: " + youngLads;
        }
    }

    private GameState _currentState = GameState.ChoosingCard;
    private GameState currentState {
        get { return _currentState; }
        set {
            _currentState = value;
            Debug.Log(Time.frameCount + " State set: " + value);
        }
    }
    public delegate void DayChangedDelegate(int prevDay, int currentDay);
    public event DayChangedDelegate DayChangedEvent;

    private int currentDay = 0;
    public int CurrentDay {
        get {
            return currentDay;
        }
        set {
            int prevDay = currentDay;
            currentDay = value;
            dayIndicator.GoToDayNumber(currentDay);
            DayChangedEvent?.Invoke(prevDay, currentDay);
        }
    }
    List<CardData.DelayedEffect> delayedEffects = new List<CardData.DelayedEffect>();


    [SerializeField]
    private Deck deck;

    void Start() {
        deck.Initialize();
        uiManager.cardClickedEvent += CardClickedHandler;
        godUI.sacrificeClickedEvent += SacrificeClickedHandler;
        DrawCards();
        Villagers = 30;
        Goats = 1;
        Maidens = 1;
        YoungLads = 1;
        DayChangedEvent += DayChangedHandler;
    }

    void DayChangedHandler(int prev, int curDay){
        for (int i = 0; i < delayedEffects.Count; i++)
        {
            CardData.DelayedEffect dE = delayedEffects[i];
            if(dE.dayOfActivation <= CurrentDay){
                CardData.CardEffectChance e = PickEffect(dE.effects);
                ApplyEffect(e);
                delayedEffects.RemoveAt(i);
                i--;
                ShowNextEventMessage(UIManager.CARD_MOVE_TIME);
                currentState = GameState.ViewingDayStartEffect;
                currentState = GameState.ViewingDayStartEffect;
                // Debug.Log(message);
                //show message
            }
        }
        notificationManager.RefreshNotificaitons(CurrentDay);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (currentState == GameState.ViewingDayEndEffect) {
                if (uiManager.EffectPanelOpen()) {
                    if (eventMessages.Count > 0) {
                        ShowNextEventMessage(0.4f);
                        uiManager.CloseEffectPanel(0.3f);
                    } else {
                        CurrentDay++;
                        if (IsDaySacrificeDay(CurrentDay)) {
                            uiManager.HideCards();
                            System.Array values = System.Enum.GetValues(typeof(GodType));
                            God randomGod = godList.GetGod((GodType)values.GetValue(Random.Range(0, values.Length)));
                            godUI.ShowGod(randomGod, CanSacrifice(randomGod.type));
                            currentState = GameState.InteractingWithGod;
                        } else {
                            ResetCards();
                            currentState = GameState.ChoosingCard;
                        }
                        uiManager.CloseEffectPanel();
                    }
                }
            } else if(currentState == GameState.ViewingDayStartEffect) {
                currentState = GameState.ChoosingCard;
                uiManager.CloseEffectPanel();
            }
        }
    }

    private void CardClickedHandler(CardData data) {
        if (currentState == GameState.ChoosingCard) {
            CardData.CardEffectChance choosenEffect = PickEffect(data.effects);
            ApplyEffect(choosenEffect);
            ShowNextEventMessage(UIManager.CARD_MOVE_TIME);

            for (int i = 0; i < data.delayedEffects.Length; i++)
            {
                data.delayedEffects[i].dayOfActivation = CurrentDay + data.delayedEffects[i].duration; 
                delayedEffects.Add(data.delayedEffects[i]);
                notificationManager.AddNotification(data.delayedEffects[i].notificaitonText, data.delayedEffects[i].dayOfActivation);
            }
            if(data.discardOnUse)
                deck.RemoveFromDeck(data);
        }
    }
    
    private CardData.CardEffectChance PickEffect(CardData.CardEffectChance[] chances) {
        float effectNumber = Random.Range(0.0f, 1.0f);
        float accum = 0;
        for (int i = 0; i < chances.Length; i++) {
            CardData.CardEffectChance effectChance = chances[i];
            accum += effectChance.chance;
            if (effectNumber <= accum) {
                return effectChance;
            }
        }
        return null;
    }

    private void ApplyEffect(CardData.CardEffectChance effect) {
        string message = "";
        if (effect != null) {
            Villagers += effect.villagerChange;
            Goats += effect.goatChange;
            Maidens += effect.maidenChange;
            YoungLads += effect.youngLadChange;
            deck.AddToDeck(effect.unlockedCards);
            message += CardData.GetEffectText(effect) + System.Environment.NewLine;
        } else {
            message = "Nothing Happened";
        }
        AddEventMessage(message);
    }

    private List<string> eventMessages = new List<string>();
    private void AddEventMessage(string message) {
        eventMessages.Add(message);
    }
    private void ShowNextEventMessage(float waitTime) {
        if (eventMessages.Count > 0) {
            string nextMessage = eventMessages[0];
            Debug.Log("SHOW: " + nextMessage);
            uiManager.ShowEffect(nextMessage, waitTime);
            currentState = GameState.ViewingDayEndEffect;
            eventMessages.RemoveAt(0);
        }
    }

    private void SacrificeClickedHandler(God god, bool successful) {
        if(currentState == GameState.InteractingWithGod) {
            CardData.CardEffectChance choosenEffect;
            if (successful) {
                choosenEffect = PickEffect(god.goodChances);
            } else {
                choosenEffect = PickEffect(god.badChances);
            }
            ApplyEffect(choosenEffect);
            ShowNextEventMessage(UIManager.CARD_MOVE_TIME);

            godUI.HideUI();
            CurrentDay++;
        }
    }

    private void DrawCards() {
        var cards = deck.DrawCards();
        uiManager.ShowCards(cards.Item1, cards.Item2);
    }

    private void ResetCards() {
        var cards = deck.DrawCards();
        uiManager.ResetCards(cards.Item1, cards.Item2);
    }

    public static bool IsDaySacrificeDay(int day) {
        return day % 7 == 0;
    }

    private enum GameState {
        ViewingDayStartEffect,
        ChoosingCard,
        ViewingDayEndEffect,
        InteractingWithGod
    }

    private static string GetSacrificeText(GodType type) {
        switch (type) {
            case GodType.Frog:
                return "Sacrifice Maiden";
            case GodType.Goat:
                return "Sacrifice Goat";
            case GodType.Rabbit:
                return "Sacrifice Young Lad";
            default:
                return "";
        }
    }

    private bool CanSacrifice(GodType type) {
        switch (type) {
            case GodType.Frog:
                return Maidens > 0;
            case GodType.Goat:
                return Goats > 0;
            case GodType.Rabbit:
                return YoungLads > 0;
            default:
                return false;
        }
    }

    private void OnDestroy() {
        uiManager.cardClickedEvent -= CardClickedHandler;
        godUI.sacrificeClickedEvent -= SacrificeClickedHandler;
    }
}
