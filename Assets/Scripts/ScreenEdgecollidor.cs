using UnityEngine;

public class ScreenEdgeBlockers : MonoBehaviour
{
    public BoxCollider2D topCollider;
    public BoxCollider2D bottomCollider;
    public BoxCollider2D leftCollider;
    public BoxCollider2D rightCollider;

    public float colliderThickness = 1f; // Thickness of the border walls

    void Start()
    {
        PositionEdgeColliders();
    }

   void PositionEdgeColliders()
{
    Camera cam = Camera.main;
    if (cam == null || !cam.orthographic)
    {
        Debug.LogError("Main camera is missing or not orthographic.");
        return;
    }

    float cameraHeight = 2f * cam.orthographicSize;
    float cameraWidth = cameraHeight * cam.aspect;

    Vector3 camPos = cam.transform.position;

    // Horizontal colliders
    topCollider.size = new Vector2(cameraWidth, colliderThickness);
    topCollider.offset = Vector2.zero;
    topCollider.transform.position = new Vector3(camPos.x, camPos.y + (cameraHeight / 2f) + (colliderThickness / 2f), 0f);

    bottomCollider.size = new Vector2(cameraWidth, colliderThickness);
    bottomCollider.offset = Vector2.zero;
    bottomCollider.transform.position = new Vector3(camPos.x, camPos.y - (cameraHeight / 2f) - (colliderThickness / 2f), 0f);

    // Vertical colliders
    leftCollider.size = new Vector2(colliderThickness, cameraHeight);
    leftCollider.offset = Vector2.zero;
    leftCollider.transform.position = new Vector3(camPos.x - (cameraWidth / 2f) - (colliderThickness / 2f), camPos.y, 0f);

    rightCollider.size = new Vector2(colliderThickness, cameraHeight);
    rightCollider.offset = Vector2.zero;
    rightCollider.transform.position = new Vector3(camPos.x + (cameraWidth / 2f) + (colliderThickness / 2f), camPos.y, 0f);
}

}
