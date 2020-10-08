/****************************************************
文件：PlayerController.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/27 16:25:19
功能：表现实体角色控制器
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    public GameObject daggeratk1fx;

   

    private Transform camTrans;
   
    private Vector3 camOffset;
    private float targetBlend;
    private float currentBlend;

    

    // Start is called before the first frame update
    void Start()
    {
       // Init();
    }


    // Update is called once per frame
    void Update()
    {

        #region 输入方向
        /*

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 _dir = new Vector2(h, v).normalized;
        if (_dir != Vector2.zero)
        {
            Dir = _dir;
            SetBlend(Constants.BlendWalk);
        }
        else {
            Dir = Vector2.zero;
            SetBlend(Constants.BlendIdle);
        }
        */
        #endregion

        if (currentBlend != targetBlend)
        {
            UpdateMixBlend();
        }

        if (isMove == true)
        {
            //设置方向
            SetDir();

            // 移动
            SetMove();

            // 相机跟随
            SetCamera();
        }

        // 技能移动
        if (skillMove==true )
        {
            SetSkillMove();

            SetCamera();
        }
        
    }

    public override void Init() {
        base.Init();
        camTrans = Camera.main.transform;
        camOffset = transform.position - camTrans.position;
        if (daggeratk1fx != null)
        {
            fxDic.Add(daggeratk1fx.name, daggeratk1fx);

        }
    }

    private void SetDir() {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1))+camTrans.localEulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
    private void SetMove() {
        ctrl.Move(transform.forward * Time.deltaTime * Constants.PlayerMoveSpeed);
    }
    public void SetCamera() {
        if (camTrans !=null)
        {
            camTrans.position = transform.position - camOffset;
        }
    }

    public override void SetBlend(float blend) {
        targetBlend = blend;
    }

    private void UpdateMixBlend() {
        if (Mathf.Abs(currentBlend - targetBlend) < Constants.AccelerSpeed * Time.deltaTime)
        {
            currentBlend = targetBlend;
        }
        else if (currentBlend > targetBlend)
        {
            currentBlend -= Constants.AccelerSpeed * Time.deltaTime;
        }
        else {
            currentBlend += Constants.AccelerSpeed * Time.deltaTime;
        }

        ani.SetFloat("Blend",currentBlend);
    }


    //////////////////////////////////////////////////////////

    public override void SetFX(string fxName, float delayDestory)
    {
        GameObject go = null;
        if (fxDic.TryGetValue(fxName, out go))
        {
            go.SetActive(true);
            timerSvc.AddTimeTask((int tid)=>
            {
                go.SetActive(false);
            },delayDestory);
        }
    }

    public void SetSkillMove() {
        ctrl.Move(transform.forward * Time.deltaTime *skillMoveSpeed);
    }
}
