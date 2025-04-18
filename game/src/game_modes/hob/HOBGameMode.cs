namespace HOB;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AudioManager;
using GameplayFramework;
using GameplayTags;
using Godot;
using Godot.Collections;
using GodotStateCharts;
using HexGridMap;
using HOB.GameEntity;
using WidgetSystem;

[GlobalClass]
public partial class HOBGameMode : GameMode {
  [Export] private PlayerAttributeSet? PlayerAttributeSet { get; set; }
  [Export] public Array<EntityIcon>? EntityIcons { get; private set; }

  [Export] private MissionData? MatchData { get; set; }

  [Export] private PackedScene? PlayerControllerScene { get; set; }
  [Export] private PackedScene? PlayerCharacterScene { get; set; }
  [Export] private PackedScene? AIControllerScene { get; set; }
  [Export] private PackedScene? HUDScene { get; set; }

  [Export] private Node? StateChartNode { get; set; }

  private StateChart? StateChart { get; set; }

  [Export] private MatchComponent MatchComponent { get; set; } = default!;
  [Export] private HOBPlayerManagmentComponent PlayerManagmentComponent { get; set; } = default!;

  private GameBoard GameBoard => GetGameState().GameBoard;

  private double _savedTimeScale;

  public override void _EnterTree() {
    base._EnterTree();

    MusicManager.Instance.Play("music", "medival", autoLoop: true);

    GetGameState().GameBoard = GameInstance.GetWorld().GetCurrentLevel().GetChildByType<GameBoard>();

    StateChart = StateChart.Of(StateChartNode!);

    PlayerManagmentComponent!.PlayerSpawned += (playerState) => MatchComponent!.OnPlayerSpawned(playerState as IMatchPlayerState);

    GameBoard.GridCreated += () => CallDeferred(MethodName.OnGridCreated);
  }

  public override void _ExitTree() {
    base._ExitTree();

    WidgetManager.Instance.PopAllWidgets();
    Engine.TimeScale = 1;
    GameInstance.SetPause(false);
  }

  public override void _Ready() {
    base._Ready();

    if (MatchData?.Map != null) {
      GameBoard.Init(MatchData.Map);
      PlaceProps();
    }
  }

  public void Pause() {
    StateChart?.SendEvent("pause");
  }

  public void Resume() {
    StateChart?.SendEvent("resume");
  }

  public bool IsCurrentTurn(IMatchController controller) {
    return MatchComponent.IsCurrentTurn(controller);
  }


  public Texture2D? GetIconFor(Entity entity) {
    return EntityIcons?.FirstOrDefault(i => i.EntityType != null && entity.AbilitySystem.OwnedTags.HasExactTag(i.EntityType), null)?.Icon;
  }

  public Texture2D? GetIconFor(EntityData entityData) {
    return EntityIcons?.FirstOrDefault(i => i.EntityType != null && entityData.Tags.HasExactTag(i.EntityType), null)?.Icon;
  }

  public IMatchEvents GetMatchEvents() => MatchComponent!;
  public IEntityManagment GetEntityManagment() => MatchComponent!;

  public ITurnManagment GetTurnManagment() => MatchComponent!;

  public override HOBGameState GetGameState() => base.GetGameState() as HOBGameState;

  protected override GameState CreateGameState() => new HOBGameState();

  private void OnPausedStateEntered() {
    WidgetManager.Instance.PushWidget<PauseMenuWidget>(menu => {
      menu.ResumeEvent += Resume;
      menu.MainMenuEvent += () => _ = OnMainMenu();
      menu.QuitEvent += OnQuit;
    });

    _savedTimeScale = Engine.TimeScale;
    Engine.TimeScale = 1;
    GameInstance.SetPause(true);
  }
  private void OnPausedStateExited() {
    GameInstance.SetPause(false);
    Engine.TimeScale = _savedTimeScale;
  }

  protected virtual void OnInMatchStateEntered() {
    MatchComponent.OnGameStarted();
  }

  protected virtual void OnInMatchStateExited() {

  }

  private void OnInMatchPlayingStateProcess(float delta) {
    GetGameState().GameTimeMSec += (ulong)(delta * 1000f);
  }


