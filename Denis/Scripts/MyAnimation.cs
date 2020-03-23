using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Work_Directory.Denis.Scripts
{
    
    /*
     * Async animation for mini games
     */
    public interface ISequenceTask
    {
        Func<Task> RunTask { get; }
    }

    public enum Mode
    {
        In, Out
    }

    public class ToScale : ISequenceTask
    {
        public Func<Task> RunTask { get; }

        public ToScale(Component component, AnimationCurve curve, Vector2 scale)
        {
            RunTask = async delegate
            {
                var endTime = curve.keys[curve.keys.Length - 1].time;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                var timer = curve.keys[0].time;
                var startScale = component.transform.localScale;
                while (timer < endTime)
                {
                    timer += Time.deltaTime;
                    component.transform.localScale = startScale * (1 - curve.Evaluate(timer) / endValue) +
                                                     (Vector3)scale * curve.Evaluate(timer) / endValue;
                    await new WaitForUpdate();
                }

                component.transform.localScale = scale;
            };
        }
    }

    public class Scale : ISequenceTask
    {
        public Func<Task> RunTask { get; }

        public Scale(Component parent, bool parentOnly,bool startScaleOnEnd, Mode scaleInOut = Mode.Out, AnimationCurve curve = null)
        {
            RunTask = async delegate
            {
                if(curve == null)
                    curve = AnimationCurve.EaseInOut(0,0,1,1);
                var transforms = parent.GetComponentsInChildren<Transform>().ToList();
                var startScale = transforms.Select(transform => transform.localScale).Select(dummy => (Vector2) dummy).ToList();
                var timer = curve.keys[0].time;
                var endTime = curve.keys[curve.keys.Length - 1].time;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                var parentStartScale = parent.transform.localScale;
                while (timer < endTime)
                {
                    timer += Time.deltaTime;
                    if (parentOnly)
                    {
                        if (scaleInOut == Mode.Out)
                            parent.transform.localScale = parentStartScale * (1 - curve.Evaluate(timer) / endValue);
                        else if (scaleInOut == Mode.In)
                            parent.transform.localScale = parentStartScale * curve.Evaluate(timer / endValue);

                    }
                    else
                    {
                        for (var i = 0; i < transforms.Count; i++)
                        {
                            if (scaleInOut == Mode.Out)
                                transforms[i].localScale = startScale[i] * (1 - curve.Evaluate(timer) / endValue);
                            else if (scaleInOut == Mode.In)
                                transforms[i].localScale = startScale[i] * curve.Evaluate(timer / endValue);
                        }
                    }

                    await new WaitForUpdate();
                }

                if (startScaleOnEnd)
                {
                    if (parentOnly)
                        parent.transform.localScale = parentStartScale;
                    else
                    {
                        for (var i = 0; i < transforms.Count; i++)
                            transforms[i].localScale = startScale[i];
                    }
                }
            };
        }
    }

    public class Shake : ISequenceTask
    {
        public Func<Task> RunTask { get; }

        public Shake(Transform transform)
        {
            RunTask = async delegate { await new WaitForUpdate();};
        }
    }

    public class Rotate : ISequenceTask
    {
        public Func<Task> RunTask { get; }
    
        public Rotate(Transform transform)
        {
            RunTask = async delegate { await new WaitForUpdate();};
        }
    }


    public class CurveMove : ISequenceTask
    {
        public Func<Task> RunTask { get; }

        public CurveMove(Transform animObj, Vector2 desPos, AnimationCurve curve)
        {
            RunTask = async delegate
            {
                float journeyTime = 1f;
                float timer = 0;
                Vector2 sunrise = animObj.position;
                var delta = Mathf.Sign(desPos.x - animObj.position.x) ;
                var endTime = curve.keys[curve.keys.Length - 1].time;
                while ( timer < endTime)
                {
                    timer += Time.deltaTime;
                    var center = (sunrise + desPos) * 0.5F;

                    center -= new Vector2(4, 4);

                    var riseRelCenter = sunrise - center;
                    var setRelCenter = desPos - center;

                    var fracComplete = curve.Evaluate(timer) / journeyTime;

                    animObj.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
                    animObj.position += (Vector3)center;
                    await new WaitForUpdate();
                }
            };
        }
    }

    public class Move : ISequenceTask
    {
        public Func<Task> RunTask { get; }

        public Move(Transform animObj, Vector2 destPos, bool startPosOnEnd, AnimationCurve curve = null)
        {
            RunTask = async delegate
            {
                curve = curve ?? AnimationCurve.EaseInOut(0,0,1,1);
                var timer = 0f;
                var startPos = animObj.position;
                var endTime = curve.keys[curve.keys.Length - 1].time;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                while (timer < endTime)
                {
                    timer += Time.deltaTime;
                    animObj.position = (Vector2)startPos * (1 - curve.Evaluate(timer)/endValue) + destPos * curve.Evaluate(timer)/endValue;
                    await new WaitForUpdate();
                }

                if (startPosOnEnd)
                    animObj.position = startPos;
            };
        }
    }

    public class SmoothColoring : ISequenceTask
    {
        public Func<Task> RunTask { get; }

        public SmoothColoring(SpriteRenderer animObj, Color fromColor, Color toColor, AnimationCurve curve = null)
        {
            RunTask = async delegate
            {
                if(curve == null)
                    curve = AnimationCurve.EaseInOut(0,0,1,1);
                var timer = curve.keys[0].time;
                var endTime = curve.keys[curve.keys.Length - 1].time;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                await new WaitForSeconds(timer);
                while (timer < endTime)
                {
                    timer += Time.deltaTime;
                    var color = (fromColor * (endValue - curve.Evaluate(timer)) / endValue +
                                 toColor * curve.Evaluate(timer) / endValue);
                    animObj.color = color;
                    await new WaitForUpdate();
                }

                animObj.color = toColor;
            };
        }
    }

    public class Scaling : ISequenceTask
    {
        public Func<Task> RunTask { get; }
        public Scaling(Transform animObj, bool startScaleOnEnd, AnimationCurve curve = null)
        {

            RunTask = async delegate
            {
                curve = curve ?? AnimationCurve.EaseInOut(0, 0, 1, 1);
                var timer = 0f;
                var startScale = animObj.localScale;
                var endTime = curve.keys[curve.keys.Length - 1].time;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                while (timer < endTime)
                {
                    timer += Time.deltaTime;
                    if(animObj == null) return;
                    animObj.localScale = startScale * curve.Evaluate(timer);
                    await new WaitForUpdate();
                }
                if (startScaleOnEnd)
                    animObj.localScale = startScale;
                else
                    animObj.localScale = startScale * endValue;
            };
        }
    }

    public class ThrowOnScreen : ISequenceTask
    {
        public Func<Task> RunTask { get; }

        public ThrowOnScreen(Component parent, Vector2 startPoint)
        {
            RunTask = async delegate
            {
                var children = parent.GetComponentsInChildren<Transform>();
                var startPos = children.Select(transform => transform.position).Select(dummy => (Vector2) dummy).ToList();
                foreach (var transform in children)
                    transform.position = startPoint;
                var curve = AnimationCurve.EaseInOut(0, 0, .65f, 1);
                var timer = 0f;
                while (timer < 1)
                {
                    timer += Time.deltaTime;
                    for (var i = 0; i < children.Length; i++)
                        children[i].position = startPoint * (1 - curve.Evaluate(timer)) + startPos[i] * curve.Evaluate(timer);
                    await new WaitForUpdate();
                }
                for (var i = 0; i < children.Length; i++)
                    children[i].position = startPos[i];
            
            };
        }
    }

    public class Fade: ISequenceTask 
    {
        public Func<Task> RunTask { get; }

        public Fade(Component parent, Mode fadeInOut = Mode.Out, AnimationCurve curve = null)
        {
            RunTask = async delegate
            {
                if(curve == null)
                    curve = AnimationCurve.EaseInOut(0,0,1,1);
                if(parent == null) return;
                var renders = parent.GetComponentsInChildren<SpriteRenderer>();
                var images = parent.GetComponentsInChildren<Image>();
                var timer = curve.keys[0].time;
                var endTime = curve.keys[curve.keys.Length - 1].time;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                while (timer < endTime)
                {
                    timer += Time.deltaTime;
                    if(parent == null || parent.gameObject == null) return;
                    foreach (var renderer in renders)
                    {
                        if(renderer == null || renderer.gameObject == null) return;
                        var materialColor = renderer.color;
                        if (fadeInOut == Mode.In)
                            materialColor.a = curve.Evaluate(timer) ;
                        else if (fadeInOut == Mode.Out)
                            materialColor.a = 1 - curve.Evaluate(timer) ;
                        renderer.color = materialColor;
                    }
                    foreach (var renderer in images)
                    {
                        if(renderer == null || renderer.gameObject == null) return;
                        var materialColor = renderer.color;
                        if (fadeInOut == Mode.In)
                            materialColor.a = curve.Evaluate(timer) / endValue;
                        else if (fadeInOut == Mode.Out)
                            materialColor.a = 1 - curve.Evaluate(timer) / endValue;
                        renderer.color = materialColor;
                    }
                    await new WaitForUpdate();
                }
            };
        }
    }
}