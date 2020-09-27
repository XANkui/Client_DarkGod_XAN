/****************************************************
文件：ClientSession.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/25 10:39:38
功能：ClientSession
*****************************************************/

using PENet;
using Protocal;
using UnityEngine;

public class ClientSession : PENet.PESession<GameMsg>
{
    protected override void OnConnected()
    {
        Common.Log(GetType() + "/OnConnected()/ Connect To Server");
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        Common.Log(GetType() + "/OnReciveMsg()/ Recive Servver Msg cmd: " +((CMD)msg.cmd).ToString());
        NetSvc.Instance.AddNetPackMsg(msg);
    }

    protected override void OnDisConnected()
    {
        Common.Log(GetType() + "/OnDisConnected()/ Disonnected To Server");
    }
}
