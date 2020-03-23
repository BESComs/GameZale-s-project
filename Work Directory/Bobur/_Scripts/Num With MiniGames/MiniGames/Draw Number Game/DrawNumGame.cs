using System;
using System.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Num_With_MiniGames.MiniGames.Draw_Number_Game
{
    [Serializable]
    public class DrawNumGame : ITask
    {
        [SerializeField] private string textString;
        [SerializeField] private Position2D textPosition;

        [SerializeField] private GameObject _targetPadPrefab;
        [SerializeField] private Position2D _targetPadPosition;
        [SerializeField] private GameObject _padPrefab;
        [SerializeField] private Position2D _padPosition;

        private GameObject _targetPad;
        private GameObject _pad;
        
        private GameObject parentObject;
        
        public async Task Init(GameObject parent)
        {
            parentObject = parent;
            
            _targetPad = GameObject.Instantiate(_targetPadPrefab);
            _pad = GameObject.Instantiate(_padPrefab);
            
            _targetPad.transform.SetParent(parentObject.transform);
            _pad.transform.SetParent(parentObject.transform);
            
            _targetPad.transform.localPosition = new Vector3(_targetPadPosition.X, _targetPadPosition.Y);
            _pad.transform.localPosition = new Vector3(_padPosition.X, _padPosition.Y);
            
            foreach (Transform cell in _targetPad.transform)
            {
                cell.GetComponent<PaintCell>()?.SetColliderEnabled(false);
            }

            for (int i = 0; i < _pad.transform.childCount; i++)
            {
                PaintCell cell = _pad.transform.GetChild(i).GetComponent<PaintCell>();
                cell.GetClickable().SetCallback(() =>
                {
                    cell.OnClick();

                    if (CheckTaskComplete())
                    {
                        TaskCompleted();
                    }
                });
            }
            
            gameController.TextObject.text = textString;
            gameController.TextObject.transform.localPosition = new Vector3(textPosition.X, textPosition.Y);
            AnimationUtility.ScaleIn(gameController.TextObject.gameObject);
            await AnimationUtility.ScaleIn(parentObject);
        }

        public void TaskCompleted()
        {
            foreach (Transform cell in _pad.transform)
            {
                cell.GetComponent<PaintCell>().SetColliderEnabled(false);
            }
            
            Vector3 particleSize = new Vector3((_padPrefab.transform as RectTransform).rect.width, (_padPrefab.transform as RectTransform).rect.height);
            gameController.PlayCorrectAnswerParticle(_pad, particleSize);
            gameController.SetNextButtonEnabled(true);
        }

        public bool CheckTaskComplete()
        {
            for (int i = 0; i < _targetPad.transform.childCount; i++)
            {
                PaintCell targetCell = _targetPad.transform.GetChild(i).GetComponent<PaintCell>();
                PaintCell padCell = _pad.transform.GetChild(i).GetComponent<PaintCell>();

                if (targetCell.IsPainted != padCell.IsPainted)
                    return false;
            }

            return true;
        }

        public async Task Finish()
        {
            AnimationUtility.ScaleOut(gameController.TextObject.gameObject);
            await AnimationUtility.ScaleOut(parentObject);
        }
        
        private NumWMGGameController gameController => NumWMGGameController.Instance;
    }
}