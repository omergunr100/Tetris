﻿using System.Text;
using Management;
using Management.Board;
using Management.Score;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InGameScoreBoard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textElement;
        
        private readonly StringBuilder _builder = new();
        private const string Template = "Score: ";
        
        private void Awake()
        {
            ScoreManager.Instance.AddScoreChangeListener(UpdateScore);
        }

        private void UpdateScore(int newScore) => textElement.text = _builder.Clear().Append(Template).Append(newScore)
            .ToString();
    }
}