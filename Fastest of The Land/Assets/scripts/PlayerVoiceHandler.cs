using System.Collections.Generic;
using UnityEngine;

public class PlayerVoiceHandler : MonoBehaviour
{
    public VoiceData voiceData;
    public AudioSource voiceSource;
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
