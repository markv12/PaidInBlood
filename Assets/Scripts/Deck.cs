using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Deck
{
    [SerializeField]
    StartingDeck startingDeck;

    CardList mainList = new CardList();

    public void Initialize() {
        mainList.Initialize(startingDeck.masterList);
    }

    public Tuple<CardData, CardData> DrawCards(){
        CardData firstCard = mainList.GetNextDrawable();
        CardData secondCard;
        do {
            secondCard = mainList.GetNextDrawable();
        } while (firstCard.title == secondCard.title);

        return new Tuple<CardData, CardData>(firstCard, secondCard);
    }

    public void AddToDeck(CardData[] cards) {
        mainList.AddToDeck(cards);
    }

    public void RemoveFromDeck(CardData card) {
        mainList.RemoveFromDeck(card);
    }

    private class CardList {
        List<CardData> theList = new List<CardData>();

        public void Initialize(CardData[] startingDeck) {
            theList = new List<CardData>(startingDeck.Length);
            for (int i = 0; i < startingDeck.Length; i++) {
                theList.Add(startingDeck[i]);
            }
            SortInPlaceRandom(theList);
            currentIndex = 0;
        }

        int _currentIndex;
        int currentIndex {
            get {
                return _currentIndex % theList.Count;
            }
            set {
                _currentIndex = value;
                if (_currentIndex < 0)
                    _currentIndex = 0;
                if(_currentIndex >= theList.Count) {
                    _currentIndex = 0;
                    SortInPlaceRandom(theList);
                }
            }
        }

        private void SortInPlaceRandom(List<CardData> cardList) {
            for (int t = 0; t < cardList.Count; t++) {
                CardData tmp = cardList[t];
                int r = UnityEngine.Random.Range(t, cardList.Count);
                cardList[t] = cardList[r];
                cardList[r] = tmp;
            }
        }

        public CardData GetNextDrawable() {
            CardData c = null;
            while (c == null) {
                if (theList[currentIndex])
                    c = theList[currentIndex];
                currentIndex++;
            }
            return c;
        }

        public void RemoveFromDeck(CardData card) {
            for (int i = 0; i < theList.Count; i++) {
                if (theList[i] == card) {
                    theList.RemoveAt(i);
                    currentIndex--;
                    return;
                }
            }
            Debug.LogError("Card Remove failed");
        }

        public void AddToDeck(CardData[] cards) {
            if (cards == null)
                return;
            for (int i = 0; i < cards.Length; i++) {
                RegisterUsableCardAtRandomPosition(cards[i]);
            }
        }

        void RegisterUsableCardAtRandomPosition(CardData card) {
            int randomIndex = UnityEngine.Random.Range(0, theList.Count);
            theList.Insert(randomIndex, card);
            if (randomIndex < currentIndex) {
                currentIndex++;
            }
        }
    }
}
