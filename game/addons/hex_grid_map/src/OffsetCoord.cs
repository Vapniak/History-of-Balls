namespace HexGridMap;

using Godot;

public enum Offset {
  Even = 1,
  Odd = -1
}

public enum OffsetType {
  ROffset,
  QOffset
}

public struct OffsetCoord {
  public int Col { get; private set; }
  public int Row { get; private set; }

  public OffsetCoord(int col, int row) {
    Col = col;
    Row = row;
  }


  public readonly CubeCoord QoffsetToCube(Offset offset) {
    var q = Col;
    var r = Row - ((Col + ((int)offset * (Col & 1))) / 2);
    return new CubeCoord(q, r);
  }

  public readonly CubeCoord RoffsetToCube(Offset offset) {
    var q = Col - ((Row + ((int)offset * (Row & 1))) / 2);
    var r = Row;

    return new CubeCoord(q, r);
  }

  public override readonly string ToString() {
    return "Col: " + Col + " Row: " + Row;
  }
}
