namespace Whimsical.Gameplay
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Debug;
    using UnityEngine;


    public class CombatCamera : MonoBehaviour
    {
        private Camera _cam;

        [SerializeField] private Vector2 _offset;
        [SerializeField] private float _timeToFollow;
        [SerializeField] private float _minProjectionSize;
        [SerializeField] private float _maxProjectionSize;
        private Vector2 _maxRightOffset;
        private Vector2 _currentPositionVelocity;

        [SerializeField] private List<Transform> _targets = new();

        public void Start()
        {
            _cam = this.GetComponent<Camera>();

            _maxRightOffset = new Vector2(1 - _offset.x, 1 - _offset.y);
            DebugExtensions.Log($"Using offsets: {_offset} and max offsets: {_maxRightOffset}");
        }

        public void Update()
        {
            var screenPositions = _targets.Select(target =>
            {
                var normalizedScreenPos = _cam.WorldToScreenPoint(target.position);
                normalizedScreenPos.x /= Screen.width;
                normalizedScreenPos.y /= Screen.height;
                return normalizedScreenPos;
            }).ToArray();

            if (!screenPositions.Any()) return;

            var topRight = new Vector2(screenPositions.Max(pos => pos.x), screenPositions.Max(pos => pos.y));
            var bottomLeft = new Vector2(screenPositions.Min(pos => pos.x), screenPositions.Min(pos => pos.y));
            var shouldZoomIn = bottomLeft.x < _offset.x
                               || topRight.x > _maxRightOffset.x
                               || bottomLeft.y < _offset.y
                               || topRight.y > _maxRightOffset.y;

            var centroid = _targets
                .Select(target => target.position)
                .Aggregate((accumulator, current) => accumulator + current);
            centroid /= _targets.Count;

            DebugExtensions.Log($"centroid is: {centroid}");

            var targetCameraPosition = Vector2.SmoothDamp(
                _cam.transform.position,
                centroid,
                ref _currentPositionVelocity,
                _timeToFollow);

            _cam.transform.position = new Vector3(
                targetCameraPosition.x,
                targetCameraPosition.y,
                _cam.transform.position.z);

            const float speed = 0.01f;
            var multiplier = shouldZoomIn ? 1 : -1;
            _cam.orthographicSize += multiplier * speed;
            _cam.orthographicSize = Math.Clamp(_cam.orthographicSize, _minProjectionSize, _maxProjectionSize);
        }
    }
}