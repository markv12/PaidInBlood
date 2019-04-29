using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class CardData : ScriptableObject {
    public string title;
    [TextArea]
    public string description;
    public Sprite cardBackground;
    public Sprite artBackground;
    public Sprite art;
    public bool discardOnUse;

    [Reorderable]
    public CardEffectChance[] effects;
    [Reorderable]
    public DelayedEffect[] delayedEffects;

    [Serializable]
    public class DelayedEffect{
        public int duration;
        [HideInInspector]
        public int dayOfActivation;
        public string notificaitonText;
        public CardEffectChance[] effects; 
    }

    [Serializable]
    public class CardEffectChance {
        [Range(0.0f, 1.0f)]
        public float chance;
        public int villagerChange = 0;
        public int goatChange = 0;
        public int maidenChange = 0;
        public int youngLadChange = 0;
        public CardData[] unlockedCards;
        [TextArea]
        public string description;
        public AudioClip audioClip;
    }

    public static string GetEffectText(CardEffectChance effect) {
        StringBuilder result = new StringBuilder();
        result.AppendLine(effect.description);
        if (effect.villagerChange != 0) {
            result.AppendLine(GetChangeLine(effect.villagerChange, "Villager"));
        }
        if (effect.maidenChange != 0) {
            result.AppendLine(GetChangeLine(effect.maidenChange, "Maiden"));
        }
        if (effect.goatChange != 0) {
            result.AppendLine(GetChangeLine(effect.goatChange, "Goat"));
        }
        if (effect.youngLadChange != 0) {
            result.AppendLine(GetChangeLine(effect.youngLadChange, "Young Lad"));
        }
        return result.ToString();
    }

    private static string GetChangeLine(int change, string title) {
        int absChange = Math.Abs(change);
        string sign = change >= 0 ? "+" : "";
        string pluralizedTitle = (absChange == 1) ? title : title + 's';
        return sign + change + " " + pluralizedTitle;
    }
}


