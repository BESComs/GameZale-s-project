using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Num_With_MiniGames
{
    [Serializable]
    public class NumFindObjects : IValidatable
    {
        [SerializeField] private string textString;
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Position2D backgroundPosition;
        [SerializeField] private GameObject counterPrefab;
        [SerializeField] private List<Sprite> _sprites;
        [SerializeField] private List<Position2D> _positions;

        private GameObject parentObject;
        private int count;
        
        public void OnValidate()
        {              
            if (_positions.Count > 0 && _positions.Count >= _sprites.Count)
            {                    
                List<Position2D> tmp = new List<Position2D>();
                bool emptyError = false;
                for (int i = 0; i < _sprites.Count; i++)
                {
                    tmp.Add(_positions[i]);
                    emptyError = _sprites[i] == null;
                }
                _positions.Clear();
                _positions = tmp;
                
                if (emptyError)
                    throw new Exception($"Not all Sprites of {GetType().Name} are set.");
            }
            
            else
            {
                for (int i = 0, positionsCount = _positions.Count; i < _sprites.Count - positionsCount; i++)
                {
                    _positions.Add(new Position2D());
                }
            }
        }

        public async void Init()
        {
            count = 0;
            parentObject = new GameObject("Find Objects");
            parentObject.transform.SetParent(gameController.transform);
            gameController.TextObject.text = textString;
            Color color = gameController.TextObject.color;
            color.a = 1f;
            gameController.TextObject.color = color;
            
            GameObject background = new GameObject("Background Sprite");
            
            if (backgroundSprite != null)
            {
                background.AddComponent<SpriteRenderer>().sprite = backgroundSprite;
                background.GetComponent<SpriteRenderer>().sortingLayerName = "Interactive Shapes";
            }
            
            background.transform.SetParent(parentObject.transform);
            background.transform.localPosition = new Vector3(backgroundPosition.X, backgroundPosition.Y);

            for (int i = 0; i < _sprites.Count; i++)
            {
                GameObject ob = new GameObject("Sprite " + (i + 1));
                ob.AddComponent<SpriteRenderer>().sprite = _sprites[i];
                ob.GetComponent<SpriteRenderer>().sortingLayerName = "Empty Shapes";
                ob.AddComponent<Clickable>();
                
                ob.GetComponent<Clickable>().SetCallback(async () =>
                {
                    GameObject counter = GameObject.Instantiate(counterPrefab);
                    count++;
                    counter.GetComponentInChildren<TextMeshPro>().text = count.ToString();
                    counter.transform.SetParent(ob.transform, false);
                    ob.GetComponent<Clickable>().DisableClick();
                    AnimationUtility.ScaleIn(counter, maxScale: counter.transform.localScale.x, speed: 2.5f);
                    gameController.PlayCorrectAnswerParticle(ob);

                    if (_sprites.Count == count)
                    {
                        await Awaiters.Seconds(1.5f);
                        gameController.SetNextButtonEnabled(true);
                    }
                });
                
                ob.transform.SetParent(background.transform);
                ob.transform.localPosition = new Vector3(_positions[i].X, _positions[i].Y);
            }

            background.transform.localEulerAngles = new Vector3(0, 90, 0);
            
            AnimationUtility.ScaleIn(gameController.TextObject.gameObject, 0.8f);
            await AnimationUtility.Rotate(background, Vector3.zero, 0.8f);
        }

        public async Task Finish()
        {
            AnimationUtility.ScaleOut(gameController.TextObject.gameObject, 1.5f);
            await AnimationUtility.ScaleOut(parentObject, 1.5f);
            GameObject.Destroy(parentObject);
        }
        
        private NumWMGGameController gameController => NumWMGGameController.Instance;
    }
}