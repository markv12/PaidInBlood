using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using UnityEngine.EventSystems;

public class CardFrame : MonoBehaviour {

    public bool clickable = true;

    public delegate void CardClickedDelegate(CardData data);
    public CardClickedDelegate cardClickedEvent;

    public RectTransform rectT;

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
            artImage.sprite = data.art;
            titleText.text = data.title;
            descriptionText.text = GetDescription(data);
        }
    }

    private void Awake() {
        button.onClick.AddListener(delegate { if (clickable) { cardClickedEvent?.Invoke(data); } });
    }

    public void OnPointerEnter(BaseEventData eventData) {
        if (clickable) {
            rectT.localScale = new Vector3(1.02f, 1.02f, 1.02f);
        }
    }

    public void OnPointerExit(BaseEventData eventData) {
        rectT.localScale = Vector3.one;
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
                result.Append("<size=140%>");
                result.Append((effect.chance * 100).ToString());
                result.Append("% ");
                result.Append("<size=100%>");
            }
            result.Append(CardData.GetEffectText(effect));
            result.AppendLine();
        }
        if (totalChance < 1.0 && data.effects.Length > 0) {
            Debug.LogError(data.title + "Percentages don't add up to 1: " + totalChance);
        }
        return result.ToString();
    }
}
