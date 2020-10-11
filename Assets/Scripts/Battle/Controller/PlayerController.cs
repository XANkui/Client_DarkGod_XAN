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
    public GameObject daggerskill1fx;
    public GameObject daggerskill2fx;
    public GameObject daggerskill3fx;


    public GameObject daggeratk1fx;
    public GameObject daggeratk2fx;
    public GameObject daggeratk3fx;
    public GameObject daggeratk4fx;
    public GameObject daggeratk5fx;

   

   
   
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

#if UNITY_EDITOR  // 编辑状态有效
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 _dir = new Vector2(h, v).normalized;
        if (_dir != Vector2.zero)
        {
            Dir = _dir;
            SetBlend(Constants.BlendMove);
        }
        else {
            Dir = Vector2.zero;
            SetBlend(Constants.BlendIdle);
        }
#endif
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
        
        if (daggerskill1fx != null)
        {
            fxDic.Add(daggerskill1fx.name, daggerskill1fx);

        }

        if (daggerskill2fx != null)
        {
            fxDic.Add(daggerskill2fx.name, daggerskill2fx);

        }

        if (daggerskill3fx != null)
        {
            fxDic.Add(daggerskill3fx.name, daggerskill3fx);

        }

        if (daggeratk1fx != null)
        {
            fxDic.Add(daggeratk1fx.name, daggeratk1fx);

        }

        if (daggeratk2fx != null)
        {
            fxDic.Add(daggeratk2fx.name, daggeratk2fx);

        }

        if (daggeratk3fx != null)
        {
            fxDic.Add(daggeratk3fx.name, daggeratk3fx);

        }

        if (daggeratk4fx != null)
        {
            fxDic.Add(daggeratk4fx.name, daggeratk4fx);

        }
        if (daggeratk5fx != null)
        {
            fxDic.Add(daggeratk5fx.name, daggeratk5fx);

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
