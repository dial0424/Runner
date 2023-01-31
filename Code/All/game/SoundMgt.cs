using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgt
{
    public enum sceneName
    {
        Loby = 0,
        Proc,
        Fever,
        Result,
        length
    }
    sceneName currSceneName;
    
    public AudioSource bgSound;
    AudioClip[] bgClips;

    AudioSource touchSound;
    AudioClip[] touchClips;
    AudioClip[] cookieClips;
    AudioClip[] procEftClips;
    AudioClip[] resultEftClips;

    AudioSource eftSound;

    public AudioSource[] audioSources;

    public SoundMgt()
    {
        currSceneName = 0;

        bgClips = new AudioClip[3];
        for(int i = 0; i < 3; i++)
        {
            bgClips[i] = Resources.Load<AudioClip>("BGM_" + i);
        }

        //0: normaltouh, 1: button touch, 2: page button
        touchClips = new AudioClip[3];
        for(int i = 0; i < 3; i++)
        {
            touchClips[i] = Resources.Load<AudioClip>("touch" + i);
        }

        //0: jump, 1: slide, 2: crush, 3: gofever 
        cookieClips = new AudioClip[4];
        //0: score, 1: money, 2: star, 3: timer
        procEftClips = new AudioClip[4];
        for(int i=0;i<4;i++)
        {
            cookieClips[i] = Resources.Load<AudioClip>("cookieSound" + i);
            procEftClips[i] = Resources.Load<AudioClip>("procEft" + i);
        }

        //0: result, 1: score up 
        resultEftClips = new AudioClip[2];
        for(int i=0;i<resultEftClips.Length;i++)
        {
            resultEftClips[i] = Resources.Load<AudioClip>("resultEft" + i);
        }

        //0: bg, 1:touch/btn or cookie, 2:eft
        audioSources = new AudioSource[3];
        for (int i = 0; i < audioSources.Length; i++)
            audioSources[i] = new AudioSource();
    }

    public void playBgSound(string name)
    {
        sceneName s;
        for (s = 0; s < sceneName.length; s++)
        {
            currSceneName = s;
            if (s.ToString() == name)
                break;
        }
        if(s == sceneName.length - 1)
        {
            audioSources[0].clip = resultEftClips[0];
            audioSources[0].loop = false;
            audioSources[0].Play();
            return;
        }
        audioSources[0].clip = bgClips[(int)currSceneName];
		audioSources[0].loop = true;
		audioSources[0].Play();
    }

    public void playBgSound()
    {
        audioSources[0].Play();
    }

    public void stopBgSound()
    {
        audioSources[0].Stop();
    }

    public void playTouchSound(int idx)
    {
        audioSources[1].clip = touchClips[idx];
        audioSources[1].loop = false;
        audioSources[1].Play();
    }

    public void playCookieSound(int idx)
    {
        audioSources[1].clip = cookieClips[idx];
        audioSources[1].loop = false;
        audioSources[1].Play();
    }

    public void stopCookieSound()
    {
        audioSources[1].Stop();
    }

    public void playProcSound(int idx)
    {
        if (idx > procEftClips.Length - 1)
            idx = 2;
        audioSources[2].clip = procEftClips[idx];
        audioSources[2].loop = false;
        audioSources[2].Play();
    }

    public void playResultSound(int idx)
    {
        audioSources[2].clip = resultEftClips[idx];
        audioSources[2].loop = false;
        audioSources[2].Play();
    }
}
