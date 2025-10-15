using Godot;
using System;
using Direction = (Godot.Vector2I, Godot.Vector2);

public enum CharacterDirection
{
    Side,
    Up,
    Down
}

public enum CharacterState
{
    Idle,
    Walk,
    Run
}

public partial class Player : CharacterBody2D
{
    [Export]
    public float Speed { get; set; }
    public Vector2I Direction { get; set; } = Vector2I.Zero;
    private CharacterState State { get; set; }
    private AnimationHandler _anim { get; set; }

    public override void _Ready()
    {
        _anim = new AnimationHandler(GetChild<AnimatedSprite2D>(0));
    }

    public override void _PhysicsProcess(double delta)
    {
        //in future we'll check for run input
        Direction = PlayerInputHandler.DetermineDirection();
        if (Direction != Vector2I.Zero) HandleMovement();
        else
        {
            State = CharacterState.Idle;
        }

        _anim.PlayMovementAnimation(Direction, State);
        //update character direction
        //update character animation
        //move character
    }

    public void HandleMovement()
    {
        D_TopDown.Utilities.Logger.VarPrint(Velocity);
        State = CharacterState.Walk;
        ChangeVelocity();
        MoveAndSlide();
    }
    
    public void ChangeVelocity()
    {
        if (Direction == Vector2I.Up)
            Velocity = new Vector2(0, -Speed);
        else if (Direction == Vector2I.Down)
            Velocity = new Vector2(0, Speed);
        else if (Direction == Vector2I.Right)
            Velocity = new Vector2(Speed, 0);
        else if (Direction == Vector2I.Left)
            Velocity = new Vector2(-Speed, 0);
    }
}

public class AnimationHandler
{
    private AnimatedSprite2D Sprite { get; }
    private CharacterState State { get; set; } = CharacterState.Idle;
    private CharacterDirection Direction { get; set; } = CharacterDirection.Down;
    private string AnimationName => $"{State}{Direction}";

    public AnimationHandler(AnimatedSprite2D sprite)
    {
        Sprite = sprite;
        Sprite.Play(AnimationName);
    }

    public void PlayMovementAnimation(Vector2I spriteDirection, CharacterState state)
    {
        State = state;

        if (spriteDirection == Vector2I.Up)
        {
            Direction = CharacterDirection.Up;
            Sprite.FlipH = false;
        }
        else if (spriteDirection == Vector2I.Down)
        {
            Direction = CharacterDirection.Down;
            Sprite.FlipH = false;
        }
        else if (spriteDirection == Vector2I.Right)
        {
            Direction = CharacterDirection.Side;
            Sprite.FlipH = false;
        }
        else if(spriteDirection == Vector2I.Left)
        {
            Direction = CharacterDirection.Side;
            Sprite.FlipH = true;        
        }

        Sprite.Play(AnimationName);
    }

}

public static class PlayerInputHandler
{
    private static string Up = "ui_up";
    private static string Down = "ui_down";
    private static string Left = "ui_left";
    private static string Right = "ui_right";

    public static bool IsMovementInput() => DetermineDirection() != Vector2I.Zero;

    public static Vector2I DetermineDirection()
    {
        if (Input.IsActionPressed(Up))
            return Vector2I.Up;
        else if (Input.IsActionPressed(Down))
            return Vector2I.Down;
        else if (Input.IsActionPressed(Right))
            return Vector2I.Right;
        else if (Input.IsActionPressed(Left))
            return Vector2I.Left;
        return Vector2I.Zero;
    }
}
