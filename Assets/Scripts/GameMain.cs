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
    private BackgroundMusic backgroundMusic;

    [SerializeField]
    private OverTimeEffectsUIManager notificationManager;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private DayIndicator dayIndicator;
    [SerializeField]
    private GodUI godUI;
    [SerializeField]
    private GameOverManager gameOverManager;

    [SerializeField]
    private GodList godList;

    [SerializeField]
    private AudioSource effectAudioSource;
    [SerializeField]
    private AudioSource godAudioSource;
    [SerializeField]
    private AudioSource crowdAudioSource;
    [SerializeField]
    private AudioClip crowdHappyClip;
    [SerializeField]
    private AudioClip crowdTerrorClip;

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
            //Debug.Log(Time.frameCount + " State set: " + value);
        }
    }

    private int currentDay = 0;
    public int CurrentDay {
        get {
            return currentDay;
        }
    }
    private void GoToNextDay(System.Action onDayComplete) {
        currentDay += 1;
        dayIndicator.GoToDayNumber(currentDay, delegate { DayChangedHandler(currentDay, onDayComplete); });
    }

    List<CardData.DelayedEffect> delayedEffects = new List<CardData.DelayedEffect>();


    [SerializeField]
    private Deck deck;

    void Start() {
        deck.Initialize();
        uiManager.cardClickedEvent += CardClickedHandler;
        godUI.sacrificeClickedEvent += SacrificeClickedHandler;
        DrawCards();
        Villagers = 10;
        Goats = 0;
        Maidens = 0;
        YoungLads = 0;
        backgroundMusic.PlayMusic(MusicType.Peaceful);
    }

    void DayChangedHandler(int curDay, System.Action onDayComplete){
        bool messageAdded = false;
        for (int i = 0; i < delayedEffects.Count; i++){
            CardData.DelayedEffect dE = delayedEffects[i];
            if(dE.dayOfActivation <= CurrentDay){
                CardData.CardEffectChance e = PickEffect(dE.effects);
                ApplyEffect(e);
                messageAdded = true;
                delayedEffects.RemoveAt(i);
                i--;
                // Debug.Log(message);
                //show message
            }
        }
        if (messageAdded) {
            DisplayMessageList(delegate { onDayComplete?.Invoke(); }, UIManager.CARD_MOVE_TIME);
        } else {
            onDayComplete?.Invoke();
        }
        notificationManager.RefreshNotificaitons(CurrentDay);
    }

    private void BeginNextTurn() {
        uiManager.HideCards();
        GoToNextDay(delegate {
            if(Villagers <= 0) {
                gameOverManager.ShowGameOver(CurrentDay);
                currentState = GameState.none;
                backgroundMusic.PlayMusic(MusicType.God);
            } else {
                if (IsDaySacrificeDay(CurrentDay)) {
                    uiManager.HideCards();
                    System.Array values = System.Enum.GetValues(typeof(GodType));
                    God randomGod = godList.GetGod((GodType)values.GetValue(Random.Range(0, values.Length)));
                    godUI.ShowGod(randomGod, CanSacrifice(randomGod.type));
                    backgroundMusic.PlayMusic(MusicType.God);
                    currentState = GameState.InteractingWithGod;
                } else {
                    ResetCards();
                    currentState = GameState.ChoosingCard;
                }
            }
        });
    }

    private void CardClickedHandler(CardData data) {
        if (currentState == GameState.ChoosingCard) {
            CardData.CardEffectChance choosenEffect = PickEffect(data.effects);
            ApplyEffect(choosenEffect, data.defaultStartText);
            DisplayMessageList(BeginNextTurn, 0);

            for (int i = 0; i < data.delayedEffects.Length; i++)
            {
                data.delayedEffects[i].dayOfActivation = CurrentDay + data.delayedEffects[i].duration; 
                delayedEffects.Add(data.delayedEffects[i]);
                notificationManager.AddNotification(data.delayedEffects[i].notificaitonText, data.delayedEffects[i].dayOfActivation);
            }
            notificationManager.RefreshNotificaitons(CurrentDay);
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

    private void ApplyEffect(CardData.CardEffectChance effect, string defaultStartText = null) {
        string message = "";
        if (effect != null) {
            Villagers += effect.villagerChange;
            Goats += effect.goatChange;
            Maidens += effect.maidenChange;
            YoungLads += effect.youngLadChange;
            deck.AddToDeck(effect.unlockedCards);
            if(defaultStartText != null && defaultStartText != "") {
                message += defaultStartText + System.Environment.NewLine;
            }
            message += CardData.GetEffectText(effect) + System.Environment.NewLine;
        } else {
            if(defaultStartText != null && defaultStartText != "") {
                message = defaultStartText;
            } else {
                message = "Nothing Happened";
            }
        }
        AddEventMessage(message);
    }

    private List<string> eventMessages = new List<string>();
    private void AddEventMessage(string message) {
        eventMessages.Add(message);
    }
    private void DisplayMessageList(System.Action onComplete, float waitTime) {
        StartCoroutine(_DisplayMessageList(onComplete, waitTime));
    }
    private IEnumerator _DisplayMessageList(System.Action onComplete, float waitTime) {
        currentState = GameState.ViewingEffects;
        for (int i = 0; i < eventMessages.Count; i++) {
            string nextMessage = eventMessages[i];
            if (i == 0) {
                yield return StartCoroutine(uiManager.ShowEffect(nextMessage, waitTime));
            } else {
                yield return StartCoroutine(uiManager.CloseEffectPanel(uiManager.ShowEffect(nextMessage)));
            }
            while (!Input.GetMouseButtonDown(0)){
                yield return null;
            }
        }
        onComplete?.Invoke();
        StartCoroutine(uiManager.CloseEffectPanel(null));
        eventMessages.Clear();
    }

    private void SacrificeClickedHandler(God god, bool successful) {
        if(currentState == GameState.InteractingWithGod) {
            if (successful) {
                RemoveSacrifice(god.type);
            }
            CardData.CardEffectChance choosenEffect;
            if (successful) {
                choosenEffect = PickEffect(god.goodChances);
                godAudioSource.clip = god.happySound;
                godAudioSource.Play();
                crowdAudioSource.clip = crowdHappyClip;
                crowdAudioSource.PlayDelayed(1.666f);
            } else {
                choosenEffect = PickEffect(god.badChances);
                godAudioSource.clip = god.angrySound;
                godAudioSource.Play();
                crowdAudioSource.clip = crowdTerrorClip;
                crowdAudioSource.PlayDelayed(1.666f);
            }
            ApplyEffect(choosenEffect);
            DisplayMessageList(delegate {
                GoToNextDay(delegate {
                    if (Villagers <= 0) {
                        gameOverManager.ShowGameOver(CurrentDay);
                        currentState = GameState.none;
                        backgroundMusic.PlayMusic(MusicType.God);
                    } else {
                        godUI.HideUI();
                        DrawCards();
                        backgroundMusic.PlayMusic(MusicType.Peaceful, 2.5f);
                        currentState = GameState.ChoosingCard;
                    }
                });
            },0);
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
        return day > 0 && day % 7 == 0;
    }

    private enum GameState {
        none,
        ChoosingCard,
        ViewingEffects,
        InteractingWithGod
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

    private void RemoveSacrifice(GodType type) {
        switch (type) {
            case GodType.Frog:
                Maidens--;
                break;
            case GodType.Goat:
                Goats--;
                break;
            case GodType.Rabbit:
                YoungLads--;
                break;
        }
    }

    private void OnDestroy() {
        uiManager.cardClickedEvent -= CardClickedHandler;
        godUI.sacrificeClickedEvent -= SacrificeClickedHandler;
    }
}
