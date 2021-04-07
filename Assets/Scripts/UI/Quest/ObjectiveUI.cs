﻿using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class ObjectiveUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private GameObject _bullet;

        public void Setup(string desc, bool status)
        {
            _description.text = desc;
            _bullet.SetActive(status);
        }
    }
}