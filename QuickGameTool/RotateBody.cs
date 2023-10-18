using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBody : MonoBehaviour
{
    [SerializeField]
    float speed = 0.1f;

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            //得到手指在这一帧的移动距离
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            //在X 轴上旋转物体
            // GameObject.Find("zhuan/Kazuko").transform.Rotate(0, touchDeltaPosition.x, 0);
            transform.Rotate(0, -touchDeltaPosition.x, 0);

        }
    }
}
#region
//保持UTF-8
#endregion

