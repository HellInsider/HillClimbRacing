using UnityEngine;

public class ColliderLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();

        if (lineRenderer == null || edgeCollider == null)
        {
            Debug.LogError("LineRenderer or EdgeCollider2D component missing!");
            enabled = false;
            return;
        }
        UpdateCollider();
    }

    void Update()
    {
        UpdateCollider();
    }

    private void UpdateCollider()
    {
        if (lineRenderer.positionCount == 0) return;

        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);

        Vector2[] points = new Vector2[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 localPos = transform.InverseTransformPoint(positions[i]);
            points[i] = new Vector2(localPos.x, localPos.y);
        }

        edgeCollider.points = points;
    }
}