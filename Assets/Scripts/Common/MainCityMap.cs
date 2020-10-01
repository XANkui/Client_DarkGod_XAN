/****************************************************
文件：MainCityMap.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/29 17:12:42
功能：主城的地图
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCityMap : MonoBehaviour
{
    public Transform[] NpcPosTrans;
    private void Start()
    {
        Debug.Log(GetType() + "/Start()/ NpcPosTrans.Length : " + NpcPosTrans.Length);
    }
}
