using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using _Scripts.Utility;
using _Scripts.Utility.Drag;

namespace _Scripts.Num_With_MiniGames
{
    [Serializable]
    public class CompleteJigsawGame : IValidatable
    {
        [SerializeField] private string textString;
        [SerializeField] private Position2D textPosition;
        
        [Header("Pattern")]
        [SerializeField] private Sprite patternSprite;
        [SerializeField] private Position2D patternPosition;

        [Header("Pieces")]
        [SerializeField] private GameObject _piecePrefab;
        [SerializeField] private List<Sprite> _sprites;
        [SerializeField] private List<Position2D> _spritePositions;
        [SerializeField] private List<Position2D> _patternPiecePositions;

        private GameObject parentObject;
        private NumWMGGameController gameController => NumWMGGameController.Instance;

        private int score;
        
        public void OnValidate()
        {
            if ((_spritePositions.Count > 0 || _patternPiecePositions.Count > 0) && _spritePositions.Count >= _sprites.Count)
            {                    
                List<Position2D> tmpSpritePositions = new List<Position2D>();
                List<Position2D> tmpPatternPiecesPositions = new List<Position2D>();
                bool emptyError = false;
                for (int i = 0; i < _sprites.Count; i++)
                {
                    tmpSpritePositions.Add(_spritePositions[i]);
                    tmpPatternPiecesPositions.Add(_patternPiecePositions[i]);
                    emptyError = _sprites[i] == null;
                }
                _spritePositions.Clear();
                _spritePositions = tmpSpritePositions;
                
                _patternPiecePositions.Clear();
                _patternPiecePositions = tmpPatternPiecesPositions;
                
                if (emptyError)
                    throw new Exception($"Not all _sprites of {GetType().Name} are set.");
            }
            
            else
            {
                for (int i = 0, positionsCount = _spritePositions.Count; i < _sprites.Count - positionsCount; i++)
                {
                    _spritePositions.Add(new Position2D());
                    _patternPiecePositions.Add(new Position2D());
                }
            }
        }

        public async Task Init(GameObject parent)
        {
            score = 0;
            parentObject = parent;
            gameController.TextObject.text = textString;
            gameController.TextObject.transform.localPosition = new Vector3(textPosition.X, textPosition.Y);
            
            GameObject pattern = new GameObject("Pattern");
            pattern.AddComponent<SpriteRenderer>().sprite = patternSprite;
            pattern.GetComponent<SpriteRenderer>().sortingLayerName = "Empty Shapes";
            
            pattern.transform.SetParent(parentObject.transform);
            pattern.transform.localPosition = new Vector3(patternPosition.X, patternPosition.Y);

            for (int i = 0; i < _sprites.Count; i++)
            {
                GameObject ob = GameObject.Instantiate(_piecePrefab);
                
                ob.GetComponent<JigsawPiece>().SetParentJigsaw(this);
                ob.AddComponent<SpriteRenderer>().sprite = _sprites[i];
                ob.GetComponent<SpriteRenderer>().sortingLayerName = "Interactive Shapes";
                ob.GetComponent<JigsawPiece>().RefreshColliderSize(ob.GetComponent<SpriteRenderer>().size);
                
                ob.GetComponent<ID>().Id = i;
                
                ob.transform.SetParent(parentObject.transform);
                ob.transform.localPosition = new Vector3(_spritePositions[i].X, _spritePositions[i].Y);

                GameObject piecePosition = GameObject.Instantiate(ob);
                piecePosition.GetComponent<SpriteRenderer>().sprite = null;
                GameObject.Destroy(piecePosition.GetComponent<DraggableReturn>());
                GameObject.Destroy(piecePosition.GetComponent<Rigidbody2D>());
                
                piecePosition.transform.SetParent(pattern.transform);
                piecePosition.transform.localPosition = new Vector3(_patternPiecePositions[i].X, _patternPiecePositions[i].Y);
                piecePosition.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);
                
                AnimationUtility.FadeIn(ob.GetComponent<SpriteRenderer>());
            }

            AnimationUtility.FadeIn(gameController.TextObject.renderer);
            await AnimationUtility.FadeIn(pattern.GetComponent<SpriteRenderer>());
        }

        public async Task Finish()
        {
            AnimationUtility.ScaleOut(gameController.TextObject.gameObject);
            await AnimationUtility.ScaleOut(parentObject);
            GameObject.Destroy(parentObject);
        }

        public void IncrementScore()
        {
            score++;
            if (score == _sprites.Count)
            {
                gameController.SetNextButtonEnabled(true);
            }
        }
    }
}