using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using _Scripts.Num_With_MiniGames.MiniGames.Draw_Number_Game;
using _Scripts.Utility;

namespace _Scripts.Num_With_MiniGames
{
    [Serializable]
    public class NumMiniGames
    {
        [SerializeField] private MiniGameType type;
        [SerializeField] private FindCubesGame _findCubesGame;
        [SerializeField] private CompleteJigsawGame _completeJigsawGame;
        [SerializeField] private ClickAndRotateGame _clickAndRotateGame;
        [SerializeField] private DrawNumGame _drawNumGame;

        private GameObject minigamesParent;
        
        public void Init()
        {
            gameController.TextObject.gameObject.SetActive(true);
            minigamesParent = new GameObject("Mini games");
            minigamesParent.transform.SetParent(NumWMGGameController.Instance.transform);
            
            switch (type)
            {
                case MiniGameType.FindCubes : _findCubesGame.Init(minigamesParent); break;
                case MiniGameType.CompleteJigsaw : _completeJigsawGame.Init(minigamesParent); break;
                case MiniGameType.ClickAndRotate : _clickAndRotateGame.Init(minigamesParent); break;
                case MiniGameType.DrawNum : _drawNumGame.Init(minigamesParent); break;
            }
        }
        
        public async void Finish()
        {
            switch (type)
            {
                case MiniGameType.FindCubes : await _findCubesGame.Finish(); break;
                case MiniGameType.CompleteJigsaw : await _completeJigsawGame.Finish(); break;
                case MiniGameType.ClickAndRotate : await _clickAndRotateGame.Finish(); break;
                case MiniGameType.DrawNum : await _drawNumGame.Finish(); break;
            }
            GameObject.Destroy(minigamesParent);
        }
        
        private class PaintNumsGame
        {
            
        }

        public IValidatable ChosenGame()
        {
            switch (type)
            {
                case MiniGameType.FindCubes : return _findCubesGame; 
                case MiniGameType.CompleteJigsaw : return _completeJigsawGame; 
            }

            return null;
        }
        
        private NumWMGGameController gameController => NumWMGGameController.Instance;
    }

    public interface IValidatable
    {
        void OnValidate();
    }
    
    public enum MiniGameType
    {
        FindCubes,
        CompleteJigsaw,
        ClickAndRotate,
        DrawNum,
        PaintNums
    }
}