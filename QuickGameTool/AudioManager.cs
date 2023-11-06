using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class AudioClipName
{
    string[] strings;

    public AudioClipName(string[] strings)
    {
        this.strings = strings;
    }

    public string[] Strings { get => strings; set => strings = value; }
}
public class AudioManager : AutoSingleton<AudioManager>
{
    public string audioResourcePath = "\\Resources\\AudioClip";
    string audioResourceName;
    
    public bool isUseAudioResource = true;
    [SerializeField] TextAsset textAsset;

    private Dictionary<string, AudioClip> _DicAudio; //音频库(字典)
    private List<AudioSource> effectAudioSources = new List<AudioSource>();//音频源
    private AudioSource musicAudioSource;

    //音效静音
    private bool _effectMuted = false;
    [HideInInspector]
    public bool EffectMuted
    {
        get
        {
            return _effectMuted;
        }
        set
        {
            _effectMuted = value;
            if (!value)
            {
                StopAllEffect();
            }
        }
    }
    //背景音乐静音
    private bool _musicMuted = false;
    [HideInInspector]
    public bool MusicMuted
    {
        get
        {
            return _musicMuted;
        }
        set
        {
            _musicMuted = value;
            if (value)
            {
                StopMusic();
            }
            else
            {
                if (musicAudioSource.clip != null) musicAudioSource.Play();
            }
        }
    }

    //[Header("VOL")]
    //[Range(0, 1)]
    //public float volumeOfBGM;
    //[Range(0, 1)]
    //public float volumeOfEffect;

    //[Header("VOL Slider")]
    private float _Volume = 1f;
    public float Volume
    {
        get
        {
            return _Volume;
        }
        set
        {
            _Volume = value;
            musicAudioSource.volume = value;
            for (int i = 0; i < effectAudioSources.Count; i++)
                effectAudioSources[i].volume = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _DicAudio = new Dictionary<string, AudioClip>();
        //指定背景音乐的音频源
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        for (int i = 0; i < 5; i++)
        {
            effectAudioSources.Add(gameObject.AddComponent<AudioSource>());
        }
    }
    private void Start()
    {
        if (isUseAudioResource)
        {
            audioResourceName = $"{Application.dataPath}{audioResourcePath}\\AudioNames";
            var text = AssestLoad.Load<TextAsset>("AudioClip\\AudioNames");
            if (text == null)
            {
                Debug.LogError($"未在{audioResourceName}路径下找到该文件，请将音频文件放至{Application.dataPath}{audioResourcePath}路径下,并点击[Test/BuildClipNames]自动生成配置文件");
            }
            //Debug.Log(sa.name);
            AudioClipName audioClipName = text != null ? JsonConvert.DeserializeObject<AudioClipName>(text.text) : JsonConvert.DeserializeObject<AudioClipName>(textAsset.text);
            foreach (var v in audioClipName.Strings)
            {



                var clip = AssestLoad.Load<AudioClip>($"AudioClip\\{v}");

                _DicAudio.Add(clip.name, clip);
                //Debug.Log(clip.name);

            }
        }

    }
    //播放音效函数：
    public AudioSource PlayEffect(string acName, bool loop = false)
    {
        if (EffectMuted) return null;
        //当传进来的名字不为空且在音频库中
        if (_DicAudio.ContainsKey(acName) && !string.IsNullOrEmpty(acName))
        {
            AudioClip ac = _DicAudio[acName];
            return PlayEffect(ac, loop);
        }
        return null;
    }
    public AudioSource PlayEffect(AudioClip ac, bool loop = false)
    {
        if (EffectMuted) return null;
        if (!ac) return null;
        for (int i = 0; i < effectAudioSources.Count; i++)
        {
            if (effectAudioSources[i].isPlaying) continue;
            //当有音频源空闲时，则用其播放
            AudioSource _as = effectAudioSources[i];
            _as.loop = loop;
            _as.clip = ac;
            _as.volume = Volume;
            _as.Play();
            return _as;
        }
        //当没有多余的音频源空闲时，则创建新的音频源
        AudioSource newAs = gameObject.AddComponent<AudioSource>();
        newAs.loop = loop;
        newAs.clip = ac;
        newAs.volume = Volume;
        newAs.Play();
        effectAudioSources.Add(newAs);
        return newAs;
    }

    //播放BGM函数：
    public void PlayMusic(string acName)
    {
        if (MusicMuted) return;
        //当传进来的名字不为空且在音频库中
        if (_DicAudio.ContainsKey(acName) && !string.IsNullOrEmpty(acName))
        {
            AudioClip ac = _DicAudio[acName];
            PlayMusic(ac);
        }
    }
    //暂停音效
    public void StopEffect(string acName)
    {
        if (_DicAudio.ContainsKey(acName) && !string.IsNullOrEmpty(acName))
        {
            AudioClip ac = _DicAudio[acName];
            StopEffect(ac);
        }
    }
    public void StopEffect(AudioClip ac)
    {

        for (int i = 0; i < effectAudioSources.Count; i++)
        {
            if (effectAudioSources[i].isPlaying && effectAudioSources[i].clip == ac)
                effectAudioSources[i].Stop();
        }

    }
    //暂停所有音效
    public void StopAllEffect()
    {
        for (int i = 0; i < effectAudioSources.Count; i++)
        {
            effectAudioSources[i].Stop();
        }
    }
    //播放背景音乐
    public void PlayMusic(AudioClip ac)
    {
        if (!ac) return;

        musicAudioSource.clip = ac;
        musicAudioSource.loop = true;
        musicAudioSource.volume = Volume;
        musicAudioSource.Play();

    }

    //停止当前BGM的播放函数：
    public void StopMusic()
    {
        if (musicAudioSource != null && musicAudioSource.isPlaying)
            musicAudioSource.Stop();
    }
}
