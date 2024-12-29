namespace RaycastSystem;

using Godot;

public class RaycastResult {
  public RaycastResult(Vector3 position, Vector3 normal, GodotObject collider, int colliderID, Rid rid, int shape, int faceIndex) {
    Position = position;
    Normal = normal;
    Collider = collider;
    ColliderID = colliderID;
    RID = rid;
    Shape = shape;
    FaceIndex = faceIndex;
  }
  public Vector3 Position;
  public Vector3 Normal;
  public GodotObject Collider;
  public int ColliderID;
  public Rid RID;
  public int Shape;
  public int FaceIndex;
}

[GlobalClass]
public partial class RaycastSystem : Node {
  /// <summary>
  ///
  /// </summary>
  /// <param name="world"></param>
  /// <param name="viewport"></param>
  /// <param name="collisionMask"></param>
  /// <returns>Null if not hit anything.</returns>
  public static RaycastResult RaycastOnMousePosition(World3D world, Viewport viewport, uint collisionMask = 1) {
    var spaceState = world.DirectSpaceState;
    var camera = viewport.GetCamera3D();
    var mousePos = viewport.GetMousePosition();

    var origin = camera.ProjectRayOrigin(mousePos);
    var end = origin + (camera.ProjectRayNormal(mousePos) * camera.Far);
    var query = PhysicsRayQueryParameters3D.Create(origin, end);
    query.CollideWithAreas = true;
    query.CollisionMask = collisionMask;

    var result = spaceState.IntersectRay(query);
    if (result.Count == 0) {
      return null;
    }

    return new(
      result["position"].AsVector3(),
      result["normal"].AsVector3(),
      result["collider"].AsGodotObject(),
      result["collider_id"].AsInt32(),
      result["rid"].AsRid(),
      result["shape"].AsInt32(),
      result["face_index"].AsInt32()
    );
  }
}
