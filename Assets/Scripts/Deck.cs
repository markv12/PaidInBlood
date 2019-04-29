using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Deck
{
    [SerializeField]
    StartingDeck startingDeck;
    List<CardData> deck;
    
    int _currentIndex;
    int currentIndex{
        get{
            return _currentIndex % deck.Count;
        }
        set{
            _currentIndex = value;
            if(_currentIndex < 0)
                _currentIndex = 0;
        }
    }

    public void Initialize(){
        deck = new List<CardData>(startingDeck.masterList.Length);
        for (int i = 0; i < startingDeck.masterList.Length; i++)
        {
            deck.Add(startingDeck.masterList[i]);
        }
        // SortInPlaceRandom(deck);
        currentIndex = 0;
    }

    private void SortInPlaceRandom(List<CardData> cardList) {
        for (int t = 0; t < cardList.Count; t++) {
            CardData tmp = cardList[t];
            int r = UnityEngine.Random.Range(t, cardList.Count);
            cardList[t] = cardList[r];
            cardList[r] = tmp;
        }
    }

    public void Reset(){
        currentIndex = 0;
    }

    public Tuple<CardData, CardData> DrawCards(){
        return new Tuple<CardData, CardData>(GetNextDrawable(), GetNextDrawable());
    }

    CardData GetNextDrawable(){
        CardData c = null;
        while(c == null){
            if(deck[currentIndex])
                c = deck[currentIndex];
            currentIndex++;
        }
        return c;
    }

    public void RemoveFromDeck(CardData card)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if(deck[i] == card){
                deck.RemoveAt(i);
                currentIndex--;
                return;
            }
        }
        Debug.LogError("Card Remove failed");
    }

    public void AddToDeck(CardData[] cards){
        if(cards == null)
            return;
        for (int i = 0; i < cards.Length; i++)
        {
            RegisterUsableCardAtRandomPosition(cards[i]);
        }
    }

    void RegisterUsableCardAtRandomPosition(CardData card) {
        int randomIndex = UnityEngine.Random.Range(0, deck.Count);
        deck.Insert(randomIndex, card);
        if(randomIndex < currentIndex){
            currentIndex++;
        }
    }
}
