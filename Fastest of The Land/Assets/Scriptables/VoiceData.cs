using UnityEngine;

[CreateAssetMenu(fileName = "NewVoiceData", menuName = "Lander/Voice Data")]
public class VoiceData : ScriptableObject
{
    public AudioClip[] coinClips;
    public AudioClip[] acornClips;
    public AudioClip[] lapClips;
}
