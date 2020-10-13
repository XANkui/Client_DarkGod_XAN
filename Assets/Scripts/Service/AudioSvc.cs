/****************************************************
文件：AudioSvc.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/23 21:30:12
功能：音乐音效播放服务
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSvc : MonoBehaviour
{
    public static AudioSvc Instance = null;

    public AudioSource bgAudio;
    public AudioSource uiAudio;

    public void InitSvc() {
        Debug.Log(GetType()+ "/InitSvc()/ Init Audio Service...");

        Instance = this;
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isLoop"></param>
    public void PlayBGAudio(string name, bool isLoop = true) {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/"+name,true);
        if (bgAudio.clip == null || bgAudio.clip.name != audio.name)
        {
            bgAudio.clip = audio;
            bgAudio.loop = isLoop;
            bgAudio.Play();
        }
    }

    public void StopBGAudio() {
        if (bgAudio !=null)
        {
            bgAudio.Stop();
        }
    }

    /// <summary>
    /// 播放UI音效
    /// </summary>
    /// <param name="name"></param>
    public void PlayUIAudio(string name) {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name,true);
        uiAudio.clip = audio;
        uiAudio.Play();
    }

    public void PlayCharAudio(string name,AudioSource audioSource)
    {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        audioSource.clip = audio;
        audioSource.Play();
    }
}
