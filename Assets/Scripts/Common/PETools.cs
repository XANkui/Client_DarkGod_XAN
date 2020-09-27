/****************************************************
文件：PETools.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/24 09:25:51
功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PETools 
{
    /// <summary>
    /// 随机整数生成
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="rd"></param>
    /// <returns></returns>
    public static int RDInt(int min,int max, System.Random rd = null) {
        if (rd == null)
        {
            rd = new System.Random();
        }
        int val = rd.Next(min,max+1);

        return val;
    }
}
