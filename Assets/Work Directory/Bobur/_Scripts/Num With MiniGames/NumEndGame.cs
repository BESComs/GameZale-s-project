using System;
using System.Threading.Tasks;
using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Num_With_MiniGames
{
    [Serializable]
    public class NumEndGame
    {
        [Header("Text Settings")]
        [SerializeField] private string textString;
        [SerializeField] private Position2D textPosition;
        [Header("Sprite Settings")]
        [SerializeField] private Sprite sprite;
        [SerializeField] private Position2D spriteStartPosition;
        [SerializeField] private Position2D spriteJumpPosition;
        [SerializeField] private float jumpSpeed;
        [SerializeField] private float jumpHeight;

        private GameObject parentObject;

        public async Task Init()
        {
            parentObject = new GameObject("End Game");
            parentObject.transform.SetParent(gameController.transform);
            
            
            GameObject numObject = new GameObject("Number");
            numObject.transform.localPosition = new Vector3(spriteStartPosition.X, spriteStartPosition.Y);
            numObject.AddComponent<SpriteRenderer>().sprite = sprite;
            numObject.GetComponent<SpriteRenderer>().sortingLayerName = "Interactive Shapes";
            numObject.transform.SetParent(parentObject.transform);
            
            gameController.TextObject.transform.localPosition = new Vector3(textPosition.X, textPosition.Y);
            gameController.TextObject.text = textString;
            gameController.TextObject.enabled = false;
            
            await AnimationUtility.JumpTo(numObject, new Vector3(spriteJumpPosition.X, spriteJumpPosition.Y), jumpHeight, jumpSpeed, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
            
            gameController.TextObject.enabled = true;
            await AnimationUtility.ScaleIn(gameController.TextObject.gameObject, animCurve: AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
            
            gameController.PlayEndGameParticle();
        }

        public async Task Finish()
        {
            GameObject.Destroy(parentObject);
            gameController.StopEndGameParticle();
        }
        
        private NumWMGGameController gameController => NumWMGGameController.Instance;
    }
}