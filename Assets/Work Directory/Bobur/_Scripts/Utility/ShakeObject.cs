using UnityEngine;

namespace DefaultNamespace
{
    public class ShakeObject : MonoBehaviour
    {
        private Vector3 startPosition;
        private float animTime;
        private AnimationCurve _animationCurve;
        private bool isShaking;
        private OptionGameController _optionGameController;

        private void Start()
        {
            Keyframe[] keys = new Keyframe[11];
            keys[0] = new Keyframe(0, 0);
            keys[1] = new Keyframe(0.1f, -1);
            keys[2] = new Keyframe(0.2f, 2);
            keys[3] = new Keyframe(0.3f, -4);
            keys[4] = new Keyframe(0.4f, 4);
            keys[5] = new Keyframe(0.5f, -4);
            keys[6] = new Keyframe(0.6f, 4);
            keys[7] = new Keyframe(0.7f, -4);
            keys[8] = new Keyframe(0.8f, 2);
            keys[9] = new Keyframe(0.9f, -1);
            keys[10] = new Keyframe(1f, 0);
		
            _animationCurve = new AnimationCurve(keys);
            startPosition = transform.localPosition;
        }
        
        private void Update()
        {
            if (isShaking) {
                animTime += Time.deltaTime / 0.9f;
                if (animTime >= 1)
                {
                    isShaking = false;
                    transform.localPosition = startPosition;
                    
                    if (_optionGameController != null)
                        _optionGameController.EnableCurrentQuestionColliders();
                    
                    Destroy(transform.GetComponent<ShakeObject>());
                }
                else
                {
                    float animPos = _animationCurve.Evaluate(animTime) / 10;
                    transform.localPosition = new Vector3(startPosition.x + animPos, transform.localPosition.y, transform.localPosition.z);
                }
            }
        }

        public void Shake(OptionGameController optionGameController)
        {
            _optionGameController = optionGameController;
            isShaking = true;
        }

        public void Shake()
        {
            isShaking = true;
        }
    }
}