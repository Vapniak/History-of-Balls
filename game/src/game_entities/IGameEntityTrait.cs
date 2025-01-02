namespace HOB.Entity;

public interface IGameEntityTrait<T> where T : IGameTraitData {
  T GetData();
}
