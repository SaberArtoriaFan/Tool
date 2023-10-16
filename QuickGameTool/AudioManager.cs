using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class  AudioClipName
{
    string[] strings;

    public AudioClipName(string[] strings)
    {
        this.strings = strings;
    }

    public string[] Strings { get => strings; set => strings = value; }
}

public class AudioManager : Singleton<AudioManager>
{
    //音频管理器 存储所有的音频并且可以播放和停止
    [Serializable]
    public class Sound
    {
        [Header("音频剪辑")]
        private AudioClip clip;

        [Header("音频分组")]
        private AudioMixerGroup outputGroup;

        [Header("音频音量")]
        [Range(0, 1)]
        private float volume;

        [Header("音频是否自启动")]
        private bool playOnAwake;

        [Header("音频是否要循环播放")]
        private bool loop;

        public Sound(AudioClip clip, AudioMixerGroup outputGroup, float volume, bool playOnAwake, bool loop)
        {
            this.clip = clip;
            this.outputGroup = outputGroup;
            this.volume = volume;
            this.playOnAwake = playOnAwake;
            this.loop = loop;
        }

        public AudioClip Clip { get => clip; set => clip = value; }
        public AudioMixerGroup OutputGroup { get => outputGroup; set => outputGroup = value; }
        public float Volume { get => volume; set => volume = value; }
        public bool PlayOnAwake { get => playOnAwake; set => playOnAwake = value; }
        public bool Loop { get => loop; set => loop = value; }
    }

    public string audioResourcePath = "\\Resources\\AudioClip";

    //public List<Sound> sounds;//存储所有音频的信息
//
    //private Dictionary<string, AudioSource> audioDic;//每一个音频的名称组件

    private Dictionary<string, Sound> soundDic=new Dictionary<string, Sound>();

    AudioSource audioSource;

    event Action OnAfterPlayAudio;

    string audioResourceName;
    Timer timer;

    [SerializeField]
    TextAsset textAsset;
    private void OnDestroy()
    {
        OnAfterPlayAudio = null;
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) { audioSource = gameObject.AddComponent<AudioSource>(); }
        //audioDic = new Dictionary<string, AudioSource>();
        // Debug.Log("路径" + Application.dataPath + audioResourcePath);
        //audioResourceName = $"{Application.persistentDataPath}/{Application.productName}_AudioName";
        audioResourceName = $"{Application.dataPath}{audioResourcePath}\\AudioNames";


#if UNITY_EDITOR
        DirectoryInfo root = new DirectoryInfo(Application.dataPath + audioResourcePath);
        FileInfo[] filesInfo = root.GetFiles();
        List<string> fileName = new List<string>();
        foreach (var v in filesInfo)
        {
            if (v.Name.EndsWith(".wav"))
            {
                string s = v.Name.Remove(v.Name.Length - 4);
                //Debug.Log(s);
                fileName.Add(s);
                var clip = AssestLoad.Load<AudioClip>($"AudioClip\\{s}");

                soundDic.Add(clip.name, new Sound(clip, null, 1, false, false));
                //Debug.Log(clip.name);
            }

        }
        JsonUtil.Saver(audioResourceName, new AudioClipName(fileName.ToArray()));
        //var sa = Resources.Load("AudioClip/AudioNames.json");


#else

          var text = AssestLoad.Load<TextAsset>("AudioClip/AudioNames");
        //Debug.Log(sa.name);
        AudioClipName audioClipName = JsonConvert.DeserializeObject<AudioClipName>(text.text);

        if(audioClipName == null )
        {
            audioClipName = JsonConvert.DeserializeObject<AudioClipName>(textAsset.text);
        }
        foreach (var v in audioClipName.Strings)
        {



            var clip = AssestLoad.Load<AudioClip>($"AudioClip\\{v}");

            soundDic.Add(clip.name, new Sound(clip, null, 1, false, false));
            //Debug.Log(clip.name);

        }
 

  
#endif
    }


    //播放某个音频的方法 iswait为是否等待
    public static void PlayAudio(string name, bool iswait = false)
    {
        if (!instance.soundDic.ContainsKey(name))
        {
            //不存在次音频
            Debug.LogError("不存在" + name + "音频");
            return;
        }
        if (iswait)
        {
            if (!instance.audioSource.isPlaying)
            {
                //如果是等待的情况 不在播放
                instance.RealPlay(instance.soundDic[name], false);
            }
            else
            {
                instance.OnAfterPlayAudio +=()=> instance.RealPlay(instance.soundDic[name], true);

            }
        }
        else
        {
            instance.RealPlay(instance.soundDic[name], true);
        }
    }
    void RealPlay(Sound audioClip,bool isCover)
    {
        //if (audioSource.isPlaying)
        //{
        //    if (isCover)
        //    {
        //        audioSource.Stop();
        //        timer?.Stop();
        //        timer = null;
        //    }
        //    else
        //        return;
        //}
        audioSource.PlayOneShot(audioClip.Clip);

        //audioSource.clip = audioClip.Clip;
        //audioSource.volume = audioClip.Volume;
        //audioSource.playOnAwake = false;
        //audioSource.loop= audioClip.Loop;
        //audioSource.outputAudioMixerGroup= audioClip.OutputGroup;
        //timer= TimerManager.instance.AddTimer(() =>
        //{
        //    if (audioSource.isPlaying)
        //        audioSource.Stop();
        //    OnAfterPlayAudio?.Invoke();
        //    OnAfterPlayAudio = null;
        //    timer = null;
        //},audioClip.Clip.length);
    }

    //停止音频的播放
    public static void StopMute(string name)
    {

        if (!instance.soundDic.ContainsKey(name))
        {
            //不存在次音频
            Debug.LogError("不存在" + name + "音频");
            return;
        }
        else
        {
            instance.audioSource.Stop();
            instance.OnAfterPlayAudio = null;
            instance.timer?.Stop();
            instance.timer = null;

        }
    }
}

