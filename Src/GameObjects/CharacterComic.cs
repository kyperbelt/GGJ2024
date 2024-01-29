using System;
using Godot;

[GlobalClass]
[Tool]
public partial class CharacterComic : Area2D
{

    public enum CharacterFacing
    {
        Front,
        Back
    }

    private Callable _characterConfidenceCallable;
    Confidence _characterConfidenceUi;
    [Export] 
    public Confidence CharacterConfidenceUi
    {
        get => _characterConfidenceUi;
        set
        {
            _characterConfidenceUi = value;
            // GD.Print($"CharacterComic {this} Set CharacterConfidence:{value}");
            if (_characterData != null && _characterConfidenceUi != null) {
                _characterConfidenceUi.MaxHealth = _characterData.MaxConfidence;
                _characterConfidenceUi.SetCurrentHealth(_characterData.CurrentConfidence);
            }
        }
    }

    private CharacterFacing _characterFacing = CharacterFacing.Front;
    [Export]
    public CharacterFacing CharacterFacingDirection
    {
        get => _characterFacing;
        set
        {
            _characterFacing = value;
            if (CharacterData == null || _sprite == null)
            {
                return;
            }

            switch (_characterFacing)
            {
                case CharacterFacing.Front:
                    _sprite.Texture = CharacterData.FrontImage;
                    break;
                case CharacterFacing.Back:
                    _sprite.Texture = CharacterData.BackImage;
                    break;
            }
        }
    }

    private CharacterData _characterData;
    [Export]
    public CharacterData CharacterData
    {
        get => _characterData;
        set
        {
            if (value != null && value != _characterData)
            {
                // GD.Print($"CharacterComic {this} Set CharacterData:{value}");
                _characterData = value;
                CharacterFacingDirection = _characterFacing;
                if (_characterConfidenceUi != null) {
                    _characterConfidenceUi.MaxHealth = _characterData.MaxConfidence;
                    _characterConfidenceUi.SetCurrentHealth(_characterData.CurrentConfidence);
                }
            }
            else
            {
                _characterData = value;
            }

        }
    }

    public int CurrentConfidence
    {
        get => _characterData == null ? 0 : _characterData.CurrentConfidence;
        set
        {
            var oldConfidence = _characterData.CurrentConfidence;
            _characterData.CurrentConfidence = value;
            // Use value from CharacterData which has been properly clamped
            _characterConfidenceUi.OnHealthChanged(oldConfidence, _characterData.CurrentConfidence);
        }
    }
    
    [Export]
    private Shader _shader;
    private Sprite2D _sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        CharacterFacingDirection = _characterFacing;
        if (_characterData != null && _characterConfidenceUi != null) {
            _characterConfidenceUi.MaxHealth = _characterData.MaxConfidence;
            _characterConfidenceUi.SetCurrentHealth(_characterData.CurrentConfidence);
        }

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void HighLight(bool highlight)
    {
        ShaderMaterial material = _sprite.Material as ShaderMaterial;
        if (highlight)
        {
            material.Shader = _shader;
        }
        else
        {
            material.Shader = null;
        }
    }
}
