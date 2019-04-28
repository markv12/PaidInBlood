using UnityEngine;

public class MistManager : MonoBehaviour
{
    public RectTransform mist1;
    public RectTransform mist2;
    public float mistWidth;
    public float mistSpeed;

    void Update()
    {
        UpdateMist();
    }

    private void UpdateMist()
    {
        mist1.anchoredPosition += (new Vector2(-mistSpeed, 0) * Time.unscaledDeltaTime);
        mist2.anchoredPosition += (new Vector2(mistSpeed, 0) * Time.unscaledDeltaTime);
        if (mist1.anchoredPosition.x <= -mistWidth)
        {
            mist1.anchoredPosition = Vector2.zero;
        }
        if (mist2.anchoredPosition.x >= mistWidth)
        {
            mist2.anchoredPosition = Vector2.zero;
        }
    }
}
