using UnityEngine;

public class FollowingBackground : MonoBehaviour
{
    private Vector3 _startPos;
    private float _repeatWidth;
    private Transform _transform;
    private Camera _camera;

    public float speed = 10f;
    
    private void Start()
    {
        _transform = transform;
        _startPos = _transform.position;
        _camera = FindObjectOfType<Camera>();
        _repeatWidth = _transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        _transform.position = new Vector3(_startPos.x - _camera.transform.position.x / speed, _startPos.y, _startPos.z);

        if (_transform.position.x < _camera.transform.position.x - _repeatWidth)
            _startPos += Vector3.right * _repeatWidth;
        if (_transform.position.x > _camera.transform.position.x) _startPos -= Vector3.right * _repeatWidth;
    }
}