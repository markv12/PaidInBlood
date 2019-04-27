using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class CardFrame : MonoBehaviour {

    public delegate void CardClickedDelegate(CardData data);
    public CardClickedDelegate cardClickedEvent;

    public Button button;

    public Image cardBackgroundImage;
    public Image artBackgroundImage;
    public Image artImage;
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    private CardData data;
    public CardData Data {
        get { return data; }
        set {
            data = value;
            cardBackgroundImage.sprite = data.cardBackground;
            artBackgroundImage.sprite = data.artBackground;
            artImage.sprite = data.art;
            titleText.text = data.title;
            descriptionText.text = GetDescription(data);
        }
    }

    private void Awake() {
        button.onClick.AddListener(delegate { cardClickedEvent?.Invoke(data); });
    }

    private static string GetDescription(CardData data) {
        StringBuilder result = new StringBuilder();
        result.AppendLine(data.description);
        result.AppendLine();
        float totalChance = 0;
        for (int i = 0; i < data.effects.Length; i++) {
            CardData.CardEffectChance effect = data.effects[i];
            totalChance += effect.chance;
            if (effect.chance < 1) {
                result.Append((effect.chance * 100).ToString());
                result.Append("% ");
            }
            result.Append(CardData.GetEffectText(effect));
            result.AppendLine();
        }
        if (totalChance < 1.0) {
            Debug.LogError(data.title + "Percentages don't add up to 1: " + totalChance);
        }
        return result.ToString();
    }
}
