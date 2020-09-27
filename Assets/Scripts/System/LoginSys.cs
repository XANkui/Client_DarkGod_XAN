/****************************************************
文件：LoginSys.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/22 22:33:21
功能：登录业务系统
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSys : SystemRoot
{
    public static LoginSys Instance = null;
    public LoginWnd loginWnd;
    public CreateWnd createWnd;

    public override void InitSys() {
        base.InitSys();
        Debug.Log(GetType() + "/InitSys()/Init LoginSys...");

        Instance = this;
    }

    /// <summary>
    /// 进入登录场景
    /// </summary>
    public void EnterLogin() {
        Debug.Log(GetType() + "/EnterLogin()/Loading ...");


        //异步加载登录场景
        //并显示进度条
        //加载结束，打开登录界面
        resSvc.AsyncLoadScene(Constants.SceneLogin,()=> {

            // 加载完成以后打开登陆界面            
            loginWnd.SetWndState();

            // 播放背景音乐
            audioSvc.PlayBGAudio(Constants.BGLogin);

        });
        
    }

    /// <summary>
    /// 网络登录响应
    /// </summary>
    public void RspLogin(GameMsg msg) {
        GameRoot.AddTips("登陆成功");

        // 缓存角色信息
        GameRoot.Instance.SetPlayerData(msg.rspLogin);

        // 打开角色创建界面(通过名字是否为空来判断是新建账号，还是老账号)
        if (msg.rspLogin.playerData.name.Equals(""))
        {
            // 角色创建界面
            createWnd.SetWndState(true);
        }
        else {

            // 进入主城
            MainCitySys.Instance.EnterMainCity();
        }
        
        // 关闭登陆界面
        loginWnd.SetWndState(false);
    }


    public void RspRename(GameMsg msg) {
        GameRoot.Instance.SetPlayerName(msg.rspRename.name);

        
        // 跳转场景进入主城
        // 打开主城界面
        MainCitySys.Instance.EnterMainCity();

        // 关闭常见界面
        createWnd.SetWndState(false);
    }
}
