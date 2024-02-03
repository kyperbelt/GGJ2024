using Godot;
using System;
using System.Diagnostics;
using GGJ2024.Util;
using Debug = System.Diagnostics.Debug;

public partial class GlobalAudio : Node
{
    [Export]
    private AudioStreamPlayer _buttonClick;

    public static GlobalAudio Instance { get; private set; }
    public override void _EnterTree()
    {
        base._EnterTree();
        Debug.Assert(Instance == null, "Expected GlobalAudio instance to be null when entering tree, there may be more than one instance of singleton.");
        Instance = this;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Instance = null;
    }

    public void ButtonClick()
    {
        _buttonClick.Play();
    }
}
