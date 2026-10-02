using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManeger : MonoBehaviour
{
    public static SoundManeger instance;
    [SerializeField] private AudioSource soundFXObject;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, AudioMixerGroup outputGroup = null)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = outputGroup;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
}
