
//#define USE_TESTCONSOLE
using System.Collections.Generic;
using UnityEngine;

namespace Consolation
{
    /// <summary>
    /// 一个在游戏中显示Unity调试日志的控制台。
    /// </summary>
    class TestConsole : MonoBehaviour
    {
        //#if USE_TESTCONSOLE
        struct Log
        {
            public string message;
            public string stackTrace;
            public LogType type;
        }

        #region Inspector Settings

        /// <summary>
        /// 显示和隐藏控制台窗口的热键。
        /// </summary>
        public KeyCode toggleKey = KeyCode.BackQuote;

        /// <summary>
        /// Whether to open the window by shaking the device (mobile-only).
        /// 是否通过摇动设备来打开窗口（仅限移动设备）。
        /// </summary>
        public bool shakeToOpen = true;

        /// <summary>
        /// 窗口应打开的（平方）加速度。
        /// </summary>
        public float shakeAcceleration = 3f;

        /// <summary>
        /// Whether to only keep a certain number of logs.
        ///是否仅保留一定数量的日志。
        /// Setting this can be helpful if memory usage is a concern.
        /// 如果担心内存使用情况，则设置此选项将很有帮助。
        /// </summary>
        public bool restrictLogCount = false;

        /// <summary>
        /// Number of logs to keep before removing old ones.
        /// 删除旧日志之前要保留的日志数。
        /// </summary>
        public int maxLogs = 1000;

        #endregion

        readonly List<Log> logs = new List<Log>();
        Vector2 scrollPosition;
        bool visible;
        bool collapse;

        // Visual elements:视觉元素：

        static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
        {
            { LogType.Assert, Color.white },
            { LogType.Error, Color.red },
            { LogType.Exception, Color.red },
            { LogType.Log, Color.white },
            { LogType.Warning, Color.yellow },
        };

        const string windowTitle = "Console";
        const int margin = 20;
        static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
        static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

        readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);
        Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));

        [System.Obsolete]
        void OnEnable()
        {
#if UNITY_5
			Application.logMessageReceived += HandleLog;
#else
            Application.RegisterLogCallback(HandleLog);
#endif
        }

        [System.Obsolete]
        void OnDisable()
        {
#if UNITY_5
			Application.logMessageReceived -= HandleLog;
#else
            Application.RegisterLogCallback(null);
#endif
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                visible = !visible;
            }

            if (shakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration)
            {
                visible = !visible;
            }
        }

        void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            windowRect = GUILayout.Window(123456, windowRect, DrawConsoleWindow, windowTitle);
        }

        /// <summary>
        /// Displays a window that lists the recorded logs.
        /// 显示列出已记录日志的窗口。
        /// </summary>
        /// <param name="windowID">Window ID.</param>
        void DrawConsoleWindow(int windowID)
        {
            DrawLogsList();
            DrawToolbar();

            // Allow the window to be dragged by its title bar.
            GUI.DragWindow(titleBarRect);
        }

        /// <summary>
        /// Displays a scrollable list of logs.
        /// 显示日志的滚动列表。
        /// </summary>
        void DrawLogsList()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            // Iterate through the recorded logs.
            //遍历记录的日志。
            for (var i = 0; i < logs.Count; i++)
            {
                var log = logs[i];

                // Combine identical messages if collapse option is chosen.
                //如果选择了折叠选项，则合并相同的消息。
                if (collapse && i > 0)
                {
                    var previousMessage = logs[i - 1].message;

                    if (log.message == previousMessage)
                    {
                        continue;
                    }
                }

                GUI.contentColor = logTypeColors[log.type];
                GUILayout.Label(log.message);
            }

            GUILayout.EndScrollView();

            // Ensure GUI colour is reset before drawing other components.
            //在绘制其他组件之前，请确保已重置GUI颜色。
            GUI.contentColor = Color.white;
        }

        /// <summary>
        /// Displays options for filtering and changing the logs list.
        /// 显示用于过滤和更改日志列表的选项。
        /// </summary>
        void DrawToolbar()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(clearLabel))
            {
                logs.Clear();
            }

            collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Records a log from the log callback.
        /// 从日志回调中记录日志。
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="stackTrace">Trace of where the message came from.</param>
        /// <param name="type">Type of message (error, exception, warning, assert).</param>
        void HandleLog(string message, string stackTrace, LogType type)
        {
            logs.Add(new Log
            {
                message = message,
                stackTrace = stackTrace,
                type = type,
            });

            TrimExcessLogs();
        }

        /// <summary>
        /// Removes old logs that exceed the maximum number allowed.
        /// 删除超过允许的最大数量的旧日志。
        /// </summary>
        void TrimExcessLogs()
        {
            if (!restrictLogCount)
            {
                return;
            }

            var amountToRemove = Mathf.Max(logs.Count - maxLogs, 0);

            if (amountToRemove == 0)
            {
                return;
            }

            logs.RemoveRange(0, amountToRemove);
        }
        //#endif
    }
}