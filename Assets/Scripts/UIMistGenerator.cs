using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMistGenerator : MonoBehaviour
{
    public GameObject baseImage;
    public int particleCount;
    public RectTransform mistContainer;
    public Color mistColor;
    private Color mistClearColor;
    public Vector2 spawnDimensions;
    public Vector2 sizeRange;
    public Vector2 movementSpeedRange;
    public float emptyCenterRadius;

    private MistParticle[] particles;

    void Awake()
    {
        mistClearColor = new Color(mistColor.r, mistColor.g, mistColor.b, 0);

        particles = new MistParticle[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            GameObject newParticleObject = Instantiate(baseImage.gameObject);
            MistParticle newParticle = new MistParticle
            {
                rectTransform = newParticleObject.GetComponent<RectTransform>(),
                image = newParticleObject.GetComponent<Image>(),
                sizeRange = sizeRange,
                movementSpeedRange = movementSpeedRange,
                spawnDimensions = spawnDimensions,
                emptyCenterRadius = emptyCenterRadius,
                mistColor = mistColor,
                mistClearColor = mistClearColor
            };
            newParticle.rectTransform.SetParent(mistContainer, false);
            particles[i] = newParticle;
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            StartCoroutine(particles[i].Co_Mist(this));
        }
    }

    private class MistParticle
    {
        public Vector2 sizeRange;
        public Vector2 movementSpeedRange;
        public Color mistColor;
        public Color mistClearColor;
        public Vector2 spawnDimensions;
        public float emptyCenterRadius;

        public RectTransform rectTransform;
        public Image image;

        public IEnumerator Co_Mist(MonoBehaviour coroutineSource)
        {
            bool initialPass = true;
            while (true)
            {
                Vector2 velocity = GetVelocity();
                float lifetime = UnityEngine.Random.Range(11f, 30f);
                float size = UnityEngine.Random.Range(sizeRange.x, sizeRange.y);
                coroutineSource.StartCoroutine(Co_MistMovement(lifetime, velocity));
                yield return coroutineSource.StartCoroutine(Co_MistFade(lifetime, size, !initialPass));
                initialPass = false;
            }
        }

        private Vector2 GetVelocity()
        {
            float movMin = movementSpeedRange.x;
            float movMax = movementSpeedRange.y;

            float x = UnityEngine.Random.Range(-movMax, movMax);
            x += (x > 0) ? movMin : -movMin;
            float y = UnityEngine.Random.Range(-movMax, movMax);
            y += (y > 0) ? movMin : -movMin;
            return new Vector2(x, y);
        }

        private bool exitEarly = false;
        private IEnumerator Co_MistMovement(float lifetime, Vector2 movementSpeed)
        {
            exitEarly = false;
            rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0f, 360f)));
            float x = spawnDimensions.x;
            float y = spawnDimensions.y;
            rectTransform.anchoredPosition = GetStartPosition(x, y, emptyCenterRadius);
            float elapsedTime = 0;
            float progress = 0;
            while (progress <= 1 && !exitEarly)
            {
                progress = elapsedTime / lifetime;
                elapsedTime += Time.unscaledDeltaTime;
                rectTransform.anchoredPosition += (movementSpeed * Time.unscaledDeltaTime);
                if (OutOfBounds(rectTransform.anchoredPosition))
                {
                    exitEarly = true;
                }
                yield return null;
            }
        }

        private static Vector2 GetStartPosition(float x, float y, float _emptyCenterRadius)
        {
            Vector2 pos = Vector2.zero;
            for (int i = 0; i < 3; i++)
            {
                pos = new Vector2(UnityEngine.Random.Range(-x, x), UnityEngine.Random.Range(-y, y));
                if(pos.magnitude > _emptyCenterRadius)
                    return pos;
            }
            return pos;
        }

        private bool OutOfBounds(Vector2 anchoredPosition)
        {
            return Mathf.Abs(anchoredPosition.x) > (spawnDimensions.x + 30) || Mathf.Abs(anchoredPosition.y) > (spawnDimensions.y + 30);
        }

        private const float FADE_TIME = 5f;
        private IEnumerator Co_MistFade(float lifetime, float size, bool fadeIn)
        {
            rectTransform.sizeDelta = Vector2.zero;
            image.color = mistClearColor;
            float elapsedTime = 0;
            float progress = 0;
            float firstHalfFadeTime = fadeIn ? FADE_TIME : 0.2f;
            while (progress <= 1 && !exitEarly)
            {
                progress = elapsedTime / firstHalfFadeTime;
                elapsedTime += Time.unscaledDeltaTime;
                image.color = Color.Lerp(mistClearColor, mistColor, progress);
                float currentSize = Mathf.Lerp(0, size, progress);
                rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
                yield return null;
            }
            if (!exitEarly) {
                image.color = mistColor;
            }

            elapsedTime = 0;
            progress = 0;
            float waitTime = lifetime - (firstHalfFadeTime + FADE_TIME);
            while (progress <= 1 && !exitEarly)
            {
                progress = elapsedTime / waitTime;
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            elapsedTime = 0;
            progress = 0;
            float startSize = rectTransform.sizeDelta.x;
            Color startColor = image.color;
            while (progress <= 1) //Don't check exitEarly here because we want it to fade out even if it's been marked as exitEarly
            {
                progress = elapsedTime / FADE_TIME;
                elapsedTime += Time.unscaledDeltaTime;
                image.color = Color.Lerp(startColor, mistClearColor, progress);
                float currentSize = Mathf.Lerp(startSize, 0, progress);
                rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
                yield return null;
            }
            rectTransform.sizeDelta = Vector2.zero;
            image.color = mistClearColor;
        }
    }
}
