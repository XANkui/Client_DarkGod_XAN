/****************************************************
文件：NetSvc.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/25 10:32:29
功能：网络服务
*****************************************************/

using Protocal;
using System.Collections.Generic;
using UnityEngine;

public class NetSvc : MonoBehaviour
{
    public static NetSvc Instance = null;
    private static readonly string obj = "lock";
    PENet.PESocket<ClientSession, GameMsg> client = null;
    private Queue<GameMsg> msgQue = new Queue<GameMsg>();

    public void InitSvc() {
        Instance = this;        

        client = new PENet.PESocket<ClientSession, GameMsg>();
        client.SetLog(true, (string msg, int lv) => {
            switch (lv)
            {
                case 0:
                    msg = "Log:" + msg;
                    Debug.Log(msg);
                    break;
                case 1:
                    msg = "Warn:" + msg;
                    Debug.LogWarning(msg);
                    break;
                case 2:
                    msg = "Error:" + msg;
                    Debug.LogError(msg);
                    break;
                case 3:
                    msg = "Info:" + msg;
                    Debug.Log(msg);
                    break;

                default:
                    break;
            }
        });
        client.StartAsClient(SrvCfg.srvIP, SrvCfg.srvPort);
        Common.Log(GetType() + "/InitSvc()/Init NetSvc...");

    }


    public void SendMsg(GameMsg msg) {
        if (client.session != null)
        {
            client.session.SendMsg(msg);
        }
        else {
            GameRoot.AddTips("服务器未连接");
            InitSvc();
        }
    }

    public void AddNetPackMsg(GameMsg msg) {
        lock (obj)
        {
            msgQue.Enqueue(msg);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (msgQue.Count>0)
        {
            lock (obj)
            {
                GameMsg msg = msgQue.Dequeue();
                ProcessMsg(msg);
            }
        }
    }

    /// <summary>
    /// 消息分发处理
    /// </summary>
    /// <param name="msg"></param>
    private void ProcessMsg(GameMsg msg) {

        // 错误码提示处理
        if (msg.err != (int)ErrorCode.None)
        {
            switch ((ErrorCode)msg.err)
            {
                case ErrorCode.UpdateDBError:
                    Common.Log("数据库更新异常",LogType.Error);
                    GameRoot.AddTips("网络不稳定");
                    break;
                case ErrorCode.AcctIsOnline:
                    GameRoot.AddTips("当前账号已被登陆");
                    break;
                case ErrorCode.WrongPassword:
                    GameRoot.AddTips("密码错误");
                    break;
                default:
                    break;
            }

            return;
        }

        // 消息分发处理
        switch ((CMD)msg.cmd)
        {
            
            case CMD.RspLogin:
                LoginSys.Instance.RspLogin(msg);
                break;

            case CMD.RspRename:
                LoginSys.Instance.RspRename(msg);
                break;
            default:
                break;
        }
    }
}
