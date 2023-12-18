using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBodyBySlider : Singleton<MoveBodyBySlider>
{
    public RectTransform rightBot;
    public RectTransform leftBot;
    Vector2 edge=new Vector2();

    public Action OnMoveEndEvent;

    public bool Lock = false;
    protected override void Awake()
    {
        base.Awake();
        edge.x = leftBot.GetWorldCenter().x+0.2f;
        edge.y = rightBot.GetWorldCenter().x-0.2f;
    }
    void Update()
    {
        if (Lock) return;
        if (Input.touchCount > 0)
        {
            if(Input.GetTouch(0).phase== TouchPhase.Began )
            {

            }
            else if ( Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                //得到手指在这一帧的移动距离
                Vector2 touchDeltaPosition = Input.GetTouch(0).position;
                //在X 轴上旋转物体
                // GameObject.Find("zhuan/Kazuko").transform.Rotate(0, touchDeltaPosition.x, 0);
                var worldPos= PositionConvert.ScreenPointToWorldPoint(touchDeltaPosition, 0);
                //touchDeltaPosition= Camera.main.ScreenToWorldPoint(touchDeltaPosition);
                transform.position = new Vector3(CheckEdge(worldPos.x), transform.position.y, 0);

            }else if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                OnMoveEndEvent?.Invoke();
            }
        }

    }
    float CheckEdge(float x)
    {
        if(x< edge.x)return edge.x;
        if(x> edge.y)return edge.y;
        return x;
    }
}
#region
//保持UTF-8
#endregion

