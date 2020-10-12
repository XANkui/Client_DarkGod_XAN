/****************************************************
文件：TriggerData.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/12 08:22:22
功能：怪物触发器
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerData : MonoBehaviour
{
    public int triggerMstWave;
    public MapMgr mapMgr;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (mapMgr!= null)
            {
                mapMgr.TriggerMonsterBorn(this,triggerMstWave);
            }
        }
    }
}
