using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] GameObject _damageTextPrefab;

        public void Spawn(float damage)
        {
            GameObject damageText = Instantiate(_damageTextPrefab, transform);
            damageText.GetComponentInChildren<Text>().text = String.Format("{0:0}", damage);
        }
    }
}