namespace HOB.GameEntity;


public interface IGetTraitData<T> where T : TraitData {
  T Data { get; }
}
