/****************************************************
文件：BaseData.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/27 17:26:18
功能：配置数据
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCfg : BaseData<MapCfg> {
    public string mapName;
    public string sceneName;
    public Vector3 mainCameraPos;
    public Vector3 mainCameraRot;
    public Vector3 playerBornPos;
    public Vector3 playerBornRot;
}

public class BaseData<T>
{
    public int ID;
}
