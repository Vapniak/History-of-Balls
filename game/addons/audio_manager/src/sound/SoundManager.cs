namespace AudioManager;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class SoundManager : Node {
  [Signal] public delegate void BanksUpdatedEventHandler();

  [Signal] public delegate void PoolsUpdatedEventHandler();

  [Export] private Array<SoundBank> Banks { get; set; } = default!;


  public static SoundManager Instance { get; private set; } = default!;
  private readonly List<PooledAudioStreamPlayer> _1dPlayers = new();
  // private List<PooledAudioStreamPlayer2D> _2dPlayers = new List<PooledAudioStreamPlayer2D>();
  // private List<PooledAudioStreamPlayer3D> _3dPlayers = new List<PooledAudioStreamPlayer3D>();
  private readonly System.Collections.Generic.Dictionary<string, SoundBank> _eventTable = new();
  private int _eventTableHash;

  public override void _EnterTree() {
    Instance = this;

    ProcessMode = ProcessModeEnum.Always;

    GetTree().NodeAdded += OnNodeAdded;
  }

  public override void _Ready() {
    for (var i = 0; i < 16; i++) {
      CreatePlayer1d();
    }

    foreach (var bank in Banks) {
      AddBank(bank);
    }
  }

  public override void _Process(double delta) {
    var currentHash = _eventTable.GetHashCode();
    if (_eventTableHash != currentHash) {
      _eventTableHash = currentHash;
      EmitSignal(SignalName.BanksUpdated);
    }
  }

  // public object QuickInstance(object pInstance, Func<object> pFactory, Node pBase = null, bool pFinishPlaying = false) {
  //   if (ShouldSkipInstancing(pInstance))
  //     return pInstance;

  //   var newInstance = pFactory();

  //   if (pBase != null)
  //     ReleaseOnExit(pBase, (Node)newInstance, pFinishPlaying);

  //   return newInstance;
  // }

  public void Play(string pBankLabel, string pEventName, string pBus = "") {
    var instance = InstanceManual(pBankLabel, pEventName, false, pBus, false);
    if (instance != null) {
      instance.Trigger();
      instance.Release(true);
    }
  }

  // public void PlayAtPosition(string pBankLabel, string pEventName, object pPosition, string pBus = "") {
  //   var instance = InstanceManual(pBankLabel, pEventName, false, pBus, false, pPosition);
  //   instance.Trigger();
  //   instance.Release(true);
  // }

  // public void PlayOnNode(string pBankLabel, string pEventName, Node pNode, string pBus = "") {
  //   var instance = InstanceManual(pBankLabel, pEventName, false, pBus, false, pNode);
  //   instance.Trigger();
  //   instance.Release(true);
  // }

  public void PlayVaried(string pBankLabel, string pEventName, float pPitch = 1.0f, float pVolume = 0.0f, string pBus = "") {
    var instance = InstanceManual(pBankLabel, pEventName, false, pBus, false);
    if (instance != null) {
      instance.TriggerVaried(pPitch, pVolume);
      instance.Release(true);
    }
  }

  // public void PlayAtPositionVaried(string pBankLabel, string pEventName, object pPosition, float pPitch = 1.0f, float pVolume = 0.0f, string pBus = "") {
  //   var instance = InstanceManual(pBankLabel, pEventName, false, pBus, false, pPosition);
  //   instance.TriggerVaried(pPitch, pVolume);
  //   instance.Release(true);
  // }

  // public void PlayOnNodeVaried(string pBankLabel, string pEventName, Node pNode, float pPitch = 1.0f, float pVolume = 0.0f, string pBus = "") {
  //   var instance = InstanceManual(pBankLabel, pEventName, false, pBus, false, pNode);
  //   instance.TriggerVaried(pPitch, pVolume);
  //   instance.Release(true);
  // }

  public PooledAudioStreamPlayerBase? CreateInstance(string pBankLabel, string pEventName, string pBus = "") {
    return InstanceManual(pBankLabel, pEventName, true, pBus, false);
  }

  // public PooledAudioStreamPlayerBase InstanceAtPosition(string pBankLabel, string pEventName, object pPosition, string pBus = "") {
  //   return InstanceManual(pBankLabel, pEventName, true, pBus, false, pPosition);
  // }

  // public PooledAudioStreamPlayerBase InstanceOnNode(string pBankLabel, string pEventName, Node pNode, string pBus = "") {
  //   return InstanceManual(pBankLabel, pEventName, true, pBus, false, pNode);
  // }

  // public PooledAudioStreamPlayerBase InstancePoly(string pBankLabel, string pEventName, string pBus = "") {
  //   return InstanceManual(pBankLabel, pEventName, true, pBus, true, null);
  // }

  // public PooledAudioStreamPlayerBase InstanceAtPositionPoly(string pBankLabel, string pEventName, object pPosition, string pBus = "") {
  //   return InstanceManual(pBankLabel, pEventName, true, pBus, true, pPosition);
  // }

  // public PooledAudioStreamPlayerBase InstanceOnNodePoly(string pBankLabel, string pEventName, Node pNode, string pBus = "") {
  //   return InstanceManual(pBankLabel, pEventName, true, pBus, true, pNode);
  // }

  public void ReleaseOnExit(Node node, PooledAudioStreamPlayerBase pInstance, bool pFinishPlaying = false) {
    if (pInstance == null || node == null) {
      return;
    }

    node.TreeExiting += () => pInstance.Release(pFinishPlaying);
  }


  public void RemoveBank(string pBankLabel) {
    if (!_eventTable.ContainsKey(pBankLabel)) {
      return;
    }

    _eventTable.Remove(pBankLabel);
  }

  public void ClearBanks() {
    _eventTable.Clear();
  }

  private void InitializePool(int pSize, string methodName) {
    for (var i = 0; i < pSize; i++) {
      CallDeferred(methodName);
    }
  }


  public void AddBank(SoundBank bank) {
    _eventTable.TryAdd(bank.Label, bank);
  }

  public void RemoveBank(SoundBank bank) {
    if (!_eventTable.TryGetValue(bank.Label, out var value)) {
      return;
    }

    _eventTable.Remove(bank.Label);
  }

  private static string GetBus(string pBankBus, string pEventBus) {
    if (pEventBus is not null and not "") {
      return pEventBus;
    }

    if (pBankBus is not null and not "") {
      return pBankBus;
    }

    // TODO: return value from settings
    return "";
  }

  private PooledAudioStreamPlayerBase? InstanceManual(string pBankLabel, string pEventName, bool pReserved = false, string pBus = "", bool pPoly = false, object? pAttachment = null) {
    var player = GetPlayer(pAttachment);

    if (player == null) {
      GD.PushWarning($"The event [{pEventName}] on bank [{pBankLabel}] can't be instanced; no pooled players available.");
      return null;
    }

    _eventTable.TryGetValue(pBankLabel, out var bank);
    var @event = bank?.Events.FirstOrDefault(e => e.Name == pEventName);
    if (bank == null || @event == null) {
      return null;
    }

    pBus = pBus == "" ? GetBus(bank.Bus, @event.Bus) : pBus;

    player.Configure(@event.Streams.ToList(), pReserved, pBus, pPoly, @event.Volume, @event.Pitch, 0);

    return player;
  }

  private static bool IsPlayerFree(PooledAudioStreamPlayerBase pPlayer) {
    return !pPlayer.Playing && !pPlayer.Reserved;
  }

  private static PooledAudioStreamPlayerBase? GetPlayerFromPool<T>(List<T> pPool) where T : PooledAudioStreamPlayerBase {
    if (pPool.Count == 0) {
      GD.PushError("Player pool has not been initialised. This can occur when calling a [play/instance*] function from [_ready].");
      return null;
    }

    foreach (var player in pPool) {
      if (IsPlayerFree(player)) {
        return player;
      }
    }

    GD.PushWarning("Player pool exhausted, consider increasing the pool size in the project settings (Audio/Manager/Pooling) or releasing unused audio stream players.");
    return null;
  }

  private PooledAudioStreamPlayer? GetPlayer1d() {
    return GetPlayerFromPool(_1dPlayers) as PooledAudioStreamPlayer;
  }

  private PooledAudioStreamPlayerBase? GetPlayer(object? attachment = null) {
    return GetPlayer1d();
  }

  private T AddPlayerToPool<T>(T pPlayer, List<T> pPool) where T : PooledAudioStreamPlayerBase {
    AddChild(pPlayer);

    pPlayer.Released += () => OnPlayerReleased(pPlayer);
    pPlayer.Finished += () => OnPlayerFinished(pPlayer);
    pPool.Add(pPlayer);

    return pPlayer;
  }

  private void CreatePlayer1d() {
    AddPlayerToPool(PooledAudioStreamPlayer.Create(), _1dPlayers);
  }

  // private void CreatePlayer2d() {
  //   AddPlayerToPool(PooledAudioStreamPlayer2D.Create(), _2dPlayers);
  // }

  // private void CreatePlayer3d() {
  //   AddPlayerToPool(PooledAudioStreamPlayer3D.Create(), _3dPlayers);
  // }

  private void OnPlayerReleased(Node pPlayer) {
    var player = (PooledAudioStreamPlayerBase)pPlayer;
    if (player.Playing) {
      return;
    }

    EmitSignal(SignalName.PoolsUpdated);
  }

  private void OnPlayerFinished(Node pPlayer) {
    var player = (PooledAudioStreamPlayerBase)pPlayer;
    if (player.Reserved) {
      return;
    }

    EmitSignal(SignalName.PoolsUpdated);
  }

  private void OnNodeAdded(Node node) {
    if (node is Button button) {
      ConnectButtonSignals(button);
    }
  }

  private void ConnectButtonSignals(Button button) {
    var weakButton = WeakRef(button);

    void handlePressed() {
      if (weakButton?.GetRef().As<Node>() is Button validButton) {
        Play("ui", "click");
      }
    }

    // void handleHover() {
    //   if (weakButton.GetRef().As<Node>() is Button validButton)
    //     Play("UI", "hover");
    // }

    button.Pressed += handlePressed;
    //button.MouseEntered += handleHover;
  }
}