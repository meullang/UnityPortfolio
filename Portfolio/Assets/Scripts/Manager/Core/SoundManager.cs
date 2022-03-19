using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    AudioMixer mixer = null;

    public AudioMixer GetAudioMixer() { return mixer; }

    float pitch = 1.0f;

    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    //사운드 매니저를 root의 자식으로 생성 그리고 있는 곡의 정보들을 가져옴
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; ++i)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }

        mixer = Resources.Load<AudioMixer>("Sounds/AudioMixer");

        _audioSources[(int)Define.Sound.Bgm].outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[1];
        _audioSources[(int)Define.Sound.Effect].outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[2];
    }

    //오디오 소스에 있는 오디오클립을 비우고 정지함
    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    //이펙트를 재생시킴
    public void Play(string path, Define.Sound type = Define.Sound.Effect)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type);
    }


    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect)
    {
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void PlayAtPoint(GameObject Point, string path, bool isLoop = false, Define.Sound type = Define.Sound.Effect)
    {
        AudioClip _cilp = GetOrAddAudioClip(path, type);
        PlayAtPoint(Point, _cilp, isLoop, type);
    }

    public void PlayAtPoint(GameObject Point, AudioClip audioClip, bool isLoop = false, Define.Sound type = Define.Sound.Effect)
    {
        if (audioClip == null)
            return;

        AudioSource pointAudio = Point.GetOrAddComponent<AudioSource>();
        pointAudio.pitch = pitch;
        pointAudio.outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[2];

        if (isLoop)
        {
            pointAudio.clip = audioClip;
            pointAudio.loop = true;
            pointAudio.Play();
        }
        else
        {
            pointAudio.PlayOneShot(audioClip);
        }
    }

    public void ControlPitch(GameObject Point, float pitch)
    {
        AudioSource pointAudio = Point.GetOrAddComponent<AudioSource>();
        pointAudio.pitch = pitch;
    }

    public void StopAtPoint(GameObject Point)
    {
        AudioSource pointAudio = Point.GetOrAddComponent<AudioSource>();
        pointAudio.Stop();
    }

    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Define.Sound.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }

            if (audioClip == null)
                Debug.Log($"AudioClip Missing ! {path}");
        }

        if (audioClip == null)
            Debug.Log("AudioClip Missing ! {path}");

        return audioClip;
    }
}
