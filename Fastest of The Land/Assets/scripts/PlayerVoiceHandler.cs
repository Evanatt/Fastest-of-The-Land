using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVoiceHandler : MonoBehaviour
{
    [Header("AudioSources separados")]
    public AudioSource voiceSource;   // Para diálogos del personaje
    public AudioSource lapAudioSource;     // Para sonidos de vuelta

    [Header("Clips de vuelta")]
    public AudioClip lap1Sound;        // Sonido normal vuelta 1 y 2
    public AudioClip finalLapSound;    // Sonido final de carrera


    public VoiceData voiceData;
    public float minIntervalBetweenVoices = 2.5f;
    private float lastVoiceTime;

    private AudioClip lastClipPlayed; // guarda el último clip reproducido

    public void PlayCoinLine()
    {
        TryPlayRandomClip(voiceData.coinClips);
    }

    public void PlayAcornLine()
    {
        TryPlayRandomClip(voiceData.acornClips);
    }

    public void PlayLapLine()
    {
        TryPlayRandomClip(voiceData.lapClips);
        int currentLap = PlayerController.Instance.lapCount;

        if (currentLap < RaceManager.Instance.totalLaps)
        {
            lapAudioSource.PlayOneShot(lap1Sound);
        }
        else if (currentLap == RaceManager.Instance.totalLaps)
        {
            lapAudioSource.PlayOneShot(finalLapSound);
        }
        if (LapMusicManager.Instance != null)
        {
            LapMusicManager.Instance.TemporarilyLowerMusic(0.3f, 1.5f);
        }

        // Reproducimos el sonido después de un pequeño delay
        StartCoroutine(PlayLapSoundAfterDelay(currentLap));
    }
    IEnumerator PlayLapSoundAfterDelay(int lap)
    {
        yield return new WaitForSeconds(0.4f); // le da tiempo al fade out

        if (lap < RaceManager.Instance.totalLaps)
        {
            lapAudioSource.PlayOneShot(lap1Sound);
        }
        else if (lap == RaceManager.Instance.totalLaps)
        {
            lapAudioSource.PlayOneShot(finalLapSound);
        }
    }
    private void TryPlayRandomClip(AudioClip[] clips)
    {
        if (Time.time - lastVoiceTime < minIntervalBetweenVoices) return;
        if (clips == null || clips.Length == 0) return;

        AudioClip selectedClip = null;

        if (clips.Length == 1)
        {
            selectedClip = clips[0];
        }
        else
        {
            // Crear una lista de índices válidos excluyendo el último
            List<int> validIndices = new List<int>();
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != lastClipPlayed)
                {
                    validIndices.Add(i);
                }
            }

            // Si por alguna razón no hay opciones (todos son iguales), usar cualquiera
            if (validIndices.Count == 0)
            {
                selectedClip = clips[Random.Range(0, clips.Length)];
            }
            else
            {
                int chosenIndex = validIndices[Random.Range(0, validIndices.Count)];
                selectedClip = clips[chosenIndex];
            }
        }

        if (selectedClip != null)
        {
            voiceSource.PlayOneShot(selectedClip);
            lastClipPlayed = selectedClip;
            lastVoiceTime = Time.time;
        }
    }

}
