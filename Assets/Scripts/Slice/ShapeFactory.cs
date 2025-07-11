using DG.Tweening;
using UnityEngine;

public class ShapeFactory : MonoBehaviour
{
    public static ShapeFactory Instance { get; private set; }
    [SerializeField] private GameObject[] shapePrefabs;
    private GameObject shape;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateShape()
    {
        shape = Instantiate(shapePrefabs[Random.Range(0, shapePrefabs.Length)], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        shape.transform.localScale = Vector3.zero;
        shape.transform.DOScale(Vector3.one * 0.8f, 0.5f).SetEase(Ease.OutBack);
    }

    public void DestroyShape()
    {
        GameObject[] existingShapes = GameObject.FindGameObjectsWithTag("Shape");
        if (existingShapes.Length == 0)
        {
            Debug.LogWarning("No shapes found to destroy.");
            return;
        }
        foreach (GameObject existingShape in existingShapes)
        {
            if (existingShape != null)
            {
                existingShape.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Destroy(existingShape);
                });
            }
        }
    }

    public GameObject GetShape()
    {
        return shape;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        DOTween.KillAll();
    }
}