namespace HOB.GameEntity;

using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class StatsContainer : Resource {
  [Export] public Array<BaseStat> Stats { get; private set; }

  public void Init() {

    var duplicatedStats = new Array<BaseStat>();
    foreach (var stat in Stats) {
      duplicatedStats.Add(stat.Duplicate() as BaseStat);
    }

    Stats = duplicatedStats;
  }

  public bool TryGetStat<T>(out T stat) where T : BaseStat {
    stat = GetStat<T>();
    if (stat == null) {
      return false;
    }

    return true;
  }
  public T GetStat<T>() where T : BaseStat {
    return Stats.OfType<T>().FirstOrDefault();
  }
}
