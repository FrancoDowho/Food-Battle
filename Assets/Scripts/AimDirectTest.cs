using UnityEngine;

public class AimDirectTest : MonoBehaviour
{
    [SerializeField] Camera _cam;
    [SerializeField] Transform _target;
    [SerializeField] float _distance = 1.2f;
    [SerializeField] float _angleOffset = 0f;

    void Update()
    {
        if (_cam == null || _target == null)
            return;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(_cam.transform.position.z);

        Vector3 mouseWorldPos = _cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3 targetPos = _target.position;
        targetPos.z = 0f;

        Vector3 direction = mouseWorldPos - targetPos;

        if (direction.sqrMagnitude <= 0.0001f)
            return;

        direction.Normalize();

        transform.position = targetPos + direction * _distance;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + _angleOffset);
    }
}