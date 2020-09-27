/****************************************************
文件：DynamicWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/23 22:30:11
功能：动态UI界面
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWnd : WindowRoot
{
    public Animation tipsAni;
    public Text txtTips;

    protected override void InitWnd()
    {
        base.InitWnd();

        // 隐藏消息提示
        SetActive(txtTips,false);
    }

    public void AddTips(string tips) {
        lock (tipsQue) {
            tipsQue.Enqueue(tips);
        }
    }

    private void Update()
    {
        if (tipsQue.Count>0 && IsTipsShow ==false)
        {
            lock (tipsQue) {
                string tips = tipsQue.Dequeue();
                IsTipsShow = true;
                SetTips(tips);
            }
        }
    }

    private void SetTips(string tips) {
        SetActive(txtTips);
        SetText(txtTips, tips);

        AnimationClip clip = tipsAni.GetClip("TipsShowAni");
        tipsAni.Play();

        // 播放完关闭显示
        StartCoroutine(AniPlayDone(clip.length,()=> {
            SetActive(txtTips,false);
            IsTipsShow = false;
        }));
    }

    private IEnumerator AniPlayDone(float sec, Action cb) {
        yield return new WaitForSeconds(sec);

        if (cb != null)
        {
            cb();
        }
    }

    private Queue<string> tipsQue = new Queue<string>();
    private bool IsTipsShow = false;
}