  private void OnMatchEndedStateEntered() {
    GameInstance.SetPause(true);
  }

  private async Task OnMainMenu() {
    await GameInstance.GetWorld().OpenLevel("main_menu_level");
  }

  private void OnQuit() => GameInstance.QuitGame();

  private void OnGridCreated() {
    _ = InitGame();
  }

  private async Task InitGame() {
    if (MatchData == null) {
      return;
    }

    foreach (var playerData in MatchData.PlayerSpawnDatas) {
      Controller? controller = null;
      if (playerData.PlayerType != PlayerType.None) {
        if (PlayerAttributeSet == null) {
          Debug.Assert(false, "Player attribute set cannot be null");
          return;
        }

        if (playerData.ProducableEntities == null) {
          Debug.Assert(false, "Producable entities cannot be null");
          return;
        }

        var state = new HOBPlayerState(PlayerAttributeSet, playerData.ProducableEntities.ProducableEntities, playerData.OwnedEntities.Entities) {
          Country = playerData.Country
        };

        if (playerData.PlayerType == PlayerType.Player) {
          controller = PlayerControllerScene.Instantiate<Controller>();
          var hud = HUDScene.Instantiate<HUD>();
          var characterNode = PlayerCharacterScene.Instantiate<Node>();
          PlayerManagmentComponent.SpawnPlayer(new(controller, state, "Player", hud, characterNode));
        }
        else if (playerData.PlayerType == PlayerType.AI) {
          var ai = AIControllerScene.Instantiate<AIController>();
          if (playerData.AIProfile != null) {
            ai.Profile = playerData.AIProfile;
          }

          controller = ai;
          PlayerManagmentComponent.SpawnPlayer(new(controller, state, "AI"));
        }

      }

      await Task.Delay(500);
      if (playerData.SpawnedEntities != null) {
        foreach (var entitySpawn in playerData.SpawnedEntities) {
          var pitch = 5f;
          if (entitySpawn?.EntityData != null && entitySpawn.SpawnAt != null) {
            foreach (var coord in entitySpawn.SpawnAt) {
              await Task.Delay(50);
              MatchComponent.AddEntityOnClosestAvailableCell(entitySpawn.EntityData, new OffsetCoord(coord.X, coord.Y), controller == null ? null : controller as IMatchController);
              pitch += 0.1f;
            }
          }
        }
      }
    }

    StateChart.CallDeferred(StateChart.MethodName.SendEvent, "match_start");
  }

  public void PlaceProps() {
    var rng = new RandomNumberGenerator();
    var settingsGroups = GroupCellsByPropSettings();
    var meshGroups = new System.Collections.Generic.Dictionary<(Mesh Mesh, Material Material), List<Transform3D>>();

    var occupiedCells = new List<OffsetCoord>();

    foreach (var playerData in MatchData.PlayerSpawnDatas) {
      foreach (var entity in playerData.SpawnedEntities) {
        foreach (var spawnAt in entity.SpawnAt) {
          occupiedCells.Add(new(spawnAt.X, spawnAt.Y));
        }
      }
    }


    foreach (var kvp in settingsGroups) {
      var propSetting = kvp.Key;
      var cells = kvp.Value;
      var meshDataList = GetCachedMultiMeshData(propSetting);


      foreach (var cell in cells) {
        if (occupiedCells.Contains(cell.OffsetCoord) || rng.Randf() * 100 > propSetting.Chance) {
          continue;
        }


        for (var i = 0; i < rng.RandiRange(propSetting.AmountRange.X, propSetting.AmountRange.Y); i++) {
          var worldPos = cell.GetRealPosition() +
                   new Vector3(
                       (float)rng.RandfRange(-1f, 1f),
                       0,
                       (float)rng.RandfRange(-1f, 1f)
                   );
          var rotation = rng.RandfRange(Mathf.DegToRad(0), Mathf.DegToRad(360));

          foreach (var (mesh, material, initialTransform) in meshDataList) {
            if (!meshGroups.TryGetValue((mesh, material), out var transforms)) {
              transforms = new List<Transform3D>(cells.Count);
              meshGroups[(mesh, material)] = transforms;
            }

            var finalTransform = Transform3D.Identity
                      .Rotated(Vector3.Up, rotation)
                      .Translated(worldPos)
                      * initialTransform;

            transforms.Add(finalTransform);
          }
        }
      }
    }

    foreach (var kvp in meshGroups) {
      if (kvp.Value.Count == 0) {
        continue;
      }

      var multimesh = new MultiMesh {
        Mesh = kvp.Key.Mesh,
        TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
        InstanceCount = kvp.Value.Count
      };

      if (kvp.Key.Material != null) {
        multimesh.Mesh.SurfaceSetMaterial(0, kvp.Key.Material);
      }

      for (var i = 0; i < kvp.Value.Count; i++) {
        multimesh.SetInstanceTransform(i, kvp.Value[i]);
      }

      AddChild(new MultiMeshInstance3D { Multimesh = multimesh });
    }
  }

