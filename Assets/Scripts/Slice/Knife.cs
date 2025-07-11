using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Knife : MonoBehaviour
{
    [SerializeField] private GameObject knifeTrail;
    private Rigidbody2D rb;
    private bool isCutting = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMouseInput();
        HandleTouchInput();

        if (isCutting)
        {
            UpdateCutting();
        }
    }

    private void HandleMouseInput()
    {
        // Kiểm tra nếu chuột ở trên UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (isCutting)
            {
                StopCutting();
            }
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartCutting();
        }
        else if (Input.GetMouseButtonUp(0) && isCutting)
        {
            StopCutting();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Kiểm tra nếu chạm vào UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                if (isCutting)
                {
                    StopCutting();
                }
                return;
            }

            if (touch.phase == TouchPhase.Began)
            {
                StartCutting();
            }
            else if (touch.phase == TouchPhase.Ended && isCutting)
            {
                StopCutting();
            }
        }
    }

    private void StartCutting()
    {
        isCutting = true;
        UpdateCutting();
        StartCoroutine(DelayTrail());
    }

    private IEnumerator DelayTrail()
    {
        yield return new WaitForSeconds(0.1f);
        if (knifeTrail != null)
        {
            knifeTrail.SetActive(true);
        }
    }

    private void UpdateCutting()
    {
        Vector3 inputPosition;
        if (Input.touchCount > 0)
        {
            inputPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        rb.position = new Vector2(inputPosition.x, inputPosition.y);
    }

    private void StopCutting()
    {
        isCutting = false;
        if (knifeTrail != null)
        {
            knifeTrail.SetActive(false);
        }
    }
}
