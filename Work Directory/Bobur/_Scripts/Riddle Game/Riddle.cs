using System;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Riddle_Game
{
    [Serializable]
    public class Riddle
    {
        [SerializeField]
        [TextArea]
        private string _question;
        
        [SerializeField]
        [TextArea]
        private string _answer;
        
        [SerializeField] private Option[] _options;
        [SerializeField] private int _correctAnswer;

        public string Question => _question;
        public string Answer => _answer;
        public Option[] Options => _options;
        public int CorrectAnswer => _correctAnswer;
    }
}
