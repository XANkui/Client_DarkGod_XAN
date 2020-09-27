/****************************************************
文件：Constance.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/23 09:48:22
功能：一些常量
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants 
{
    // 场景名称
    public const string SceneLogin = "SceneLogin";
    public const string SceneMainCity = "SceneMainCity";
    public const int MainCityID = 10000;

    // 音频音效名称
    public const string BGLogin = "bgLogin";
    public const string BGMainCity = "bgMainCity";
    
    // 登录按钮音效
    public const string UILoginBtn = "uiLoginBtn";
    public const string UIExtenBtn = "uiExtenBtn";

    // UI点击音效
    public const string UIClickBtn = "uiClickBtn";

    // 游戏开发的参考的标准宽高
    public const int ScreenStandardWidth = 1334;
    public const int ScreenStandardHeight = 750;

    // 摇杆点的限制距离
    public const int ScreenOPDis = 90;

    // 角色移动速度
    public const int PlayerMoveSpeed = 8;
    public const int MonsterMoveSpeed = 4;

    // 动画平滑
    public const int BlendIdle = 0;
    public const int BlendWalk = 1;
    public const float AccelerSpeed = 5.0f;
}
