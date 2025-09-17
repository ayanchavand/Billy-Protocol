using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CrossfadeToNextScene : MonoBehaviour
{
    public static CrossfadeToNextScene Instance { get; private set; }

    [Header("Fade Settings")]
    public Image fadeImage; // Fullscreen black image (initial alpha = 1)
    public float defaultFadeDuration = 1f;
    public float reloadFadeDuration = 0.5f;
    public float entryFadeDuration = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one per scene
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (fadeImage != null)
            StartCoroutine(FadeFromBlack(entryFadeDuration));
        else
            Debug.LogWarning("CrossfadeToNextScene: No fadeImage assigned!");
    }

    public void StartTransitionToNextScene()
    {
        StartCoroutine(FadeAndLoadNextScene(defaultFadeDuration));
    }

    public void ReloadCurrentScene()
    {
        StartCoroutine(FadeAndReloadScene(reloadFadeDuration));
    }

    private IEnumerator FadeAndLoadNextScene(float duration)
    {
        yield return StartCoroutine(Fade(0f, 1f, duration)); // Fade to black

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextIndex);
    }

    private IEnumerator FadeAndReloadScene(float duration)
    {
        yield return StartCoroutine(Fade(0f, 1f, duration)); // Fade to black

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    private IEnumerator FadeFromBlack(float duration)
    {
        yield return StartCoroutine(Fade(1f, 0f, duration));
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            fadeImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        fadeImage.color = new Color(c.r, c.g, c.b, to);
    }
}
