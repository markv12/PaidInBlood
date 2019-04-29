using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour {

    public Button startButton;
    public Image coverImage;
    public AudioSource music;
    private bool clicked = false;
    void Start()
    {
        startButton.onClick.AddListener(delegate {
            if (!clicked) {
                clicked = true;
                StartCoroutine(startGame());
                startButton.gameObject.SetActive(false);
            }
        });
    }

    private const float START_ANIM_TIME = 1.2f;
    private IEnumerator startGame() {
        float startVolume = music.volume;
        float progress = 0;
        float elapsedTime = 0;
        while (progress <= 1) {
            progress = elapsedTime / START_ANIM_TIME;
            elapsedTime += Time.unscaledDeltaTime;
            coverImage.color = Color.Lerp(Color.clear, Color.black, progress);
            music.volume = Mathf.Lerp(startVolume, 0, progress);
            yield return null;
        }
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        SceneManager.LoadScene("MainScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
