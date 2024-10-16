﻿using System;
using Management;
using Management.Board;
using Management.Playback;
using Tetromino;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerController : Singleton<PlayerController>
    {
        [SerializeField] private float movementSpeed = 2f;
        private float ActualMovementSpeed() => movementSpeed + GameManager.Instance.GameSpeed();
        
        private float _timeSinceLastMove;
        private bool _listening = true;

        protected void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(phase =>
            {
                _timeSinceLastMove = 0;
                _listening = phase == GamePhase.Tetris;
            });
        }

        private void Update()
        {
            if (!_listening) return;
            
            _timeSinceLastMove += Time.deltaTime;
            
            if (GetRotationInput())
            {
                BoardManager.Instance.TryRotate(); 
                RecorderUtils.RecordRotation();
            }
            
            if (_timeSinceLastMove >= 1f / ActualMovementSpeed())
            {
                _timeSinceLastMove = 0f;
                if (!TetrominoScript.Instance.Removed)
                {
                    var movementInput = GetMovementInput();
                    BoardManager.Instance.TryMove(movementInput);
                    RecorderUtils.RecordMove(movementInput);
                }
                // todo: record movement
            }
        }

        private Vector3 GetMovementInput()
        {
            var result = Vector3.zero;
            
            if (Input.GetKey(KeyCode.DownArrow))
                result += Vector3.down;
            
            if (Input.GetKey(KeyCode.LeftArrow))
                result += Vector3.left;
            
            if (Input.GetKey(KeyCode.RightArrow))
                result += Vector3.right;
            
            return result;
        }
        
        private bool GetRotationInput() => Input.GetKeyDown(KeyCode.UpArrow);
    }
}