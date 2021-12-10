using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField]
	public Sound[] sounds;



    private static AudioManager instance;
    public static AudioManager Instance
    {
                get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load<AudioManager>("AudioManager"));
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }



	void Awake()
	{
        if (instance != null && instance != this)
        {
            Debug.Log("dest!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
            s.source.volume = s.volume;
		}
	}

	public static void Play(string sound)
	{
		Sound s = Array.Find(instance.sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.Log("Sound Clip not found");
			return;
		}
		s.source.Play();
	}

}
