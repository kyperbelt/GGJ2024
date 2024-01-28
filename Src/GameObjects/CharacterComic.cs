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
                _characterData = value;
                CharacterFacingDirection = _characterFacing;
            }
            else
            {
                _characterData = value;
            }

        }
    }

    private Sprite2D _sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
	CharacterFacingDirection = _characterFacing;

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
