using System.Collections;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField]
    private AudioSource theSource;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float maxVolume = 1;
    [SerializeField]
    private AudioClip peaceClip;
    [SerializeField]
    private AudioClip godClip;


    private Coroutine musicFadeRoutine;
    private MusicType currentlyPlayingMusicType = MusicType.none;
    public void PlayMusic(MusicType type) {
        if (type != currentlyPlayingMusicType) {
            this.EnsureCoroutineStopped(ref musicFadeRoutine);
            currentlyPlayingMusicType = type;
            musicFadeRoutine = StartCoroutine(MusicFadeRoutine(type, theSource.isPlaying));
        }
    }

    private const float MUSIC_FADE_TIME = 1.666f;
    private IEnumerator MusicFadeRoutine(MusicType type, bool firstFadeOut) {
        float progress = 0;
        float elapsedTime = 0;
        if (firstFadeOut) {
            float startVolume = theSource.volume;
            while (progress <= 1) {
                progress = elapsedTime / MUSIC_FADE_TIME;
                elapsedTime += Time.unscaledDeltaTime;
                float easedProgress = Easing.easeInOutSine(startVolume, 0, progress);
                theSource.volume = easedProgress;
                yield return null;
            }
            theSource.volume = 0;
            progress = 0;
            elapsedTime = 0;
        }

        theSource.Stop();
        theSource.clip = GetClipForMusicType(type);
        theSource.Play();

        float startVolume2 = theSource.volume;
        while (progress <= 1) {
            progress = elapsedTime / MUSIC_FADE_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            float easedProgress = Easing.easeInOutSine(startVolume2, maxVolume, progress);
            theSource.volume = easedProgress;
            yield return null;
        }
        theSource.volume = maxVolume;
    }

    private AudioClip GetClipForMusicType(MusicType type) {
        switch (type) {
            case MusicType.Peaceful:
                return peaceClip;
            case MusicType.God:
                return godClip;
        }
        return peaceClip;
    }
}
public enum MusicType {
    none,
    Peaceful,
    God
}
