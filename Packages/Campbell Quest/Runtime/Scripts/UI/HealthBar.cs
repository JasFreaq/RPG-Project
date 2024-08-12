using Campbell.Attributes;
using UnityEngine;

namespace Campbell.UI
{
    public class HealthBar : MonoBehaviour
    {
        float _healthPoints, _totalHealthPoints, _healthBarPercentage;
        Health _health;

        [SerializeField] RectTransform _healthBar;
        [SerializeField] Canvas _canvas;

        private void Awake()
        {
            _health = GetComponentInParent<Health>();
        }

        void Update()
        {
            _healthPoints = _health.GetHealth();
            _totalHealthPoints = _health.GetTotalHealth();

            _healthBarPercentage = (_healthPoints / _totalHealthPoints);

            _healthBar.localScale = new Vector3(_healthBarPercentage, 1, 1);

            if (Mathf.Approximately(_healthBarPercentage, 0) || Mathf.Approximately(_healthBarPercentage, 1))
                _canvas.enabled = false;
            else
                _canvas.enabled = true;
        }
    }
}