using System;
using UnityEngine;
public enum SlideVector
{
    None,
    Up,
    Down,
    Left,
    Right
};
public class SlidingDirectionTest : Singleton<SlidingDirectionTest>
{


    [SerializeField] private bool executeMultipleTimes = false; //是否支持多次执行
    [SerializeField] private float offsetTime = 0.1f; //判断的时间间隔 
    [SerializeField] private float slidingDistance = 80f; //滑动的最小距离

    private Vector2 touchFirst = Vector2.zero; //手指开始按下的位置

    private Vector2 touchSecond = Vector2.zero; //手指拖动的位置

    private SlideVector currentVector = SlideVector.None; //当前滑动方向

    private float timer; //时间计数器  

    public event Action<SlideVector> OnSlideEvnet;

    void OnGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            touchFirst = Event.current.mousePosition;
        }

        if (Event.current.type == EventType.MouseDrag)
        {
            touchSecond = Event.current.mousePosition;

            timer += Time.deltaTime;

            if (timer > offsetTime)
            {
                touchSecond = Event.current.mousePosition;
                Vector2 slideDirection = touchFirst - touchSecond;
                float x = slideDirection.x;
                float y = slideDirection.y;

                if (y + slidingDistance < x && y > -x - slidingDistance)
                {
                    if (!executeMultipleTimes && currentVector == SlideVector.Left)
                    {
                        return;
                    }

                    Debug.Log("left");
                    OnSlideEvnet?.Invoke(SlideVector.Left);
                    currentVector = SlideVector.Left;
                }
                else if (y > x + slidingDistance && y < -x - slidingDistance)
                {
                    if (!executeMultipleTimes && currentVector == SlideVector.Right)
                    {
                        // todo 看你是否需要多次执行 return是每次滑动仅执行一次
                        return;
                    }

                    Debug.Log("right");
                    OnSlideEvnet?.Invoke(SlideVector.Right);

                    currentVector = SlideVector.Right;
                }
                else if (y > x + slidingDistance && y - slidingDistance > -x)
                {
                    if (!executeMultipleTimes && currentVector == SlideVector.Up)
                    {
                        return;
                    }

                    Debug.Log("up");
                    OnSlideEvnet?.Invoke(SlideVector.Up);

                    currentVector = SlideVector.Up;
                }
                else if (y + slidingDistance < x && y < -x - slidingDistance)
                {
                    if (!executeMultipleTimes && currentVector == SlideVector.Down)
                    {
                        return;
                    }

                    Debug.Log("Down");
                    OnSlideEvnet?.Invoke(SlideVector.Down);

                    currentVector = SlideVector.Down;
                }

                timer = 0;
                touchFirst = touchSecond;
            }
        }

        if (Event.current.type == EventType.MouseUp)
        {
            currentVector = SlideVector.None; //初始化方向  
        }
    }
}

