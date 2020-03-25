using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Work_Directory.Bobur._Scripts.IQSha_Games.Games.Card_Match
{
    public class ScaleTo : ISequentTask
    {
        public Func<Task> RunTask { get; }

        public ScaleTo(GameObject animatedObj, Vector2 targetScale, AnimationCurve animationCurve = null)
        {
            animationCurve = animationCurve ?? AnimationCurve.EaseInOut(0, 0, 1, 1);
            RunTask = async () =>
            {
                var timer = animationCurve.keys[0].time;
                var endTime = animationCurve.keys[animationCurve.keys.Length - 1].time;
                Vector2 startScale = animatedObj.transform.localScale;
                var maxValue = animationCurve.keys[animationCurve.keys.Length - 1].value;
            
                while (timer < endTime)
                {
                    timer += Time.deltaTime;
                    animatedObj.transform.localScale =
                        Vector2.Lerp(startScale, targetScale, animationCurve.Evaluate(timer) / maxValue);
                    await new WaitForUpdate();
                }
            };
        }
    }

    public class ScaleIn: ISequentTask
    {
        public Func<Task> RunTask { get; }
    
        public ScaleIn(Transform animatedObject, AnimationCurve curve)
        {
            RunTask = async () =>
            {
                if(animatedObject == null) return;
                var startScale = animatedObject.localScale;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                var timer = curve.keys[curve.keys.Length - 1].time;
                animatedObject.gameObject.SetActive(true);
            
                while (timer > 0)
                {
                    if(animatedObject == null) return;
                    timer -= Time.deltaTime;
                    var scale = 1 - curve.Evaluate(timer) / endValue;
                    animatedObject.localScale = startScale * scale;
                    await new WaitForUpdate();
                }
                animatedObject.localScale = startScale;
            };
        }
    }

    public class Turn: ISequentTask
    {
        public Func<Task> RunTask { get; }

        public Turn(Transform animatedObject, AnimationCurve curve)
        {
            RunTask = async () =>
            {
                var timer = curve.keys[0].time;
                var endTime = curve.keys[curve.keys.Length - 1].time;
                var startScale = animatedObject.localScale;
                while (timer < endTime * 2)
                {
                    timer += Time.deltaTime;
                    var tmp = animatedObject.localScale;
                    if (timer > endTime)
                    {
                        tmp.x = startScale.x * curve.Evaluate(timer - endTime);
                        animatedObject.localScale = tmp;
                        await new WaitForUpdate();    
                        continue;
                    }
                    tmp.x = startScale.x * curve.Evaluate(endTime - timer);
                    animatedObject.localScale = tmp;
                    await new WaitForUpdate();
                }
            };
        }
    }

    public class ScaleOut: ISequentTask
    {
        public Func<Task> RunTask { get; }
    
        public ScaleOut(Transform animatedObject, AnimationCurve curve)
        {
            RunTask = async () =>
            {
                if(animatedObject == null) return;
                var startScale = animatedObject.localScale;
                var timer = curve.keys[curve.keys.Length - 1].time;
                var endValue =curve.keys[curve.keys.Length - 1].value;
                while (timer > 0)
                {
                    if(animatedObject == null) return;
                    timer -= Time.deltaTime;
                    var scale = curve.Evaluate(timer) / endValue;
                    animatedObject.localScale = startScale * scale;
                    await new WaitForUpdate();
                }
                animatedObject.localScale = startScale;
                animatedObject.gameObject.SetActive(false);
            };
        }
    }

    public class FadeIn : ISequentTask
    {
        public Func<Task> RunTask { get; }

        public FadeIn(Component animatedObject, AnimationCurve curve)
        {
            RunTask = async () =>
            {
                if(animatedObject == null) return;
                animatedObject.gameObject.SetActive(true);
                var rendS = animatedObject.GetComponent<Renderer>();
                var rendC = animatedObject.GetComponent<CanvasRenderer>();
                var canvasElem = rendC != null;
                var timer = curve.keys[curve.keys.Length - 1].time;
                var maxValue = curve.keys[curve.keys.Length - 1].value;
                while (timer > 0)
                {
                    if(animatedObject == null) return;
                    if(canvasElem)
                        rendC.SetAlpha(curve.Evaluate(timer)/maxValue);
                    else
                    {
                        timer -= Time.deltaTime;
                        var tmpColor = rendS.material.color;
                        tmpColor.a = 1 - curve.Evaluate(timer) / maxValue;
                        rendS.material.color = tmpColor;
                    }
                    await new WaitForUpdate();
                }
            };
        }
    }

    public class Bounce: ISequentTask
    {
        public Func<Task> RunTask { get; }

        public Bounce(Component animatedObject, AnimationCurve curve, Vector2 destScale)
        {
            RunTask = async () => 
            {
                var timer = curve.keys[0].time;
                var endTime = curve.keys[curve.keys.Length - 1].time;
                if(animatedObject == null || animatedObject.gameObject == null) return;
                var sr = animatedObject.GetComponent<SpriteRenderer>();
                var rt = animatedObject.GetComponent<RectTransform>();
                var startScale = sr == null ? rt.sizeDelta : sr.size;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                while (timer < endTime)
                {
                    timer += Time.deltaTime;
                    var m = curve.Evaluate(timer) * Mathf.Pow(1 + curve.Evaluate(endTime - timer) / endValue,3);
                    if(animatedObject == null || animatedObject.gameObject == null) return;
                    if (sr != null)
                        sr.size = destScale * m + startScale * (1 - m);
                    else
                        rt.sizeDelta = destScale * m + startScale * (1 - m);
                    await new WaitForUpdate();
                }
            };
        }
    }

    public class Shaking : ISequentTask
    {
        public Func<Task> RunTask { get; }

        public Shaking(Transform animatedObject, AnimationCurve curve, Vector2 shakingVector, float force)
        {
            RunTask = async () =>
            {
            
                if(animatedObject == null) return;
                var startPor = animatedObject.position;
                shakingVector = shakingVector.normalized;
                var timer = curve.keys[curve.keys.Length - 1].time;
                var counter = 0;
                while (timer > 0)
                {
                
                    if(animatedObject == null) return;
                    counter++;
                    timer -= Time.deltaTime;
                    var tmp = curve.Evaluate(timer);
                    var shake = force * Time.deltaTime * Math.Min(tmp, 1 - tmp) * shakingVector;
                    if (counter == 6) counter = 0;
                    if (counter < 3)
                        animatedObject.position += new Vector3(shake.x, shake.y, 0);
                    else if (counter < 6)
                        animatedObject.position -= new Vector3(shake.x, shake.y, 0);
                    await new WaitForUpdate();
                }

                animatedObject.position = startPor;
            };
        }
    }

    public class Jump : ISequentTask
    {
        public Func<Task> RunTask { get; }

        public Jump(Transform animatedObject, AnimationCurve curve, Vector3 destPos, float height)
        {
            RunTask = async () =>
            {
                var startPos = animatedObject.position;
                var timer = curve.keys[curve.keys.Length - 1].time;
                var startTime = timer;
                var perp = new Vector3(destPos.y - startPos.y, startPos.x - destPos.x, 0).normalized * height;
                while (timer > 0)
                {
                    animatedObject.position = startPos + destPos * curve.Evaluate(startTime - timer) -
                                              curve.Evaluate(timer) * curve.Evaluate(startTime - timer) * perp;
                
                    timer -= Time.deltaTime;
                    await new WaitForUpdate();
                }
            };
        }
    }

    public class FadeOut : ISequentTask
    {
        public Func<Task> RunTask { get; }

        public FadeOut(Component animatedObject, AnimationCurve curve)
        {
            RunTask = async () =>
            {
            
                if(animatedObject == null) return;
                animatedObject.gameObject.SetActive(true);
                var timer = curve.keys[curve.keys.Length - 1].time;
                var maxValue = curve.keys[curve.keys.Length - 1].value;
                var rendS = animatedObject.GetComponent<Renderer>();
                var rendC = animatedObject.GetComponent<CanvasRenderer>();
                var canvasElem = rendC != null;
                while (timer > 0)
                {
                
                    if(animatedObject == null) return;
                    timer -= Time.deltaTime;
                    if(canvasElem)
                        rendC.SetAlpha(curve.Evaluate(timer)/maxValue);
                    else
                    {
                        var tmpColor = rendS.material.color;
                        tmpColor.a = curve.Evaluate(timer) / maxValue;
                        rendS.material.color = tmpColor;    
                    }
                    await new WaitForUpdate();
                }
                animatedObject.gameObject.SetActive(false);
            };
        }
    }

    public class Scaling : ISequentTask
    {
        public Func<Task> RunTask { get; }

        public Scaling(Transform animatedObject, AnimationCurve curve)
        {
            RunTask = async () =>
            {
                var timer = curve.keys[curve.keys.Length - 1].time;
                var startTime = timer;
                var startScale = animatedObject.localScale;
            
                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    animatedObject.localScale = startScale * (1 + curve.Evaluate(timer) * curve.Evaluate(startTime - timer));
                    await new WaitForUpdate();
                }
            };
        }
    }

    public class Move : ISequentTask
    {
        public Func<Task> RunTask { get; }

        public Move(Transform animatedObject, AnimationCurve curve, Vector2 destPos)
        {
            RunTask = async () =>
            {
                var timer = curve.keys[curve.keys.Length - 1].time;
                Vector2 startPos = animatedObject.position;
                while (timer > 0)
                {
                    if(animatedObject == null) return;
                    timer -= Time.deltaTime;
                    animatedObject.position = destPos - curve.Evaluate(timer) * (destPos - startPos);
                    await new WaitForUpdate();
                }
            };
        }
    }

    public class ShakeRotate : ISequentTask
    {
        public Func<Task> RunTask { get; }

        public ShakeRotate(Transform animatedObject, AnimationCurve curve, float angle)
        {
            RunTask = async () =>
            {
                var timer = curve.keys[curve.keys.Length - 1].time;
                var startTime = timer;
                var startRotation = animatedObject.rotation.eulerAngles;
                var counter = 0;
                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    counter++;
                    var animatedObjectRotation = animatedObject.rotation;

                    if (counter == 6)
                        counter = 0;
                
                    if (counter < 3)
                        animatedObjectRotation.eulerAngles =
                            startRotation + new Vector3(0, 0, angle * curve.Evaluate(startTime - timer) * curve.Evaluate(timer));
                
                    else if(counter < 6)
                        animatedObjectRotation.eulerAngles =
                            startRotation - new Vector3(0, 0, angle * curve.Evaluate(startTime - timer) * curve.Evaluate(timer));
                
                    animatedObject.rotation = animatedObjectRotation;
                    await new WaitForUpdate();
                }
            };
        }
    }

    public class Rotate : ISequentTask
    {
        public Func<Task> RunTask { get; }

        public Rotate(Transform animatedObject, AnimationCurve curve, float angle)
        {
            RunTask = async () =>
            {
                var timer = curve.keys[curve.keys.Length - 1].time;
                var startTime = timer;
                var startRotation = animatedObject.rotation.eulerAngles;
                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    var animatedObjectRotation = animatedObject.rotation;
                    animatedObjectRotation.eulerAngles = startRotation + new Vector3(0,0,curve.Evaluate(startTime - timer) * angle);
                    animatedObject.rotation = animatedObjectRotation;
                    await new WaitForUpdate();
                }
            };
        }
    }
}