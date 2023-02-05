using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RaycastReflection : MonoBehaviour
{
    [SerializeField] private Transform _paddleTransform;
    public int reflections;
    public float maxLegth;

    private LineRenderer lineRenderer;
    private Ray ray;
    private RaycastHit hit;
    private Vector3 direction;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        ray = new Ray(_paddleTransform.position, new Vector3(_paddleTransform.position.x * 3f, 0.5f, 0).normalized / 2 - _paddleTransform.position);

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, _paddleTransform.position);
        float remainingLenght = maxLegth;

        for (int i = 0; i < reflections; i++)
        {
            if (Physics.Raycast(ray.origin,ray.direction, out hit, remainingLenght))
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                remainingLenght -= Vector3.Distance(ray.origin, hit.point);
                ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
                if (hit.collider.tag != "Wall")
                    break;
            }
            else
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * remainingLenght);
            }
        }
    }
}
