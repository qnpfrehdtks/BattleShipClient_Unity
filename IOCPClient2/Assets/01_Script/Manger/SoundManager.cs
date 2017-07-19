using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton_Manager<SoundManager>
{

    AudioSource audioSource;
    Dictionary<string, AudioClip> audioTable;

    protected override bool Init()
    {
        AudioSource[] SoundObject = Resources.LoadAll<AudioSource>("Sound/BGM");

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            //  audioSource.clip = new AudioClip();
        }

        audioTable = new Dictionary<string, AudioClip>();

        for (int i = 0; i < SoundObject.Length; i++)
        {
            // GameObject obj = Instantiate(SoundObject[i]) ;
            //   AudioClip clip = obj.GetComponent<AudioSource>().clip;
            AudioClip clip = SoundObject[i].clip;
            //  obj.transform.SetParent(transform);
            //  obj.SetActive(false);
            audioTable.Add(SoundObject[i].gameObject.name, clip);

        }

        return true;
    }

    public void PlayBGM(string soundName)
    {
        if (!audioSource.clip)
        {
            audioSource.clip = audioTable[soundName];
            audioSource.Play();
        }
        ///   Debug.Log(soundName);
        //  Debug.Log(audioSource.clip.name);
        if (audioSource.clip.name == soundName)
        {
            //    Debug.Log(soundName);
            //    Debug.Log(audioSource.clip.name);
            return;
        }

        audioSource.clip = audioTable[soundName];
        audioSource.Play();

    }


    public void playSoundOnseShot(string soundName)
    {
        audioSource.PlayOneShot(audioTable[soundName]);
    }

    public void stopCurBGM()
    {
        audioSource.Stop();
    }

    public void setVolume(float volume)
    {
        audioSource.volume = volume;
    }

}
