namespace GameplayFramework;

using Godot;


// TODO: find better way to make nodes controllable
public interface IPlayerControllable {
  public T GetCharacter<T>() where T : Node;
}
