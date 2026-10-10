using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanZoom : MonoBehaviour
{
    public static PanZoom current;

    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;
    [SerializeField] private float bottomLimit;
    [SerializeField] private float upperLimit;

    [SerializeField] private float zoomMin;
    [SerializeField] private float zoomMax;
    [SerializeField] private float zoomSencitivity;

    [SerializeField] private float dragThreshold = 10f;

    private float focusDuration = 1f;
    private float focusYOffset = 3f;

    private Vector3 mouseDownPosition;
    private Vector2 touchDownPosition;

    private Camera cam;

    private bool moveAllowed;
    private Vector3 touchPos;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        current = this;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleMouseZoom();
    }

    private void HandleMovementInput()
    {
        if (Input.touchCount > 0)
        {
            HandleTouchInput();
            return;
        }

        HandleMouseInput();
    }

    private void HandleMouseZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll == 0)
            return;

        Zoom(scroll * zoomSencitivity);
    }

    private void TryCloseOpenedShops()
    {
        BuildShopSystem.Instance.TryCloseShop();

        if (CurrencyShopUI.Instance != null)
        {
            CurrencyShopUI.Instance.Close();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BeginMouseInput();
        }
        else if (Input.GetMouseButton(0))
        {
            MoveCameraWithMouse();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndMouseInput();
        }
    }

    private void BeginMouseInput()
    {
        moveAllowed = !EventSystem.current.IsPointerOverGameObject();

        mouseDownPosition = Input.mousePosition;
        touchPos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void MoveCameraWithMouse()
    {
        if (!moveAllowed)
            return;

        Vector3 direction =
            touchPos - cam.ScreenToWorldPoint(Input.mousePosition);

        MoveCamera(direction);
    }

    private void EndMouseInput()
    {
        if (!moveAllowed)
            return;

        float dragDistance = Vector3.Distance(
            mouseDownPosition,
            Input.mousePosition
        );

        TryHandleMapTap(dragDistance);
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 2)
        {
            HandlePinchZoom();
            return;
        }

        HandleSingleTouch();
    }

    private void HandleSingleTouch()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                BeginTouch(touch);
                break;

            case TouchPhase.Moved:
                MoveCameraWithTouch(touch);
                break;

            case TouchPhase.Ended:
                EndTouch(touch);
                break;
        }
    }

    private void BeginTouch(Touch touch)
    {
        moveAllowed =
            !EventSystem.current.IsPointerOverGameObject(touch.fingerId);

        touchDownPosition = touch.position;
        touchPos = cam.ScreenToWorldPoint(touch.position);
    }

    private void MoveCameraWithTouch(Touch touch)
    {
        if (!moveAllowed)
            return;

        Vector3 direction =
            touchPos - cam.ScreenToWorldPoint(touch.position);

        MoveCamera(direction);
    }

    private void HandlePinchZoom()
    {
        Touch firstTouch = Input.GetTouch(0);
        Touch secondTouch = Input.GetTouch(1);

        if (IsTouchOverUI(firstTouch) || IsTouchOverUI(secondTouch))
            return;

        Vector2 firstPreviousPosition =
            firstTouch.position - firstTouch.deltaPosition;

        Vector2 secondPreviousPosition =
            secondTouch.position - secondTouch.deltaPosition;

        float previousDistance =
            Vector2.Distance(firstPreviousPosition, secondPreviousPosition);

        float currentDistance =
            Vector2.Distance(firstTouch.position, secondTouch.position);

        float difference = currentDistance - previousDistance;

        Zoom(difference * 0.01f);
    }

    private void MoveCamera(Vector3 direction)
    {
        cam.transform.position += direction;

        ClampCameraPosition();
    }

    private void TryHandleMapTap(float dragDistance)
    {
        if (dragDistance >= dragThreshold)
            return;

        TryCloseOpenedShops();
    }

    private bool IsTouchOverUI(Touch touch)
    {
        return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }

    private void EndTouch(Touch touch)
    {
        if (!moveAllowed)
            return;

        float dragDistance = Vector2.Distance(
            touchDownPosition,
            touch.position
        );

        TryHandleMapTap(dragDistance);
    }

    private void Zoom(float increment)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - increment, zoomMin, zoomMax);
    }

    private void ClampCameraPosition()
    {
        cam.transform.position = new Vector3(
            Mathf.Clamp(cam.transform.position.x, leftLimit, rightLimit),
            Mathf.Clamp(cam.transform.position.y, bottomLimit, upperLimit),
            cam.transform.position.z
        );
    }

    public void FocusOnObject(Transform target)
    {
        if (target == null)
            return;

        cam.transform.DOKill();

        Vector3 targetPosition = new Vector3(
         Mathf.Clamp(target.position.x, leftLimit, rightLimit),
         Mathf.Clamp(target.position.y + focusYOffset, bottomLimit, upperLimit),
         cam.transform.position.z);

        cam.transform.DOMove(targetPosition, focusDuration).SetEase(Ease.OutCubic);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 center = new Vector3(
            (leftLimit + rightLimit) / 2.0f,
            (bottomLimit + upperLimit) / 2.0f,
            0
        );

        Vector3 size = new Vector3(
            rightLimit - leftLimit,
            upperLimit - bottomLimit,
            0
        );

        Gizmos.DrawWireCube(center, size);       
    }
}