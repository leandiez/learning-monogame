using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
namespace MyFirstGameLibrary.Audio;

public class AudioController : IDisposable
{
    // Tracks sound effect instances created so they can be paused, unpaused, and/or disposed.
    private readonly List<SoundEffectInstance> _activeSoundEffectInstances;

    // Tracks the volume for song o SFX playback when muting and unmuting.
    private float _previousSongVolume;
    private float _previousSoundEffectVolume;
    
    // Global Mute
    public bool IsMuted { get; private set; }

    public float SongVolume
    {
        get
        {
            if(IsMuted)
            {
                return 0.0f;
            }
            return MediaPlayer.Volume;
        }
        set
        {
            if(IsMuted)
            {
                return;
            }
            MediaPlayer.Volume = Math.Clamp(value, 0.0f, 1.0f);
        }
    }

    public float SoundEffectVolume
    {
        get
        {
            if(IsMuted)
            {
                return 0.0f;
            }
            return SoundEffect.MasterVolume;
        }
        set
        {
            if(IsMuted)
            {
                return;
            }
            SoundEffect.MasterVolume = Math.Clamp(value, 0.0f, 1.0f);
        }
    }    
    /// Gets a value that indicates if this audio controller has been disposed.
    public bool IsDisposed {get; private set; }

    public AudioController()
    {
        _activeSoundEffectInstances = new List<SoundEffectInstance>();
    }

    // Finalizer called when object is collected by the garbage collector.
    ~AudioController() => Dispose(false);
    
    /// <summary>
    /// Disposes of this audio controller and cleans up resources from another call. IE the finalizer
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes this audio controller and cleans up resources.
    /// </summary>
    /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
    protected void Dispose(bool disposing)
    {
        if(IsDisposed)
        {
            return;
        }
        if (disposing)
        {
            foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
            {
                soundEffectInstance.Dispose();
            }
            _activeSoundEffectInstances.Clear();
        }
        IsDisposed = true;
    }
    /// <summary>
    /// Dispose unnused SoundEffect Instances every frame
    /// </summary>
    public void Update()
    {
        for (int i = _activeSoundEffectInstances.Count - 1; i >= 0; i--)
        {
            SoundEffectInstance instance = _activeSoundEffectInstances[i];

            if (instance.State == SoundState.Stopped)
            {
                if (!instance.IsDisposed)
                {
                    instance.Dispose();
                }
                _activeSoundEffectInstances.RemoveAt(i);
            }
        }
    }
    public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect)
    {
        return PlaySoundEffect(soundEffect, 1.0f, 1.0f, 0.0f, false);
    }

    public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect, float volume, float pitch, float pan, bool isLooped)
    {
        // Create an instance from the sound effect given.
        SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

        // Apply the volume, pitch, pan, and loop values specified.
        soundEffectInstance.Volume = volume;
        soundEffectInstance.Pitch = pitch;
        soundEffectInstance.Pan = pan;
        soundEffectInstance.IsLooped = isLooped;

        // Tell the instance to play
        soundEffectInstance.Play();

        // Add it to the active instances for tracking
        _activeSoundEffectInstances.Add(soundEffectInstance);

        return soundEffectInstance;
    }

    public void PlaySong(Song song, bool isRepeating = true)
    {
        // Check if the media player is already playing, if so, stop it.
        // If we do not stop it, this could cause issues on some platforms
        if (MediaPlayer.State == MediaState.Playing)
        {
            MediaPlayer.Stop();
        }

        MediaPlayer.Play(song);
        MediaPlayer.IsRepeating = isRepeating;
    }
    /// <summary>
    /// Pauses all audio.
    /// </summary>
    public void PauseAudio()
    {
        // Pause any active songs playing.
        MediaPlayer.Pause();

        // Pause any active sound effects.
        foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Pause();
        }
    }

    /// <summary>
    /// Resumes play of all previous paused audio.
    /// </summary>
    public void ResumeAudio()
    {
        // Resume paused music
        MediaPlayer.Resume();

        // Resume any active sound effects.
        foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Resume();
        }
    }

    /// <summary>
    /// Mutes all audio.
    /// </summary>
    public void MuteAudio()
    {
        // Store the volume so they can be restored during ResumeAudio
        _previousSongVolume = MediaPlayer.Volume;
        _previousSoundEffectVolume = SoundEffect.MasterVolume;

        // Set all volumes to 0
        MediaPlayer.Volume = 0.0f;
        SoundEffect.MasterVolume = 0.0f;

        IsMuted = true;
    }

    /// <summary>
    /// Unmutes all audio to the volume level prior to muting.
    /// </summary>
    public void UnmuteAudio()
    {
        // Restore the previous volume values.
        MediaPlayer.Volume = _previousSongVolume;
        SoundEffect.MasterVolume = _previousSoundEffectVolume;

        IsMuted = false;
    }

    /// <summary>
    /// Toggles the current audio mute state.
    /// </summary>
    public void ToggleMute()
    {
        if (IsMuted)
        {
            UnmuteAudio();
        }
        else
        {
            MuteAudio();
        }
    }
    
}