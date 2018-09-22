using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioArea : MonoBehaviour {

    public AudioClip song;

    [SerializeField]
    AudioSource source;

    [SerializeField]
    private float duckingTime = 1.5f;

    private float currentVolume;

    [SerializeField]
    SettingsMenu settingsMenu;

    bool raise;
    bool lower;
    float timer = 0f;

    // Use this for initialization
    void Start () {
		if(source == null)
        {
            source = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        }

        if(settingsMenu == null)
        {
            settingsMenu = GameObject.Find("Menu System/OptionsMenu").GetComponent<SettingsMenu>();
        }
        currentVolume = source.volume;
    }
	
	// Update is called once per frame
	void Update () {
        
        if (lower)
        {
            timer += Time.deltaTime;
            source.volume = currentVolume - (currentVolume * (timer/duckingTime));
            if(timer >= duckingTime)
            {
                lower = false;
                timer = 0f;
            }
        }
        if (raise)
        {
            timer += Time.deltaTime;
            source.volume = currentVolume * (timer / duckingTime);
            if (timer >= duckingTime)
            {
                raise = false;
                timer = 0f;
            }
        }
        if (settingsMenu.isActiveAndEnabled)
        {
            currentVolume = settingsMenu.musicFloat;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Character" && source.clip != song)
        {
            StartCoroutine(AudioChange());
        }
    }

    IEnumerator AudioChange()
    {
        AudioDuck();
        yield return new WaitForSeconds(duckingTime);
        source.clip = song;
        source.Play();
        source.loop = true;
        AudioRaise();
    }
    void AudioDuck()
    {
        lower = true;
    }

    void AudioRaise()
    {
        raise = true;
    }
}
