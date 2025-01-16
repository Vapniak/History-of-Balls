namespace HOB.GameEntity;


public interface IGetTraitData<T> where T : TraitData {
  public T Data { get; }
}
