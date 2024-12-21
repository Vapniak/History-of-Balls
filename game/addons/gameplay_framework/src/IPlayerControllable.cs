namespace GameplayFramework;

using Godot;


// TODO: find better way to make nodes controllable
public interface IPlayerControllable {
  public PlayerController PlayerController { get; set; }

  public T GetCharacter<T>() where T : Node;
}
