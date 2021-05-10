using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler I;
    public List<AudioSource> musicTracks = new List<AudioSource>();
    protected AudioSource currentTrack;

    private void Awake()
    {
        AudioHandler.I = this;
    }

    private void Start()
    {
        StartRegister();
    }

    private void StartRegister()
    {
        PlayMusic();
        HostHandler.I.onSsh.AddListener(OnSsh);
    }

    private void OnSsh(SshKey arg0)
    {
        PlayMusic();
    }

    public void PlayMusic() {
        if (currentTrack != null)
            currentTrack.Stop();
        currentTrack = GetCurrentTrack();
        currentTrack.Play();
    }

    public AudioSource GetCurrentTrack() {

        if (currentTrack == null)
            return musicTracks[0];
        int index = musicTracks.IndexOf(currentTrack) +1;
        if (index >= musicTracks.Count)
            index = 0;
        return musicTracks[index];
    }
}
