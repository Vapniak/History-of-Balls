namespace GameEntitySystem;

public interface IGameEntityComponent<T> where T : IGameEntityData {
  public T GetData();
}
