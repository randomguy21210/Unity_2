using UnityEngine.Audio;
using System;
using UnityEngine;
using TMPro;
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    public Sound[] music;
    public Sound[] effects;
    public float BGM_Vol;
    private float SFX_Vol;

    void Awake()
    {
        // this front part ensures that
        // 1. music does not restart when you switch scenes
        // 2. multiple audio managers are not created between scenes
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Initialize volume to max
        if (BGM_Vol == 0)
            BGM_Vol = 1f;

        if (music != null)
        {
            foreach (Sound s in music)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.loop = s.loop;
                s.source.playOnAwake = false;
                s.source.volume = s.volume * BGM_Vol;
                Debug.Log("AudioManager Awake: registered music '" + s.name + "' clip=" + (s.clip != null) + " baseVol=" + s.volume + " BGM_Vol=" + BGM_Vol);
            }
        }

        if (effects != null)
        {
            foreach (Sound s in effects)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.loop = s.loop;
                s.source.playOnAwake = false;
                s.source.volume = s.volume;
                Debug.Log("AudioManager Awake: registered effect '" + s.name + "' clip=" + (s.clip != null) + " baseVol=" + s.volume);
            }
        }
    }

    public void Play(string sound)
    {
        Debug.Log("AudioManager: Play('" + sound + "')");

        Sound s = null;
        string foundIn = "none";

        if (music != null)
        {
            s = Array.Find(music, item => item.name == sound);
            if (s != null) foundIn = "music";
        }

        if (s == null && effects != null)
        {
            s = Array.Find(effects, item => item.name == sound);
            if (s != null) foundIn = "effects";
        }

        if (s == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }

        if (s.source == null)
        {
            Debug.LogWarning("Sound '" + sound + "' has no AudioSource (source was null)");
            return;
        }

        if (s.clip == null)
        {
            Debug.LogWarning("Sound '" + sound + "' has no AudioClip assigned (clip is null)");
            return;
        }

        float varianceFactor = 1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f);
        Debug.Log("AudioManager: compute vol: baseVol=" + s.volume + " BGM_Vol=" + BGM_Vol + " varianceFactor=" + varianceFactor);
        if (foundIn == "music")
            s.source.volume = s.volume * BGM_Vol * varianceFactor;
        else
            s.source.volume = s.volume * varianceFactor;

        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        Debug.Log("AudioManager: Playing '" + sound + "' (" + foundIn + ") clip=" + (s.clip != null) + " vol=" + s.source.volume);
        s.source.Play();
    }
    void Start()
        {
        Play("Spring");
        }
    public void changeBGM_Vol(float newVol)
{
    BGM_Vol = newVol;

    foreach (Sound s in music)
    {
        s.source.volume = s.volume * BGM_Vol;
    }

    }
}


public class MusicValue : MonoBehaviour
{
   private TMP_Text textDisplay;
   public GameObject audioM;
   private int volume;
   void Start()
   {
       textDisplay = GetComponent<TMP_Text>();
   }
   void Update()
   {
       volume = (int)(audioM.GetComponent<AudioManager>().BGM_Vol*100);
       textDisplay.text = volume.ToString() + "%";
   }
}
