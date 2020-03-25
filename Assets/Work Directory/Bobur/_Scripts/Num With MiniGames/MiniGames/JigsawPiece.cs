using UnityEngine;
using _Scripts.Utility;

namespace _Scripts.Num_With_MiniGames
{
    public class JigsawPiece : MonoBehaviour
    {
        private int _id;
        private ID collidedObject;

        private CompleteJigsawGame parent;
        
        private void Start()
        {
            _id = GetComponent<ID>().Id;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<ID>() != null)
            {
                collidedObject = other.gameObject.GetComponent<ID>();
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            collidedObject = null;
        }

        private void OnMouseUp()
        {
            if (collidedObject != null && collidedObject.Id == _id)
            {
                collidedObject.gameObject.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                parent.IncrementScore();
                NumWMGGameController.Instance.PlayCorrectAnswerParticle(collidedObject.gameObject);
                Destroy(GetComponent<SpriteRenderer>());
            }
        }

        public void RefreshColliderSize(Vector3 size)
        {
            GetComponent<BoxCollider2D>().size = size;
        }
        
        public void SetParentJigsaw(CompleteJigsawGame parent) => this.parent = parent;
    }
}