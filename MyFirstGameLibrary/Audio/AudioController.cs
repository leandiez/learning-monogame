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
    
    /// <summary>
    /// Global Mute
    /// </summary>
    public bool IsMuted { get; private set; }

    /// <summary>
    /// Gets or Sets the global volume of songs.
    /// </summary>
    /// <remarks>
    /// If IsMuted is true, the getter will always return back 0.0f and the
    /// setter will ignore setting the volume.
    /// </remarks>
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

    /// <summary>
    /// Gets or Sets the global volume of sound effects.
    /// </summary>
    /// <remarks>
    /// If IsMuted is true, the getter will always return back 0.0f and the
    /// setter will ignore setting the volume.
    /// </remarks>
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
    /// <summary>
    /// Gets a value that indicates if this audio controller has been disposed.
    /// </summary>
    public bool IsDisposed {get; private set; }

    public AudioController()
    {
        _activeSoundEffectInstances = new List<SoundEffectInstance>();
    }

    // Finalizer called when object is collected by the garbage collector.
    ~AudioController() => Dispose(false);
    
    /// <summary>
    /// Disposes of this audio controller and cleans up resources.
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
    

}