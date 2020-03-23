using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DefaultNamespace;
using UnityEngine;
using Random = System.Random;

namespace _Scripts.PictureMatchGame
{
    [Serializable]
    public class PictureMatchGame : ITask
    {
        [SerializeField] private SpriteObject[] _pieces;
        [SerializeField] private int[] CorrectMatches;
        [SerializeField] private bool _mixWithOthers;
        
        private GameObject _piecesParentObject;
        private List<PicturePiece> _pieceGameObjects = new List<PicturePiece>();

        public void SetupShapes()
        {
            _piecesParentObject = new GameObject("Shapes Parent");

            for (int i = 0; i < _pieces.Length; i++)
            {
                GameObject pieceObject = new GameObject($"Piece {i}");

                ShufflePieces();
                pieceObject.AddComponent<PicturePiece>().AddSpriteObjects(_pieces);
                _pieceGameObjects.Add(pieceObject.GetComponent<PicturePiece>());

                Vector3 piecePosition = new Vector3(PictureMatchGameController.Instance.GetPositions()[i].X, PictureMatchGameController.Instance.GetPositions()[i].Y);
                pieceObject.transform.position = piecePosition;
                pieceObject.transform.SetParent(_piecesParentObject.transform);
            }
        }

        public GameObject GetPiecesParentObject()
        {
            return _piecesParentObject;
        }
        
        public int[] GetCorrectMatches()
        {
            return CorrectMatches;
        }
        
        private void ShufflePieces()
        {
            RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
            byte[] randomInt = new byte[10];
            rnd.GetBytes(randomInt);
            
            Random random = new Random();
            _pieces = _pieces.OrderBy(x => Convert.ToInt32(randomInt[random.Next(10)]) * random.Next(30)).ToArray();
        }

        public void TaskCompleted()
        {
            throw new NotImplementedException();
        }

        public bool CheckTaskComplete()
        {
            for (int i = 0; i < _pieceGameObjects.Count; i++)
            {
                if (_pieceGameObjects[i].GetPieceId() != CorrectMatches[i])
                    return false;
            }

            foreach (var pieceGameObject in _pieceGameObjects)
            {
                pieceGameObject.Finished();
            }
            
            return true;
        }
    }

    class PicturePiece : MonoBehaviour
    {
        private List<SpriteObject> _sprites;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _boxCollider2D;
        private int currentSpriteIndex;
        private bool finished;
        
        private void Start()
        {
            _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            _spriteRenderer.sortingLayerName = "Interactive Shapes";
            currentSpriteIndex = -1;
            finished = false;
            NextSpriteObject();
            //_boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
            //_boxCollider2D.size = _spriteRenderer.bounds.size;
            gameObject.AddComponent<BoxCollider2D>();
        }

        private void OnMouseDown()
        {
            if (!finished)
            {
                NextSpriteObject();                
            }
        }

        public void AddSpriteObjects(SpriteObject[] spriteObject)
        {
            _sprites = new List<SpriteObject>(spriteObject);
        }

        public void NextSpriteObject()
        {
            currentSpriteIndex++;
            if (currentSpriteIndex >= _sprites.Count)
            {
                currentSpriteIndex = 0;
            }

            _spriteRenderer.sprite = _sprites[currentSpriteIndex].GetSprite();
            PictureMatchGameController.Instance.CheckCurrentGame();
        }

        public int GetPieceId()
        {
            return _sprites[currentSpriteIndex].GetId();
        }

        public void Finished()
        {
            finished = true;
        }
    }
}
