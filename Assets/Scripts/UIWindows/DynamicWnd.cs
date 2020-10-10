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
    public Transform hpItemRoot;

    protected override void InitWnd()
    {
        base.InitWnd();

        // 隐藏消息提示
        SetActive(txtTips,false);
    }

    #region Tips

    private Queue<string> tipsQue = new Queue<string>();
    private bool IsTipsShow = false;

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
    #endregion

    #region Hp Item

    private Dictionary<string, ItemEntityHp> hpItemDic = new Dictionary<string, ItemEntityHp>();

    public void AddHpItemInfo(string name, Transform trans, int hp) {
        ItemEntityHp item = null;
        if (hpItemDic.TryGetValue(name, out item) == true)
        {
            return;
        }
        else {
            GameObject go = resSvc.LoadPrefab(PathDefine.HpItemPrefab,true);
            go.transform.SetParent(hpItemRoot);
            // 先放到UI外面
            go.transform.localPosition = new Vector3(-1000,0,0);
            ItemEntityHp ieh = go.GetComponent<ItemEntityHp>();
            ieh.InitItemInfo(trans,hp);
            hpItemDic.Add(name, ieh);
        }
    }

    public void RmvHpItemInfo(string name)
    {
        ItemEntityHp item = null;
        if (hpItemDic.TryGetValue(name, out item) == true)
        {
            Destroy(item.gameObject);
            hpItemDic.Remove(name);
            return;
        }
        
    }

    public void SetCiritical(string key, int critical)
    {
        ItemEntityHp item = null;
        if (hpItemDic.TryGetValue(key,out item)==true)
        {
            item.SetCiritical(critical);
        }
    }

    public void SetDodge(string key)
    {
        ItemEntityHp item = null;
        if (hpItemDic.TryGetValue(key, out item) == true)
        {
            item.SetDodge();
        }
    }

    public void SetHurt(string key, int hurt)
    {
        ItemEntityHp item = null;
        if (hpItemDic.TryGetValue(key, out item) == true)
        {
            item.SetHurt(hurt);
        }
    }

    public void SetHpVal(string key, int oldVal, int newVal)
    {
        ItemEntityHp item = null;
        if (hpItemDic.TryGetValue(key, out item) == true)
        {
            item.SetHpVal(oldVal, newVal);
        }
    }

    #endregion





}
