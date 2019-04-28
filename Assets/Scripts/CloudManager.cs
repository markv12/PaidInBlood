using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [SerializeField]
    RectTransform rectT;

    private Vector2 direction;
    private const float CLOUD_SPEED = 15;
    private const float Y_LIMIT = 540;
    private const float X_LIMIT = 960;
    private const float PADDING = 350;
    private void Awake() {
        rectT.anchoredPosition = new Vector2(Random.Range(-X_LIMIT, X_LIMIT), Random.Range(-Y_LIMIT, Y_LIMIT));
        direction = -(rectT.anchoredPosition.normalized + new Vector2(Random.Range(-0.9f, 0.9f), Random.Range(-0.9f, 0.9f))) * CLOUD_SPEED;
    }

    void Update(){
        rectT.anchoredPosition += direction * Time.deltaTime;

        float absX = System.Math.Abs(rectT.anchoredPosition.x);
        if (absX > (X_LIMIT + PADDING)) {
            direction = new Vector2(-direction.x, direction.y);
            rectT.anchoredPosition *= 0.97f;
        }
 

        float absY = System.Math.Abs(rectT.anchoredPosition.y);
        if (absY > (Y_LIMIT + PADDING)) {
            direction = new Vector2(direction.x, -direction.y);
            rectT.anchoredPosition *= 0.97f;
        }
    }
}
