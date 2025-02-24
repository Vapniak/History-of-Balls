#if TOOLS
using Godot;
using System;
using System.IO;
using Godot.Collections;
using System.Linq;
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
    private LineEdit _mapDataPathField;

    private MapData _mapData;
    private string _currentMapDataPath;
    private const string DEFAULT_SETTINGS_PATH = "res://SharedMapSettings.tres";
    private MapSettings _sharedSettings;

    public override void _EnterTree()
    {
        GD.Print("Plugin loaded.");
        _panel = CreatePanel();
        AddControlToContainer(CustomControlContainer.SpatialEditorSideRight, _panel);

        _sharedSettings = ImportSharedSettings(DEFAULT_SETTINGS_PATH);
        if (_sharedSettings == null) {
          return;
        }

        _cellTypeSelector?.Clear();
        foreach (var cs in _sharedSettings.CellSettings) {
          _cellTypeSelector?.AddItem(cs.Name);
        }
    }

    public override void _ExitTree()
    {
        if (_generateButton != null && _generateButton.IsConnected("pressed", Callable.From(GenerateMap))) {
          _generateButton.Disconnect("pressed", Callable.From(GenerateMap));
        }

        if (_updateButton != null && _updateButton.IsConnected("pressed", Callable.From(UpdateMoveCost))) {
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
        _mapDataPathField = CreateTextField("Path to save MapData resource:");
        panel.AddChild(_jsonPathField);
        panel.AddChild(_mapDataPathField);

        _generateButton = new Button { Text = "Generate Map" };
        _generateButton.Pressed += GenerateMap;
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

    private static LineEdit CreateTextField(string labelText)
        => new() { PlaceholderText = labelText, FocusMode = Control.FocusModeEnum.All };

    private static MapSettings ImportSharedSettings(string settingsPath)
    {
        var sharedSettings = ResourceLoader.Load(settingsPath) as MapSettings;
        if (sharedSettings != null) {
          return sharedSettings;
        }

        GD.Print("Creating new MapSettings at: " + settingsPath);
        sharedSettings = new MapSettings { CellSettings = new Array<CellSetting>() };
        if (ResourceSaver.Save(sharedSettings, settingsPath) != Error.Ok) {
          GD.PrintErr("Failed to save new MapSettings to: " + settingsPath);
        }
        return sharedSettings;
    }

    private void GenerateMap()
    {
        var jsonPath = _jsonPathField.Text;
        _currentMapDataPath = _mapDataPathField.Text;

        if (!File.Exists(jsonPath))
        {
            GD.PrintErr($"JSON file does not exist: {jsonPath}");
            return;
        }

        try
        {
            var jsonContent = File.ReadAllText(jsonPath);
            var json = new Json();
            if (json.Parse(jsonContent) != Error.Ok)
            {
                GD.PrintErr("Error parsing JSON");
                return;
            }

            var data = json.Data.AsGodotDictionary();
            if (!data.ContainsKey("title") || !data.ContainsKey("cols") || !data.ContainsKey("rows"))
            {
                GD.PrintErr("JSON file missing required map properties.");
                return;
            }

            _mapData = new MapData
            {
                Title = data["title"].AsString(),
                Description = data["description"].AsString(),
                Cols = data["cols"].AsInt32(),
                Rows = data["rows"].AsInt32(),
                Settings = _sharedSettings
            };

            _cellTypeSelector.Clear();
            foreach (CellSetting cs in _sharedSettings.CellSettings) {
              _cellTypeSelector.AddItem(cs.Name);
            }

            foreach (var item in data["cellDefinitions"].AsGodotArray())
            {
                var cellDef = item.AsGodotDictionary();
                var defName = cellDef["name"].AsString();
                var existing = _sharedSettings.CellSettings.FirstOrDefault(cs => cs.Name == defName);
                if (existing != null) {
                  continue;
                }

                var cellSetting = new CellSetting
                {
                  Name = defName,
                  Color = Color.FromHtml(cellDef["color"].AsString()),
                  MoveCost = 1
                };
                _sharedSettings.CellSettings.Add(cellSetting);
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
        if (_mapData == null || _mapData.Settings == null) {
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
        if (string.IsNullOrEmpty(_currentMapDataPath)) {
          return;
        }

        if (ResourceSaver.Save(_mapData, _currentMapDataPath) == Error.Ok) {
          GD.Print($"MapData updated at: {_currentMapDataPath}");
        }
        else {
          GD.PrintErr($"Failed to update MapData at: {_currentMapDataPath}");
        }

        if (ResourceSaver.Save(_sharedSettings, DEFAULT_SETTINGS_PATH) == Error.Ok) {
          GD.Print($"MapSettings updated at: {DEFAULT_SETTINGS_PATH}");
        }
        else {
          GD.PrintErr($"Failed to update MapSettings at: {DEFAULT_SETTINGS_PATH}");
        }
    }

    private void PrintAllCells()
    {
        GD.Print("Current cell move costs:");
        foreach (var cellSetting in _sharedSettings.CellSettings) {
          GD.Print($"{cellSetting.Name}: {cellSetting.MoveCost}");
        }
    }
}
#endif
