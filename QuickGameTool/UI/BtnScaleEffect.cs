using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BtnScaleEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Header("按下时缩小至多少？"), Range(0, 2)]
    private float _downScale = 1.2f;
    [SerializeField, Header("缩放变化持续时间：按下过程")]
    private float _downDuration = 0.06f;
    [SerializeField, Header("缩放变化持续时间：抬起过程")]
    private float _upDuration = 0.15f;

    private RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    private RectTransform _rectTransform;

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeScaleCoroutine(1, _downScale, _downDuration));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeScaleCoroutine(RectTransform.localScale.x, 1, _upDuration));
    }

    private IEnumerator ChangeScaleCoroutine(float beginScale, float endScale, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            RectTransform.localScale = Vector3.one * Mathf.Lerp(beginScale, endScale, timer / duration);
            timer += Time.fixedDeltaTime;
            yield return null;
        }
        RectTransform.localScale = Vector3.one * endScale;
    }

    private void OnDisable()
    {
        RectTransform.localScale = Vector3.one;
    }
}