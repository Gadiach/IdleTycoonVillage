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

        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }

        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            Zoom(scroll * zoomSencitivity);
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                moveAllowed = false;
            }
            else
            {
                moveAllowed = true;
            }

            mouseDownPosition = Input.mousePosition;
            touchPos = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && moveAllowed)
        {
            Vector3 direction = touchPos - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position += direction;

            ClampCameraPosition();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!moveAllowed)
                return;

            float distance = Vector3.Distance(mouseDownPosition, Input.mousePosition);

            if (distance < dragThreshold)
            {
                ShopSystem.Instance.TryCloseShop();
            }
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (EventSystem.current.IsPointerOverGameObject(touchOne.fingerId)
                || EventSystem.current.IsPointerOverGameObject(touchZero.fingerId))
            {
                return;
            }

            Vector2 touchZeroLastPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOneLastPos = touchOne.position - touchOne.deltaPosition;

            float distTouch = (touchZeroLastPos - touchOneLastPos).magnitude;
            float currentDistTouch = (touchZero.position - touchOne.position).magnitude;

            float difference = currentDistTouch - distTouch;

            Zoom(difference * 0.01f);
        }
        else
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        moveAllowed = false;
                    }
                    else
                    {
                        moveAllowed = true;
                    }
                    touchPos = cam.ScreenToWorldPoint(touch.position);
                    break;

                case TouchPhase.Moved:
                    if (moveAllowed)
                    {
                        Vector3 direction = touchPos - cam.ScreenToWorldPoint(touch.position);
                        cam.transform.position += direction;

                        transform.position = new Vector3
                            (
                            Mathf.Clamp(transform.position.x, leftLimit, rightLimit),
                            Mathf.Clamp(transform.position.y, bottomLimit, upperLimit),
                            transform.position.z
                            );
                    }
                    break;
            }
        }
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