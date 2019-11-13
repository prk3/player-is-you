using System.Collections.Generic;
using UnityEngine;

/**
 * Utility (and behaviour) for playing sounds in a game. Plays background audio track in loop.
 */
public class AudioPlayer : MonoBehaviour
{
    private static HashSet<string> _playQueue;
    private static Dictionary<string, AudioClip> _sounds;
    private static AudioSource _source;

    private void Awake()
    {
        if (_sounds == null)
        {
            // we load all clips at the start to avoid lag
            _sounds = new Dictionary<string, AudioClip>
            {
                {"background", Resources.Load<AudioClip>("Sounds/cyba_yellow_short")},
                {"select",     Resources.Load<AudioClip>("Sounds/select")},
                {"pause",      Resources.Load<AudioClip>("Sounds/pause")},
                {"win",        Resources.Load<AudioClip>("Sounds/win")},
                {"lose",       Resources.Load<AudioClip>("Sounds/lose")},
                {"push",       Resources.Load<AudioClip>("Sounds/push")},
                {"sink",       Resources.Load<AudioClip>("Sounds/sink")},
            };
        }
        if (_playQueue == null)
        {
            _playQueue = new HashSet<string>();
        }
    }

    void Start()
    {
        if (_source == null)
        {
            _source = gameObject.AddComponent<AudioSource>();
            _source.loop = true;
            _source.clip = _sounds["background"];
            _source.Play();
        }
    }

    void Update()
    {
        if (_playQueue.Count > 0)
        {
            foreach (var sound in _playQueue)
            {
                if (_sounds.ContainsKey(sound))
                {
                    _source.PlayOneShot(_sounds[sound]);
                }
            }

            _playQueue = new HashSet<string>();
        }
    }

    /**
     * Adds a play request to the queue. Multiple requests of the same sound are squashed into one request.
     */
    public static void PlaySound(string name)
    {
        _playQueue.Add(name);
    }
}

