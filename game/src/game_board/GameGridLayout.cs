namespace HOB;

using Godot;
using HexGridMap;

[GlobalClass]
public partial class GameGridLayout : HexLayout {
  [ExportGroup("Terrain Settings")]
  [Export(PropertyHint.Range, "0, 1")] public float SolidFactor { get; private set; } = 0.8f;
  [Export] public int TerracesPerSlope { get; private set; } = 2;
  [Export] public float ElevationStep { get; private set; } = 0.1f;
  [Export] public uint FlatMaxElevationDelta = 2;
  [Export] public uint SlopeMaxElevationDelta = 5;
  [Export] public float WaterFactor { get; private set; } = 0.6f;
  [Export] public float WaterLevel { get; private set; } = 0.1f;
  public float BlendFactor => 1f - SolidFactor;
  public int TerraceSteps => (TerracesPerSlope * 2) + 1;
  public float HorizontalTerraceStepSize => 1f / TerraceSteps;
  public float VerticalTerraceStepSize => 1f / (TerracesPerSlope + 1);
  public float WaterBlendFactor => 1f - WaterFactor;
}
