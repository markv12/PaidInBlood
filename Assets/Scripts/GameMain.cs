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
    private UIManager uiManager;
    [SerializeField]
    private DayIndicator dayIndicator;

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

    private GameState currentState = GameState.ChoosingCard;
    
    private int currentDay = 0;
    public int CurrentDay {
        get {
            return currentDay;
        }
        set {
            currentDay = value;
            dayIndicator.GoToDayNumber(currentDay);
        }
    }


    [SerializeField]
    private CardData[] cardData;

    void Start() {
        uiManager.cardClickedEvent += CardClickedHandler;
        DrawCards();
        Villagers = 30;
        Goats = 1;
        Maidens = 1;
        YoungLads = 1;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (currentState == GameState.ViewingEffect) {
                DrawCards();
                currentState = GameState.ChoosingCard;
                uiManager.CloseEffectPanel();
                CurrentDay++;
            }
        }
    }

    private void CardClickedHandler(CardData data) {
        if (currentState == GameState.ChoosingCard) {
            string message = "";
            float effectNumber = Random.Range(0.0f, 1.0f);

            float accum = 0;
            for (int i = 0; i < data.effects.Length; i++) {
                CardData.CardEffectChance effectChance = data.effects[i];
                accum += effectChance.chance;
                if(effectNumber <= accum) {
                    Villagers += effectChance.villagerChange;
                    Goats += effectChance.goatChange;
                    Maidens += effectChance.maidenChange;
                    YoungLads += effectChance.youngLadChange;

                    message += CardData.GetEffectText(effectChance) + System.Environment.NewLine;

                    break;
                }
            }
            if (message == "") { message = "Nothing Happened"; }
            uiManager.ShowEffect(message);
            currentState = GameState.ViewingEffect;
        }
    }

    private void DrawCards() {
        CardData card1 = cardData[Random.Range(0, cardData.Length)];
        CardData card2 = cardData[Random.Range(0, cardData.Length)];
        uiManager.ShowCards(card1, card2);
    }

    private enum GameState {
        ChoosingCard,
        ViewingEffect,
    }

    private void OnDestroy() {
        uiManager.cardClickedEvent -= CardClickedHandler;
    }
}
