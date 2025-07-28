using UnityEngine;
using System.Collections;

public class LapMusicManager : MonoBehaviour
{
    public static LapMusicManager Instance;

    public AudioSource musicSource;
    public AudioClip mainTheme;
    public AudioClip finalLapTheme;

    [Header("Fade y transición")]
    public float fadeDuration = 2.5f;
    public float delayBeforeFinalLap = 1.5f;
    public float volumeDropFactor = 0.4f; // cuánto baja (ej: 0.4 = baja a 40%)

    private bool finalLapStarted = false;
    private float baseVolume;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        baseVolume = musicSource.volume;  // Por ejemplo 0.1
        musicSource.clip = mainTheme;
        musicSource.Play();
    }

    void Update()
    {
        if (!finalLapStarted && PlayerController.Instance.lapCount == RaceManager.Instance.totalLaps - 1)
        {
            finalLapStarted = true;
            StartCoroutine(SwitchToFinalLapMusic());
        }
    }

    public void TemporarilyLowerMusic(float factor = 0.5f, float duration = 1.4f)
    {
        StartCoroutine(FadeDownAndUp(baseVolume * factor, duration));
    }

    IEnumerator FadeDownAndUp(float targetVolume, float waitTime)
    {
        float startVol = musicSource.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, targetVolume, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = targetVolume;

        yield return new WaitForSeconds(waitTime);

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(targetVolume, baseVolume, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = baseVolume;
    }

    IEnumerator SwitchToFinalLapMusic()
    {
        float startVol = musicSource.volume;
        float loweredVolume = baseVolume * volumeDropFactor;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = finalLapTheme;
        musicSource.volume = 0f;
        musicSource.Play();

        yield return new WaitForSeconds(delayBeforeFinalLap);

        // Fade in hasta baseVolume
        for (float t = 0; t < fadeDuration * 1.2f; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, baseVolume, t / (fadeDuration * 1.2f));
            yield return null;
        }
        musicSource.volume = baseVolume;
    }
}

