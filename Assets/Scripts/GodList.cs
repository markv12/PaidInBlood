using UnityEngine;
using System;
using System.Collections.Generic;

public class GodList : ScriptableObject {
    public God[] gods;

    private Dictionary<GodType, God> godDictionary;
    private Dictionary<GodType, God> GodDictionary {
        get {
            if(godDictionary == null) {
                godDictionary = new Dictionary<GodType, God>();
                for (int i = 0; i < gods.Length; i++) {
                    God god = gods[i];
                    godDictionary.Add(god.type, god);
                }
            }
            return godDictionary;
        }
    }

    public God GetGod(GodType type) {
        God result;
        if(GodDictionary.TryGetValue(type, out result)) {
            return result;
        } else {
            Debug.LogError("God type not found: " + type);
            return null;
        }
    }
}

[Serializable]
public class God {
    public Sprite mainSprite;
    public GodType type;
    public string name;
    public string sacrificeButtonText;
    public string entranceSaying;
    public AudioClip happySound;
    public AudioClip angrySound;
    public CardData.CardEffectChance[] goodChances;
    public CardData.CardEffectChance[] badChances;
}
public enum GodType {
    Frog,
    Goat,
    Rabbit
}
