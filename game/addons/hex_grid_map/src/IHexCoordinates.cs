namespace HexGridMap;

using System.Numerics;

public interface IHexCoordinates<T> where T : struct, INumber<T> {
  T Q { get; }
  T R { get; }
  T S { get; }

  T Length();
  T Distance(IHexCoordinates<T> other);

  IHexCoordinates<T> Add(IHexCoordinates<T> other);
  IHexCoordinates<T> Substract(IHexCoordinates<T> other);
  IHexCoordinates<T> Multiply(T scalar);
}
