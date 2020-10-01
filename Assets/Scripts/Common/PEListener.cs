/****************************************************
文件：PEListener.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/27 15:33:44
功能：UI上的一些监听事件
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PEListener : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler,IPointerClickHandler
{
    public Action<PointerEventData> onclickDown; 
    public Action<PointerEventData> onclickUp; 
    public Action<PointerEventData> onDrag; 
    public Action<object> onClick;

    public object args;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onclickDown != null)
        {
            onclickDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onclickUp != null)
        {
            onclickUp(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null)
        {
            onDrag(eventData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick(args);
        }
    }
}
