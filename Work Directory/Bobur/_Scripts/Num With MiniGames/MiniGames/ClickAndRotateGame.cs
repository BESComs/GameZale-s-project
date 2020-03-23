using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Num_With_MiniGames
{
    [Serializable]
    public class ClickAndRotateGame
    {
        [SerializeField] private string textString;
        [SerializeField] private Position2D textPosition;
        [SerializeField] private List<ClickAndRotatePiece> _pieces;
        
        private GameObject _parentObject;
        private List<Clickable> _objects;
        
        public async Task Init(GameObject parent)
        {
            _parentObject = parent;
            _objects = new List<Clickable>();
            gameController.TextObject.text = textString;
            gameController.TextObject.transform.localPosition = new Vector3(textPosition.X, textPosition.Y);
            
            for (int i = 0; i < _pieces.Count; i++)
            {
                GameObject ob = new GameObject("Piece " + i +1);
                ob.AddComponent<SpriteRenderer>().sprite = _pieces[i].Sprite;
                ob.GetComponent<SpriteRenderer>().sortingLayerName = "Interactive Shapes";
                
                ob.transform.SetParent(_parentObject.transform);
                ob.transform.localPosition = new Vector3(_pieces[i].Position.X, _pieces[i].Position.Y);
                ob.transform.localEulerAngles = new Vector3(0f, 0f, _pieces[i].Id * 90f);
                
                ob.AddComponent<Clickable>().SetCallback(async () =>
                {
                    Vector3 targetRotation = ob.transform.localEulerAngles;
                    targetRotation.z -= 90f;
                    
                    ob.GetComponent<Clickable>().DisableClick();
                    await AnimationUtility.Rotate(ob, targetRotation, 1.5f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
                    ob.GetComponent<Clickable>().EnableClick();
                    
                    if (CheckTaskComplete())
                    {
                        foreach (Clickable clickable in _objects)
                        {
                            clickable.DisableClick();
                        }

                        Vector3 particleSize = Vector3.one;
                        particleSize.x = ob.GetComponent<SpriteRenderer>().size.x * 2;
                        particleSize.y = ob.GetComponent<SpriteRenderer>().size.y * 2;
                        await gameController.PlayCorrectAnswerParticle(_parentObject, particleSize);
                        gameController.SetNextButtonEnabled(true);
                    }
                });
                
                _objects.Add(ob.GetComponent<Clickable>());
                AnimationUtility.FadeIn(ob.GetComponent<SpriteRenderer>(), 1f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
            }

            await AnimationUtility.FadeIn(gameController.TextObject.renderer, 1f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
        }

        public async Task Finish()
        {
            AnimationUtility.ScaleOut(gameController.TextObject.gameObject, animCurve: AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
            await AnimationUtility.ScaleOut(_parentObject, animCurve: AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
        }

        private bool CheckTaskComplete()
        {
            foreach (Clickable gameObject in _objects)
            {
                if (Mathf.RoundToInt(gameObject.transform.localEulerAngles.z) != 0)
                    return false;
            }

            return true;
        }
        
        private NumWMGGameController gameController => NumWMGGameController.Instance;
    }

    [Serializable]
    class ClickAndRotatePiece
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _id;
        [SerializeField] private Position2D _position;

        public Sprite Sprite => _sprite;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public Position2D Position => _position;
    }
}