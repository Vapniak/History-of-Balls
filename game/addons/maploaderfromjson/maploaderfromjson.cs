#if TOOLS
using Godot;
using System;
using System.IO;
using Godot.Collections;
using HOB;

[Tool]
public partial class maploaderfromjson : EditorPlugin
{
    private VBoxContainer _panel;
    private OptionButton _cellTypeSelector;
    private SpinBox _moveCostEditor;
    private Button _generateButton;
    private Button _updateButton;
    private LineEdit _jsonPathField;
    private LineEdit _resourcePathField;
    private MapData _mapData;
    private string _currentResourcePath;

    public override void _EnterTree()
    {
        GD.Print("Plugin loaded.");
        _panel = CreatePanel();
        AddControlToContainer(CustomControlContainer.SpatialEditorSideRight, _panel);
    }

    public override void _ExitTree()
    {
      if (_generateButton != null && _generateButton.IsConnected("pressed", Callable.From(GenerateMap)))
      {
        _generateButton.Disconnect("pressed", Callable.From(GenerateMap));
      }
      if (_updateButton != null && _updateButton.IsConnected("pressed", Callable.From(UpdateMoveCost)))
      {
        _updateButton.Disconnect("pressed", Callable.From(UpdateMoveCost));
      }

      RemoveControlFromContainer(CustomControlContainer.SpatialEditorSideRight, _panel);
      _panel?.QueueFree();
    }

    private VBoxContainer CreatePanel()
    {
        var panel = new VBoxContainer();
        panel.AddChild(new Label { Text = "Map Generator from JSON" });

        _jsonPathField = CreateTextField("Path to JSON file:");
        _resourcePathField = CreateTextField("Path to save resource:");

        _generateButton = new Button { Text = "Generate Map" };
        _generateButton.Pressed += GenerateMap;
        panel.AddChild(_jsonPathField);
        panel.AddChild(_resourcePathField);
        panel.AddChild(_generateButton);

        panel.AddChild(new Label { Text = "Move Cost Editor" });
        _cellTypeSelector = new OptionButton();
        _moveCostEditor = new SpinBox { MinValue = 1, MaxValue = 10, Step = 1 };
        _updateButton = new Button { Text = "Save Move Cost" };
        _updateButton.Pressed += UpdateMoveCost;

        panel.AddChild(_cellTypeSelector);
        panel.AddChild(_moveCostEditor);
        panel.AddChild(_updateButton);

        return panel;
    }

    private static LineEdit CreateTextField(string labelText) => new() { PlaceholderText = labelText, FocusMode = Control.FocusModeEnum.All };

    private void GenerateMap()
    {
        var jsonPath = _jsonPathField.Text;
        var resourcePath = _resourcePathField.Text;
        _currentResourcePath = resourcePath;

        if (!File.Exists(jsonPath))
        {
            GD.PrintErr($"JSON file does not exist: {jsonPath}");
            return;
        }

        try
        {
            var jsonContent = File.ReadAllText(jsonPath);
            var json = new Json();
            if (json.Parse(jsonContent) != Error.Ok) {
              return;
            }

            var data = json.Data.AsGodotDictionary();
            if (!data.ContainsKey("title") || !data.ContainsKey("cols") || !data.ContainsKey("rows")) {
              return;
            }

            _mapData = new MapData
            {
                Title = data["title"].AsString(),
                Description = data["description"].AsString(),
                Cols = data["cols"].AsInt32(),
                Rows = data["rows"].AsInt32(),
                Settings = new MapSettings { CellSettings = new Godot.Collections.Array<CellSetting>() }
            };

            var cellSettingsDict = new Dictionary<int, CellSetting>();
            foreach (var item in data["cellDefinitions"].AsGodotArray())
            {
                var cellDef = item.AsGodotDictionary();
                var cellSetting = new CellSetting
                {
                    Name = cellDef["name"].AsString(),
                    Color = Color.FromHtml(cellDef["color"].AsString()),
                    MoveCost = 1
                };
                _mapData.Settings.CellSettings.Add(cellSetting);
                cellSettingsDict[cellDef["id"].AsInt32()] = cellSetting;
                _cellTypeSelector.AddItem(cellSetting.Name);
            }

            SaveMap();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error generating map: {ex.Message}");
        }
    }

    private void UpdateMoveCost()
    {
        if (_mapData == null) {
          return;
        }

        var selectedIndex = _cellTypeSelector.Selected;
        if (selectedIndex < 0 || selectedIndex >= _mapData.Settings.CellSettings.Count) {
          return;
        }

        _mapData.Settings.CellSettings[selectedIndex].MoveCost = (int)_moveCostEditor.Value;
        GD.Print($"Changed move cost for {_mapData.Settings.CellSettings[selectedIndex].Name} to {_moveCostEditor.Value}");

        PrintAllCells();
        SaveMap();
    }

    private void SaveMap()
    {
        if (string.IsNullOrEmpty(_currentResourcePath)) {
          return;
        }

        if (ResourceSaver.Save(_mapData, _currentResourcePath) == Error.Ok)
        {
            GD.Print($"Map updated at: {_currentResourcePath}");
        }
        else
        {
            GD.PrintErr($"Failed to update map at: {_currentResourcePath}");
        }
    }

    private void PrintAllCells()
    {
        GD.Print("Current cell move costs:");
        foreach (var cellSetting in _mapData.Settings.CellSettings)
        {
            GD.Print($"{cellSetting.Name}: {cellSetting.MoveCost}");
        }
    }
}
#endif
