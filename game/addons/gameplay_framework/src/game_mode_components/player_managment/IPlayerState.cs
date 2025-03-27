namespace GameplayFramework;

public interface IPlayerState {
  public string PlayerName { get; set; }
  public int PlayerIndex { get; set; }
  public Controller? OwningController { get; set; }
  public T GetController<T>() where T : class, IController;
  public IController GetController();
  public void SetController(Controller controller);
}
