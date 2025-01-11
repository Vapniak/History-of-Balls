namespace HexGridMap;


public enum Offset {
  Even = 1,
  Odd = -1
}

public enum OffsetType {
  ROffset,
  QOffset
}

public struct HexOffsetCoordinates {
  public int Col { get; private set; }
  public int Row { get; private set; }

  public HexOffsetCoordinates(int col, int row) {
    Col = col;
    Row = row;
  }

  public static HexOffsetCoordinates QoffsetFromCube(Offset offset, HexCoordinates h) {
    var col = h.Q;
    var row = h.R + ((h.Q + ((int)offset * (h.Q & 1))) / 2);
    return new HexOffsetCoordinates(col, row);
  }


  public readonly HexCoordinates QoffsetToCube(Offset offset) {
    var q = Col;
    var r = Row - ((Col + ((int)offset * (Col & 1))) / 2);
    return new HexCoordinates(q, r);
  }


  public static HexOffsetCoordinates RoffsetFromCube(Offset offset, HexCoordinates h) {
    var col = h.Q + ((h.R + ((int)offset * (h.R & 1))) / 2);
    var row = h.R;


    return new HexOffsetCoordinates(col, row);
  }


  public readonly HexCoordinates RoffsetToCube(Offset offset) {
    var q = Col - ((Row + ((int)offset * (Row & 1))) / 2);
    var r = Row;

    return new HexCoordinates(q, r);
  }
}
