/****************************************************
文件：GameRoot.cs
作者：仙魁Xan
邮箱：1272200579@qq.com
日期：2020/09/22 22:29:37
功能：游戏的入口
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetSvc))]
[RequireComponent(typeof(ResSvc))]
[RequireComponent(typeof(AudioSvc))]
[RequireComponent(typeof(TimerSvc))]
[RequireComponent(typeof(LoginSys))]
[RequireComponent(typeof(MainCitySys))]
public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance = null;
    public LoadingWnd loadingWnd;
    public DynamicWnd dynamicWnd;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GetType() + "/Start()/Game Start...");
        Instance = this;
        // 过场不销毁
        DontDestroyOnLoad(this.gameObject);

        ClearWnd();

        Init();
    }

    /// <summary>
    /// 清理窗口
    /// </summary>
    private void ClearWnd() {

        Transform canvas = transform.Find("Canvas");

        // 隐藏所有窗口
        for (int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }

        // 显示动态窗口，并显示在最顶层
        dynamicWnd.SetWndState();
        dynamicWnd.transform.SetAsLastSibling();
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    private void Init() {

        // 服务模块初始化
        NetSvc netSvc = GetComponent<NetSvc>();
        netSvc.InitSvc();

        ResSvc res = GetComponent<ResSvc>();
        res.InitSvc();

        AudioSvc audioSvc = GetComponent<AudioSvc>();
        audioSvc.InitSvc();

        TimerSvc timerSvc = GetComponent<TimerSvc>();
        timerSvc.InitSvc();

        // 业务系统初始化
        LoginSys login = GetComponent<LoginSys>();
        login.InitSys();

        MainCitySys mainCity = GetComponent<MainCitySys>();
        mainCity.InitSys();

        // 进入登陆场景，并加载相应UI
        login.EnterLogin();

       
        
       
    }

    /// <summary>
    /// 消息提示
    /// </summary>
    /// <param name="tips"></param>
    public static void AddTips(string tips) {
        Instance.dynamicWnd.AddTips(tips);
    }


    private PlayerData playerData = null;
    public PlayerData PlayerData { get => playerData;  }
    public void SetPlayerData(RspLogin data) {
        playerData = data.playerData;
    }

    public void SetPlayerName(string name) {
        PlayerData.name = name;
    }

    public void SetPlayerDataByGuide(RspGuide data) {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
        PlayerData.guideid = data.guideid;
        
    }

    public void SetPlayerDataByStrong(RspStrong data)
    {
        PlayerData.coin = data.coin;
        PlayerData.crystal = data.crystal;
        PlayerData.hp = data.hp;
        PlayerData.ap = data.ap;
        PlayerData.addef = data.addef;
        PlayerData.apdef = data.apdef;

        PlayerData.strongArr = data.strongArr;

    }

    public void SetPlayerDataByBuy(RspBuy data)
    {
        PlayerData.coin = data.coin;
        PlayerData.diamond = data.diamond;
        PlayerData.power = data.power;

    }

    public void SetPlayerDataByPower(PshPower data)
    {
        PlayerData.power = data.power;
       

    }

    public void SetPlayerDataByTask(RspTakeTaskReward data)
    {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
        PlayerData.taskArr = data.taskArr;


    }

    public void SetPlayerDataByTaskPsh(PshTaskPrgs data)
    {
        
        PlayerData.taskArr = data.taskArr;


    }
}