  private System.Collections.Generic.Dictionary<PropSetting, List<(Mesh Mesh, Material Material, Transform3D Transform)>> _multiMeshCache
      = new();

  private List<(Mesh Mesh, Material Material, Transform3D Transform)> GetCachedMultiMeshData(PropSetting setting) {
    if (_multiMeshCache.TryGetValue(setting, out var cached)) {
      return cached;
    }

    using var scene = setting.PropScene.Instantiate<Node3D>();
    var meshInstances = GetAllMeshInstances(scene);
    var result = new List<(Mesh, Material, Transform3D)>();

    foreach (var meshInstance in meshInstances) {

      var transform = CalculateFullTransform(meshInstance, scene);
      result.Add((
          meshInstance.Mesh,
          meshInstance.GetActiveMaterial(0),
          transform
      ));
    }

    _multiMeshCache[setting] = result;
    return result;
  }

  private List<MeshInstance3D> GetAllMeshInstances(Node root) {
    var instances = new List<MeshInstance3D>();
    var queue = new Queue<Node>();
    queue.Enqueue(root);

    while (queue.Count > 0) {
      var node = queue.Dequeue();
      if (node is MeshInstance3D meshInstance) {
        instances.Add(meshInstance);
      }

      foreach (var child in node.GetChildren()) {
        queue.Enqueue(child);
      }
    }

    return instances;
  }

  private Transform3D CalculateFullTransform(Node3D node, Node3D root) {
    var transform = node.Transform;
    var current = node.GetParent();

    while (current != null) {
      if (current is Node3D node3d) {
        transform = node3d.Transform * transform;
      }
      current = current.GetParent<Node3D>();
    }

    return transform;
  }

  private System.Collections.Generic.Dictionary<PropSetting, List<GameCell>> GroupCellsByPropSettings() {
    var settingsDict = new System.Collections.Generic.Dictionary<PropSetting, List<GameCell>>();

    foreach (var cell in GameBoard.Grid.GetCells()) {
      foreach (var propSetting in cell.GetSetting().Props) {
        if (!settingsDict.TryGetValue(propSetting, out var cells)) {
          cells = new List<GameCell>();
          settingsDict[propSetting] = cells;
        }
        cells.Add(cell);
      }
    }

    return settingsDict;
  }

  protected void EndGame(IMatchController winner) {
    MatchComponent.TriggerGameEnd(winner);

    StateChart.SendEvent("match_end");
    WidgetManager.Instance.PushWidget<MatchEndMenuWidget>(menu => {
      menu.OnGameEnd(winner);
    });
  }

  // protected virtual IMatchController? CheckWinner() {
  //   var alivePlayers = new List<IMatchController>();
  //   var eliminatedPlayers = new List<IMatchController>();

  //   foreach (var player in GetGameState().PlayerArray) {
  //     var controller = player.GetController<IMatchController>();
  //     var entities = GetEntityManagment().GetOwnedEntites(controller);

  //     if (entities.Any(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructureCity)))) {
  //       alivePlayers.Add(controller);
  //     }
  //     else {
  //       eliminatedPlayers.Add(controller);
  //     }
  //   }

  //   if (eliminatedPlayers.Count > 0) {
  //     return alivePlayers.FirstOrDefault();
  //   }
  //   else {
  //     return null;
  //   }
  // }
}
