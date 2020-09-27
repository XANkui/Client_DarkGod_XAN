/****************************************************
文件：WindowRoot.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/23 21:08:51
功能：UI 窗口基类
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowRoot : MonoBehaviour
{
    protected ResSvc resSvc = null;
    protected AudioSvc audioSvc = null;
    protected NetSvc netSvc = null;

    public void SetWndState(bool isActive = true) {
        if (gameObject.activeSelf != isActive)
        {
            gameObject.SetActive(isActive);
        }
        if (isActive == true)
        {
            InitWnd();
        }
        else {
            ClearWnd();
        }
    }

    protected virtual void InitWnd() {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
    }

    protected virtual void ClearWnd() {
        resSvc = null;
        audioSvc = null;
        netSvc = null;
    }

    #region Tool Function

    protected void SetActive(GameObject go, bool isActive = true) {
        go.SetActive(isActive);
    }
    protected void SetActive(Transform trans, bool isActive = true)
    {
        trans.gameObject.SetActive(isActive);
    }

    protected void SetActive(RectTransform rexctTrans, bool isActive = true)
    {
        rexctTrans.gameObject.SetActive(isActive);
    }

    protected void SetActive(Image img, bool isActive = true)
    {
        img.gameObject.SetActive(isActive);
    }

    protected void SetActive(Text txt, bool isActive = true)
    {
        txt.gameObject.SetActive(isActive);
    }

    protected void SetText(Text txt,string context ="") {
        txt.text = context;
    }

    protected void SetText(Text txt, float num=0)
    {
        txt.text = num.ToString();
    }

    protected void SetText(Transform trans, string context = "")
    {
        trans.GetComponent<Text>().text = context;
    }

    protected void SetText(Transform trans, float num = 0)
    {
        trans.GetComponent<Text>().text = num.ToString();
    }

    protected T GetOrAddComponent<T>(GameObject go) where T : Component {
        T t = go.GetComponent<T>();
        if (t ==null)
        {
            t = go.AddComponent<T>();
        }

        return t;
    }

    #endregion

    #region Click Evts

    protected void OnClickDown(GameObject go,Action<PointerEventData> cb) {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onclickDown = cb;
    }

    protected void OnClickUp(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onclickUp = cb;
    }

    protected void OnDrag(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onDrag = cb;
    }

    #endregion
}
