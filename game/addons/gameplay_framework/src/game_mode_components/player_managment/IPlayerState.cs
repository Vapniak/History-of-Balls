namespace GameplayFramework;

public interface IPlayerState {
  public T GetController<T>() where T : class, IController;
  public IController GetController();
  public void SetController(Controller controller);
}
