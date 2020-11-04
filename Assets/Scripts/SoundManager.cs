using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioSource musicSource;
    public AudioSource efxSource;
    public static SoundManager instance = null;
    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else if(instance != this){
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); // Это шоб когда мы на новый левел переходили не уничтожался этот объект

    }

    public void PlaySingle(AudioClip clip){
        efxSource.clip = clip;
        efxSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips){ //params allow us to parse a comma separated list of arguments of the same type, as specified by the parametr
        int RandomIndex = Random.Range(0, clips.Length);  // Крч как я понял мы можем закидывать скок хотим параметров, а params все это объединит в массив как мы указали)
        float RandomPitch = Random.Range(lowPitchRange , highPitchRange);
        efxSource.pitch = RandomPitch;
        efxSource.clip = clips[RandomIndex];
        efxSource.Play();
    }

      
    
}
