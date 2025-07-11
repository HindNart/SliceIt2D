using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SlicerController : MonoBehaviour
{
    private GameObject target;
    private Vector3? startPoint = null;
    private Vector3? endPoint = null;

    private void Start()
    {
        GameManager.Instance.OnNextLevelEvent.AddListener(UpdateTarget);
    }

    private void UpdateTarget()
    {
        target = ShapeFactory.Instance.GetShape();
        if (target == null)
        {
            Debug.LogError("Target shape is not set. Please create a shape first.");
            return;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CutAttempts > 0)
        {
            HandleMouseInput();
            HandleTouchInput();
        }
    }

    // Hỗ trợ cắt bằng chuột
    private void HandleMouseInput()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            startPoint = null;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)mousePos, Vector2.zero, 0f, LayerMask.GetMask("Shape"));
            if (hit.collider != null && hit.collider.gameObject == target)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX("Slice");
                }
                startPoint = mousePos;
            }
        }
        else if (Input.GetMouseButtonUp(0) && startPoint.HasValue)
        {
            endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endPoint = new Vector3(endPoint.Value.x, endPoint.Value.y, 0);

            RaycastHit2D hit = Physics2D.Linecast((Vector2)startPoint, (Vector2)endPoint, LayerMask.GetMask("Shape"));
            if (hit.collider != null && hit.collider.gameObject == target)
            {
                if (target != null)
                {
                    SliceCommand command = new SliceCommand(target, startPoint.Value, endPoint.Value);
                    command.Execute();
                    StartCoroutine(CutCompleted());
                }
            }
            startPoint = null;
            endPoint = null;
        }
    }

    private IEnumerator CutCompleted()
    {
        yield return new WaitForSeconds(0.01f);
        GameObject[] shapes = GameObject.FindGameObjectsWithTag("Shape");
        if (shapes.Length < 2)
            yield return null;

        GameObject shape1 = shapes[0];
        GameObject shape2 = shapes[1];

        if (shape1 == null)
        {
            Debug.LogError("Shape is null after slicing.");
            yield return null;
        }
        if (shape2 == null)
        {
            Debug.LogError("Shape2 is null after slicing.");
            yield return null;
        }

        int Y = shape1.transform.localPosition.y > 0 || shape1.transform.localPosition.x > 0 ? 1 : -1;
        shape1.transform.DOMove(new Vector3(0, Y, 95), 1.5f)
            .SetEase(Ease.OutBounce);
        shape2.transform.DOMove(new Vector3(0, -Y, 95), 1.5f)
            .SetEase(Ease.OutBounce);

        float areaShape1 = CalculateArea(shape1.GetComponent<PolygonCollider2D>());
        float areaShape2 = CalculateArea(shape2.GetComponent<PolygonCollider2D>());

        float ratio1 = areaShape1 / (areaShape1 + areaShape2);
        float ratio2 = areaShape2 / (areaShape1 + areaShape2);
        Debug.Log($"Ratio Shape 1: {ratio1}");
        Debug.Log($"Ratio Shape 2: {ratio2}");

        float closestRatio = Mathf.Abs(ratio1 - GameManager.Instance.TargetRatio) < Mathf.Abs(ratio2 - GameManager.Instance.TargetRatio) ? ratio1 : ratio2;
        CutEventManager.NotifyCutPerformed(closestRatio);
    }

    private float CalculateArea(PolygonCollider2D polygonCollider)
    {
        float totalArea = 0f;

        // Lấy tất cả các đường dẫn (paths) từ PolygonCollider2D
        for (int pathIndex = 0; pathIndex < polygonCollider.pathCount; pathIndex++)
        {
            Vector2[] points = polygonCollider.GetPath(pathIndex);

            // Tính diện tích của đường dẫn hiện tại bằng công thức Shoelace
            float pathArea = 0f;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 current = points[i];
                Vector2 next = points[(i + 1) % points.Length]; // Đỉnh tiếp theo (quay lại đầu nếu vượt quá)

                pathArea += (current.x * next.y) - (next.x * current.y);
            }
            pathArea = Mathf.Abs(pathArea) * 0.5f;

            // Nếu là đường dẫn đầu tiên (đa giác chính), cộng vào tổng diện tích
            // Nếu là đường dẫn phụ (lỗ), trừ đi diện tích
            totalArea += (pathIndex == 0) ? pathArea : -pathArea;
        }

        // Áp dụng scale của GameObject
        Vector3 scale = transform.localScale;
        totalArea *= Mathf.Abs(scale.x * scale.y);

        return totalArea;
    }

    //Hỗ trợ cảm ứng trên mobile
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (UnityEngine.EventSystems.EventSystem.current != null &&
                    UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchPos.z = 0;
                RaycastHit2D hit = Physics2D.Raycast((Vector2)touchPos, Vector2.zero, 0f, LayerMask.GetMask("Shape"));
                if (hit.collider != null && hit.collider.gameObject == target)
                {
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySFX("Slice");
                    }
                    startPoint = touchPos;
                }
            }
            else if (touch.phase == TouchPhase.Ended && startPoint.HasValue)
            {
                endPoint = Camera.main.ScreenToWorldPoint(touch.position);
                endPoint = new Vector3(endPoint.Value.x, endPoint.Value.y, 0);

                RaycastHit2D hit = Physics2D.Linecast((Vector2)startPoint, (Vector2)endPoint, LayerMask.GetMask("Shape"));
                if (hit.collider != null && hit.collider.gameObject == target)
                {
                    if (target != null)
                    {
                        SliceCommand command = new SliceCommand(target, startPoint.Value, endPoint.Value);
                        command.Execute();
                        StartCoroutine(CutCompleted());
                    }
                }
                startPoint = null;
                endPoint = null;
            }
        }
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
