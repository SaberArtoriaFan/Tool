using DG.Tweening;
using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Saber.Base
{
    public class RisingSpaceUI : AutoSingleton<RisingSpaceUI>
    {
        Saber.Base.ObjectPool<TMP_Text> textObjectPool;
        //public string packageName = "fight";
        public const string TextMeshProName = "RisingSpaceItem";
        GameObject model = null;
        Transform panel;
        const string panelName = "RisingSpaceItemPanelUI";
        protected override void Awake()
        {
            base.Awake();
            Canvas canvas = UIManager.Instance.CurrSceneMainCanvas;
            RectTransform[] rectTransforms = canvas.transform.GetComponentsInChildren<RectTransform>();
            for (int i = 0; i < rectTransforms.Length; i++)
            {
                //Debug.Log("name" + rectTransforms[i].gameObject.name);
                if (rectTransforms[i].gameObject.name == panelName)
                {
                    panel = rectTransforms[i].transform;
                    break;
                }
            }
            if (panel == null)
            {
                panel = new GameObject(panelName).transform;
                panel = panel.gameObject.AddComponent<RectTransform>();
                panel.SetParent(canvas.transform);
                panel.localPosition = Vector3.zero;
                panel.localRotation = Quaternion.identity;
                panel.localScale = Vector3.one;

                var rectTransform = panel as RectTransform;
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.offsetMax = new Vector2(0, 0);


            }
            //Debug.Log("GGG"+panel.name);
            textObjectPool = PoolManager.Instance.AddPool(Spawn, RecycleBefore, InitText, "RisingSpaceItemPool", 20);
        }
        private void InitText(TMP_Text obj)
        {
            obj.gameObject.SetActive(true);
            obj.transform.SetAsFirstSibling();
            obj.gameObject.SetActive(false);
            obj.transform.localScale = Vector3.one;
        }

        private void RecycleBefore(TMP_Text obj)
        {
            obj.gameObject.SetActive(false);
        }

        private TMP_Text Spawn()
        {
            if (model == null)
                model = AssestLoad.Load<GameObject>(TextMeshProName);
            GameObject go = GameObject.Instantiate(model);
            go.transform.SetParent(panel);
            TMP_Text textMeshPro = go.GetComponent<TMP_Text>();
            textMeshPro.transform.localScale = Vector3.one;
            go.SetActive(false);
            return textMeshPro;
        }

        public static Vector3 CalculateScreenPosition(
            Vector3 position,
            Camera camera,
            Canvas canvas,
            RectTransform transform)
        {
            var screenPos = camera.WorldToScreenPoint(position);

            var pos = Vector3.zero;
            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(
                        transform,
                        screenPos,
                        null,
                        out pos);
                    break;
                case RenderMode.ScreenSpaceCamera:
                case RenderMode.WorldSpace:
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(
                        transform,
                        screenPos,
                        canvas.worldCamera,
                        out pos);
                    break;
            }

            return pos;
        }
        //伤害文字显示
        public void ShowRisingSpace(string s, Vector3 worldPos, Vector2 dir, Color color = default, int size = 24, FontStyles fontStyles = FontStyles.Normal, float speed = 1, float continueTime = 1.5f)
        {

            TMP_Text textMeshPro = textObjectPool.GetObjectInPool();
            textMeshPro.text = s;
            if (color == default) color = Color.yellow;
            textMeshPro.color = color;
            textMeshPro.fontSize = size;
            textMeshPro.fontStyle = textMeshPro.fontStyle;

            //var localPos=Camera.main.WorldToScreenPoint(worldPos);
            var localPos = CalculateScreenPosition(worldPos, Camera.main, UIManager.Instance.CurrSceneMainCanvas, textMeshPro.rectTransform);
            textMeshPro.transform.position = localPos;
            //var rtPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
            //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(textMeshPro.rectTransform, rtPos, Camera.main, out var localPos))
            //    textMeshPro.rectTransform.localPosition = localPos;
            var endPos = new Vector2(localPos.x,localPos.y) + dir.normalized * Random.Range(50, 100);
            //showUi.ChangeText(textName, s);
            //float rand_x = Random.Range(-2, 2);
            //float rand_y = Random.Range(-2, 2);
            //Vector3 endPos = Camera.main.WorldToScreenPoint(worldPos + dir.normalized * speed + (rand_y * 0.1f * Vector3.up + rand_x * 0.1f * Vector3.right));
            //Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            //textMeshPro.transform.localPosition = screenPos;
            textMeshPro.gameObject.SetActive(true);
            //textMeshPro.transform.localRotation = Quaternion.Euler(0.1f, 0.1f, 0.1f);
            textMeshPro.transform.DOScale(3f, continueTime * 1 / 3).OnComplete(() => TimerManager.Instance.AddTimer(() => textMeshPro.transform.DOScale(1f, continueTime * 1 / 3).OnComplete(() => textObjectPool.RecycleToPool(textMeshPro)), continueTime * 1 / 3));
            textMeshPro.transform.DOMove(endPos, continueTime * 1 / 2);
            //TimerManager.Instance.AddTimer(() => { textObjectPool.RecycleToPool(textMeshPro); }, continueTime, false,
            //    () =>
            //    {
            //        timer += Time.deltaTime;
            //        per = timer / continueTime;
            //        curvePer = animationCurve.Evaluate(per);
            //        //Debug.Log("Cur" + curvePer);
            //        screenPos = Camera.main.WorldToScreenPoint(worldPos);
            //        screenPos += offest;
            //        offest += dir.normalized * speed * 60 * Time.deltaTime * curvePer;
            //        //var realPos = Vector3.zero;
            //        //switch (MainCanvas.renderMode)
            //        //{
            //        //    case RenderMode.ScreenSpaceOverlay:
            //        //        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            //        //            rectTransform,
            //        //            screenPos,
            //        //            null,
            //        //            out realPos);
            //        //        break;
            //        //    case RenderMode.ScreenSpaceCamera:
            //        //    case RenderMode.WorldSpace:
            //        //        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            //        //            rectTransform,
            //        //            screenPos,
            //        //            MainCanvas.worldCamera,
            //        //            out realPos);
            //        //        break;
            //        //}
            //        showUi.transform.position = screenPos;
            //    });
        }
    }
}