using Godot;
using System;
using GGJ2024.Util;

public partial class RandomAudioPlayer : AudioStreamPlayer
{
    [Export]
    private AudioStream[] _audioStreamVariations;

    public void PlayRandom()
    {
        Stream = _audioStreamVariations.RandomElement();
        Play();
    }
}
