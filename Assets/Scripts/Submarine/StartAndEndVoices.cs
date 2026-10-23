using UnityEngine;
using UnityEngine.Audio;

public class StartAndEndVoices : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup soundGroup;
    [SerializeField] private AudioClip startAudio;
    [SerializeField] private AudioClip endAudio;
    public Reactor reactor;

    float timerMax = 70;
    float timer;


    void Start()
    {
        SoundManeger.instance.PlaySoundFXClip(startAudio, transform, 1f, soundGroup);
    }

    void Update()
    {
        if (reactor.isFixed == true)
        {
            SoundManeger.instance.PlaySoundFXClip(endAudio, transform, 1f, soundGroup);

            timer += Time.deltaTime;

            if (timer >= timerMax)
            {
                reactor.overLoaded = true;
                reactor.isFixed = false;
            }
        }
    }

}
