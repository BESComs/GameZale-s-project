using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Num_With_MiniGames
{
    [Serializable]
    public class FindCubesGame : IValidatable
    {    
        [SerializeField] private string textString;
        [SerializeField] private Sprite correctAnswerSprite;
        [SerializeField] private Position2D textPosition;
        [SerializeField] private int _correctAnswer;
        [SerializeField] private List<SpriteWithId> sprites;
        [SerializeField] private List<Position2D> _positions;
        
        private List<Clickable> _objects;
        private TextMeshPro textObject;
        private GameObject parentObject;
        private int score = 0;
        private int total = 0;
        
        public void OnValidate()
        {              
            if (_positions.Count > 0 && _positions.Count >= sprites.Count)
            {                    
                List<Position2D> tmp = new List<Position2D>();
                bool emptyError = false;
                for (int i = 0; i < sprites.Count; i++)
                {
                    tmp.Add(_positions[i]);
                    emptyError = sprites[i] == null;
                }
                _positions.Clear();
                _positions = tmp;
                
                if (emptyError)
                    throw new Exception($"Not all Sprites of {GetType().Name} are set.");
            }
            
            else
            {
                for (int i = 0, positionsCount = _positions.Count; i < sprites.Count - positionsCount; i++)
                {
                    _positions.Add(new Position2D());
                }
            }
        }

        public async void Init(GameObject parent)
        {
            _objects = new List<Clickable>();
            textObject = NumWMGGameController.Instance.TextObject;
            textObject.text = "";
            
            for (int i = 0; i < sprites.Count; i++)
            {
                Clickable ob = gameController.Create(gameController.ClickablePrefab).GetComponent<Clickable>();
                ob.GetComponent<SpriteRenderer>().sprite = sprites[i].Sprite;
                ob.GetComponent<SpriteRenderer>().sortingOrder = i;
                ob.transform.localPosition = new Vector3(_positions[i].X, _positions[i].Y);
                
                if (ob.Collider2D is BoxCollider2D d)
                {
                    d.size = sprites[i].Sprite.bounds.size;
                }
                
                ob.transform.SetParent(parent.transform);
                ob.gameObject.AddComponent<ID>().Id = sprites[i].Id;

                total += (sprites[i].Id == _correctAnswer) ? 1 : 0;

                ob.SetCallback(() =>
                {
                    if (ob.GetComponent<ID>().Id == _correctAnswer)
                    {
                        GameObject correctAnswerSign = new GameObject("Correct sign");
                        correctAnswerSign.transform.SetParent(ob.transform);
                        AnimationUtility.ScaleIn(correctAnswerSign, 3f);
                        
                        correctAnswerSign.AddComponent<SpriteRenderer>().sprite = correctAnswerSprite;
                        correctAnswerSign.GetComponent<SpriteRenderer>().sortingLayerName = "Top Layer";
                        correctAnswerSign.transform.localPosition = new Vector3(0.45f, 0.45f);
                        ob.Collider2D.enabled = false;

                        gameController.PlayCorrectAnswerParticle(ob.gameObject, ob.GetComponent<SpriteRenderer>().size);

                        score++;
                        if (score == total)
                        {
                            gameController.SetNextButtonEnabled(true);
                        }
                    }

                });
                
                await AnimationUtility.ScaleIn(ob.gameObject, 1.5f);
                
                _objects.Add(ob);
            }
            
            
            textObject.gameObject.SetActive(true);
            textObject.transform.localPosition = new Vector3(textPosition.X, textPosition.Y);
            textObject.text = textString;
            
            await AnimationUtility.ScaleIn(textObject.gameObject);
        }

        public async Task Finish()
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                AnimationUtility.FadeOut(_objects[i].GetComponent<SpriteRenderer>());
                
                
                if (_objects[i].transform.childCount > 0 && _objects[i].transform.GetChild(0).GetComponent<Renderer>() != null)
                {
                    AnimationUtility.FadeOut(_objects[i].transform.GetChild(0).GetComponent<Renderer>());    
                }
                
            }
            await AnimationUtility.FadeOut(gameController.TextObject.renderer);
            
        }

        private NumWMGGameController gameController => NumWMGGameController.Instance;
    }

    [Serializable]
    internal class SpriteWithId
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _id;
        
        public Sprite Sprite => _sprite;
        public int Id => _id;
    }
}