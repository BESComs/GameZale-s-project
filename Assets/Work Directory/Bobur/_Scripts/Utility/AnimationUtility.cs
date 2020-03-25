using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace _Scripts.Utility
{
    public class AnimationUtility : MonoBehaviour
    {
        public static async Task ScaleIn(GameObject ob, float speed = 1f, AnimationCurve animCurve = null, float maxScale = 1f, float initialScale = 0f)
        {
            for (float i = 0; i < 1f;)
            {
                if(ob == null) break;
                
                i += Time.fixedDeltaTime * speed;
                i = (i > 1f) ? 1f : i;

                animCurve = animCurve ?? AnimationCurve.EaseInOut(0f, initialScale, 1f, maxScale);
                float animVal = animCurve.Evaluate(i);
                
                ob.transform.localScale = new Vector3(animVal, animVal, 1);
                await Awaiters.NextFrame;
            }
        }
        
        public static async Task ScaleOut(GameObject ob, float speed = 1f, AnimationCurve animCurve = null, float initialScale = 1f)
        {
            for (float i = 1f; i > 0;)
            {   
                if(ob == null) break;
                
                i -= Time.deltaTime * speed;
                i = (i < 0f) ? 0f : i;

                animCurve = animCurve ?? AnimationCurve.EaseInOut(0f, 0f, 1f, initialScale);
                float animVal = animCurve.Evaluate(i);
                
                ob.transform.localScale = new Vector3(animVal, animVal, 1);
                await Awaiters.NextFrame;
            }
        }

        public static async Task FadeIn(Renderer renderer, float speed = 1f, AnimationCurve animCurve = null)
        {
            Color originalColor;
            bool isTextMesh = false;
            
            if(renderer.gameObject.GetComponent<TextMeshPro>() != null)
            {
                originalColor = renderer.gameObject.GetComponent<TextMeshPro>().color;
                isTextMesh = true;
            }
            else
            {
                originalColor = renderer.material.color;
            }
            
            originalColor.a = 0f;
            
            Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
            for (float i = 0; i < 1f;)
            {
                if(renderer == null || renderer.gameObject == null) break;
                
                i += Time.fixedDeltaTime * speed;
                i = (i > 1f) ? 1f : i;

                animCurve = animCurve ?? AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                float animVal = animCurve.Evaluate(i);
                
                if (isTextMesh)
                {
                    renderer.gameObject.GetComponent<TextMeshPro>().color = Color.Lerp(originalColor, targetColor, animVal);
                }
                else
                {
                    renderer.material.color = Color.Lerp(originalColor, targetColor, animVal);
                }
                
                await Awaiters.FixedUpdate;
            }
        }
        
        public static async Task FadeOut(Renderer renderer, float speed = 1f, AnimationCurve animCurve = null)
        {
            Color originalColor;
            bool isTextMesh = false;
            
            if(renderer.gameObject.GetComponent<TextMeshPro>() != null)
            {
                originalColor = renderer.gameObject.GetComponent<TextMeshPro>().color;
                isTextMesh = true;
            }
            else
            {
                originalColor = renderer.material.color;
            }
            
            originalColor.a = 1f;
            
            Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            
            for (float i = 0f; i < 1f;)
            {
                if(renderer == null || renderer.gameObject == null) break;
                
                i += Time.fixedDeltaTime * speed;
                i = (i > 1f) ? 1f : i;

                animCurve = animCurve ?? AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                float animVal = animCurve.Evaluate(i);

                if (isTextMesh)
                {   
                    renderer.gameObject.GetComponent<TextMeshPro>().color = Color.Lerp(originalColor, targetColor, animVal);
                }
                else
                {
                    renderer.material.color = Color.Lerp(originalColor, targetColor, animVal);
                }
                
                await Awaiters.FixedUpdate;
            }
        }

        public static async Task MoveTo(GameObject ob, Vector3 target, float speed = 1f, AnimationCurve animCurve = null)
        {
            Vector3 startPos = ob.transform.localPosition;
            for (float i = 0; i < 1f;)
            {
                if(ob == null) break;
                
                i += Time.fixedDeltaTime * speed;
                i = (i > 1f) ? 1f : i;

                animCurve = animCurve ?? AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                float animValue = animCurve.Evaluate(i);

                ob.transform.localPosition = Vector3.Lerp(startPos, target, animValue);

                await Awaiters.FixedUpdate;
            }
        }
        
        public static async Task JumpTo(GameObject ob, Vector3 target, float jumpHeight = 1f, float speed = 1f, AnimationCurve animCurve = null)
        {
            Func<float, float> func = (x) => -4 * jumpHeight * x * x + 4 * jumpHeight * x;
            Vector3 startPos = ob.transform.localPosition;
            
            for (float i = 0f; i < 1f;)
            {
                if(ob == null) break;
                
                i += Time.fixedDeltaTime * speed;
                i = (i > 1f) ? 1f : i;
                
                

                animCurve = animCurve ?? AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                float animValue = animCurve.Evaluate(i); 
                Vector3 mid = Vector3.Lerp(startPos, target, animValue);
                
                ob.transform.transform.localPosition = new Vector3(mid.x, func(animValue) + Mathf.Lerp(startPos.y, target.y, animValue), mid.z);

                await Awaiters.FixedUpdate;
            }
        }

        public static async Task Rotate(GameObject ob, Vector3 targetRotation, float speed = 1f, AnimationCurve animCurve = null)
        {
            Vector3 startRotation = ob.transform.localEulerAngles;
            for (float i = 0; i < 1f;)
            {
                if(ob == null) break;
                
                i += Time.fixedDeltaTime * speed;
                i = (i > 1f) ? 1f : i;
                
                animCurve = animCurve ?? AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                float animValue = animCurve.Evaluate(i);
                
                Vector3 rotation = new Vector3(
                                        Mathf.LerpAngle(startRotation.x, targetRotation.x, animValue),
                                        Mathf.LerpAngle(startRotation.y, targetRotation.y, animValue),
                                        Mathf.LerpAngle(startRotation.z, targetRotation.z, animValue));

                ob.transform.localEulerAngles = rotation;

                await Awaiters.FixedUpdate;
            }
        }

        public static async Task RotateLoop(GameObject ob, Vector3 targetRotation, float speed = 1f,
            AnimationCurve animCurve = null)
        {
            Vector3 startRotation = ob.transform.localEulerAngles;
            
            while (true)
            {
                await Rotate(ob, targetRotation, speed);
                await Rotate(ob, startRotation, speed);
            }
        }
        
        public enum AnimationType
        {
            Fade,
            Scale
        }
    }
}
