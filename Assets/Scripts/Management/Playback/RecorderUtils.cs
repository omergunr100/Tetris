using System;
using Management.Board;
using Management.Playback.Actions;
using Management.Tetromino;
using Tetromino;
using UnityEngine;

namespace Management.Playback
{
    public static class RecorderUtils
    {
        public static void RecordMove(Vector3 direction) => 
            GameRecorder.Instance.RecordAction(new DataAction<Vector3>(direction, x => BoardManager.Instance.TryMove(x)));

        public static void RecordRotation() =>
            GameRecorder.Instance.RecordAction(new GameAction(() => BoardManager.Instance.TryRotate()));
        
        public static void RecordSpawn(TetrominoDefinition definition)
        {
            GameRecorder.Instance.RecordAction(
                new DataAction<TetrominoDefinition>(definition, TetrominoSpawner.Instance.SpawnDefinition));
        }

        public static void RecordEffect(Type effect, int index)
        {
            GameRecorder.Instance.RecordAction(
                new DataAction<(Type, int)>((effect, index), data =>
                {
                    var (type, idx) = data;
                    TetrominoSpawner.Instance.ApplyEffect(type, idx);
                }));
        }

        public static void RecordPutOnBoard() =>
            GameRecorder.Instance.RecordAction(new GameAction(TetrominoScript.Instance.Remove));
    }
}