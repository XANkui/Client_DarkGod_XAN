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

public enum TxtColor {
    Red,
    Green,
    Blue,
    Yellow
}

public class Constants 
{
    // 颜色定义
    private const string ColorRed = "<color=#FF0000FF>";
    private const string ColorGreen = "<color=#00FF000FF>";
    private const string ColorBlue = "<color=#00B4FFFF>";
    private const string ColorYellow = "<color=#FFFF00FF>";
    private const string ColorEnd = "</color>";

    public static string Color(string str, TxtColor c) {
        string result = "";
        switch (c)
        {
            case TxtColor.Red:
                result = ColorRed + str + ColorEnd;
                break;
            case TxtColor.Green:
                result = ColorGreen + str + ColorEnd;
                break;
            case TxtColor.Blue:
                result = ColorBlue + str + ColorEnd;
                break;
            case TxtColor.Yellow:
                result = ColorYellow + str + ColorEnd;
                break;
            default:
                break;
        }

        return result;
    }

    //AutoGuideNPC
    public const int NPCWiseMan = 0;
    public const int NPCGeneral = 1;
    public const int NPCArtisan = 2;
    public const int NPCTrader = 3;

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
    public const string UIOpenPage = "uiOpenPage";
    public const string FBItemEnter = "fbitem";

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
