using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideManager : Singleton<GuideManager>
{
    public CircleGuide circleGuide;
    public RectGuide rectGuide;
    public RectTransform imgHand;
    [HideInInspector]
    public bool status = false;

    [SerializeField] Canvas canvas;
    private Queue<GuideDb> queue = new Queue<GuideDb>();

    public Canvas Canvas => canvas;

    public void Add(RectTransform target, Camera camera)
    {
        queue.Enqueue(new GuideDb(target, camera));
    }
    public void Add(RectTransform target)
    {
        queue.Enqueue(new GuideDb(target));
    }
    public void Add(GuideDb db)
    {
        queue.Enqueue(db);
    }
    public void Clear()
    {
        queue.Clear();
    }
    //public void Stop
    public void Play()
    {
        status = false;
        circleGuide.gameObject.SetActive(false);
        rectGuide.gameObject.SetActive(false);
        imgHand.gameObject.SetActive(false);
        if (queue.Count == 0) return;
        GuideDb db = queue.Dequeue();
        if (db.shape == "circle")
        {
            circleGuide.imgHand = imgHand;
            circleGuide.gameObject.SetActive(true);
            circleGuide.camera = db.camera;
            circleGuide.SetTarget(db.target);
        }
        else
        {
            rectGuide.imgHand = imgHand;
            rectGuide.camera = db.camera;
            rectGuide.gameObject.SetActive(true);
            rectGuide.SetTarget(db.target);
        }
        //设置手
        imgHand.gameObject.SetActive(true);
        if (db.camera)
        {
            imgHand.position = db.camera.WorldToScreenPoint(db.target.position);
        }
        else
        {
            imgHand.position = db.target.position;
        }


        status = true;
    }

    public void OnBtnClick()
    {
        Play();
    }
}

public class GuideDb
{
    public GuideDb(RectTransform target, Camera camera = null, string tips = "", string shape = "rect")
    {
        this.target = target;
        this.camera = camera;
        this.shape = shape;
        this.tips = tips;
    }
    public RectTransform target;
    public string shape = "rect";
    public string tips = "";
    public Camera camera = null;
    //public GuideHandDb hand;

}
public class GuideHandDb
{

    public bool has = true;
    public string ani = "click";//click点击 drag拖拽 TODO:目前的方式展示不支持拖拽
    public Vector3 position;
    //public List<Vector3> line;
}