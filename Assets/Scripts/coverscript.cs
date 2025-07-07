using UnityEngine;
using UnityEngine.InputSystem;

public class coverScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject shipCover;
    public CharacterManager characterManager;

    public float zoomedInSize = 1.6f;
    public float zoomedOutSize = 5f;

    public float cameraXOffsetIn = 0f;      // X offset when zoomed in
    public float cameraXOffsetOut = 0f;     // X offset when zoomed out
    public float cameraYOffsetIn = 0.5f;    // Y offset when zoomed in
    public float cameraYOffsetOut = -0.5f;  // Y offset when zoomed out

    private bool isZoomedOut = true;
    public bool allowMouseZoom = true;

    void Start()
    {
        isZoomedOut = true;
        shipCover.SetActive(true);

        mainCamera.orthographicSize = zoomedOutSize;
        UpdateCameraPosition(); // Set correct initial position
    }

    void Update()
{
    
    if (!allowMouseZoom) return;
    if (Mouse.current.rightButton.isPressed)
        {
            SetZoomedIn(true);
        }
        else
        {
            SetZoomedIn(false);
        }
}


    void SetZoomedIn(bool zoomIn)
    {
        if (isZoomedOut == !zoomIn) return; // already in desired state

        isZoomedOut = !zoomIn;

        mainCamera.orthographicSize = isZoomedOut ? zoomedOutSize : zoomedInSize;

        characterManager.SetMovementEnabled(!isZoomedOut);
        shipCover.SetActive(isZoomedOut);

        UpdateCameraPosition();
    }


    void UpdateCameraPosition()
    {
        Vector3 camPos = mainCamera.transform.position;
        camPos.x = isZoomedOut ? cameraXOffsetOut : cameraXOffsetIn;
        camPos.y = isZoomedOut ? cameraYOffsetOut : cameraYOffsetIn;
        mainCamera.transform.position = camPos;
    }
    public void ForceZoomIn()
{
    if (!isZoomedOut) return; // already zoomed in
    isZoomedOut = false;
    mainCamera.orthographicSize = zoomedInSize;
    characterManager.SetMovementEnabled(true);
    shipCover.SetActive(false);
    UpdateCameraPosition();
}
}
