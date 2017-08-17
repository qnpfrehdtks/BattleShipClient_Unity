using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton_Manager<SoundManager>
{
    AudioSource audioSource;
    Dictionary<string, AudioClip> BGMTable;
    Dictionary<string, AudioClip> audioTable;

    protected override bool Init()
    {
        AudioSource[] SoundObject = Resources.LoadAll<AudioSource>("02Sound/BGM");
        AudioSource[] AudioObject = Resources.LoadAll<AudioSource>("02Sound/Audio");

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            //  audioSource.clip = new AudioClip();
        }

        audioTable = new Dictionary<string, AudioClip>();
        BGMTable = new Dictionary<string, AudioClip>();


        // 브금을 집어넣자
        for (int i = 0; i < SoundObject.Length; i++)
        {
            // GameObject obj = Instantiate(SoundObject[i]) ;
            //   AudioClip clip = obj.GetComponent<AudioSource>().clip;
            AudioClip clip = SoundObject[i].clip;
            //  obj.transform.SetParent(transform);
            //  obj.SetActive(false);
            BGMTable.Add(SoundObject[i].gameObject.name, clip);

        }

        // 일반 사운드를 집어넣자.
        for (int i = 0; i < AudioObject.Length; i++)
        {
            // GameObject obj = Instantiate(SoundObject[i]) ;
            //   AudioClip clip = obj.GetComponent<AudioSource>().clip;
            AudioClip clip = AudioObject[i].clip;
            //  obj.transform.SetParent(transform);
            //  obj.SetActive(false);
            audioTable.Add(AudioObject[i].gameObject.name, clip);

        }

        return true;
    }

    public void PlayBGM(string soundName)
    {
        if (!audioSource.clip)
        {
            audioSource.clip = BGMTable[soundName];
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

        audioSource.clip = BGMTable[soundName];
        audioSource.Play();

    }


    public void playSoundOnseShot(string soundName)
    {
        if (audioTable.ContainsKey(soundName))
        {
            audioSource.PlayOneShot(audioTable[soundName]);
        }
        else
        {
            Debug.Log("해당 사운드를 가지고 있지 않음.");
        }
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
