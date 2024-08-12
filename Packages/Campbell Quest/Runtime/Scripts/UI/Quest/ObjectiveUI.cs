using TMPro;
using UnityEngine;

namespace Campbell.UI.Quests
{
    public class ObjectiveUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private GameObject _bullet;

        public void Setup(string description, bool status)
        {
            _description.text = description;
            _bullet.SetActive(status);
        }
    }
}