/****************************************************
文件：LoopDragonFly.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/23 22:19:44
功能：飞龙循环飞行
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopDragonFly : MonoBehaviour
{
    private Animation ani;

    private void Awake()
    {
        ani = GetComponent<Animation>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (ani != null)
        {
            // 间隔20s循环播放一次
            InvokeRepeating("PlayDragonAni",0,20);
        }
    }

    void PlayDragonAni() {
        if (ani != null)
        {
            ani.Play();
        }
    }
}
