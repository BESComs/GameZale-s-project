using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Work_Directory.Denis.Scripts;

namespace Work_Directory.Denis.Scenes.NewLauncher.Scripts
{
    /*
     * все анимации ниже работают по схожему принципу
     * используются для Переходов из выбора игр в выбор категорий и обратно
     * GameListUnloadAnimation работает в паре с LoadAnimation
     * первый используется для анимировнного исчезновения списка игр
     * второй для анимировоного появления списка категорий
     * GameListLoadAnimation и UnLoadAnimation используются для проигрывания обратной анимации
    */
    public class GameListUnLoadAnimation : ISequenceTask
    {
        public Func<Task> RunTask { get; }
        public GameListUnLoadAnimation(Component component, AnimationCurve curve, Vector2 destPos)
        {
            RunTask = async delegate
            {   
                //назначение начальных значений переменных взятых из curve 
                var endTime = curve.keys[curve.keys.Length - 1].time;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                var timer = curve.keys[0].time;
                Transform transform;
                var renders = (transform = component.transform).GetComponentsInChildren<TextMeshPro>();
                var spritesRenderer = transform.GetComponentsInChildren<SpriteRenderer>();
                var startPos = transform.localPosition;
                //зануление альфа канала
                foreach (var renderer in renders)
                {
                    var material = renderer.color;
                    material.a = 0;
                    renderer.color = material;
                }
                foreach (var spriteRenderer in spritesRenderer)
                {
                    var material = spriteRenderer.color;
                    material.a = 0;
                    spriteRenderer.color = material;
                }
                //плавное исчезновение(FadeOut), и сдвиг в позицию destPos обьектов на сцене 
                while (timer < endTime)
                {
                    var t = curve.Evaluate(timer) / endValue;
                    timer += Time.deltaTime;
                    transform.transform.localPosition = startPos * (1 - t) + (Vector3)destPos * t;
                    transform.localScale = Vector3.one * (1 - t);
                    foreach (var renderer in renders)
                    {
                        var material = renderer.color;
                        material.a = 1 - t;
                        renderer.color = material;
                    }
                    foreach (var spriteRenderer in spritesRenderer)
                    {
                        var material = spriteRenderer.color;
                        material.a = Mathf.Max(1 - t * 2, 0);
                        spriteRenderer.color = material;
                    }
                    await new WaitForUpdate();
                }
                //установка альфа каналов равным 1
                foreach (var renderer in renders)
                {
                    var material = renderer.color;
                    material.a = 1;
                    renderer.color = material;
                }
                foreach (var spriteRenderer in spritesRenderer)
                {
                    var material = spriteRenderer.color;
                    material.a = 1;
                    spriteRenderer.color = material;
                }
                //установка в финальные позиции
                transform.localScale = Vector3.one;
                transform.localPosition = destPos;
            };
        }
    }
    
    public class GameListLoadAnimation : ISequenceTask
    {
        public Func<Task> RunTask { get; }

        public GameListLoadAnimation(Component component, AnimationCurve curve)
        {
            
            RunTask = async () =>
            {
                var endTime = curve.keys[curve.keys.Length - 1].time;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                var timer = curve.keys[0].time;

                Transform transform;
                var renders = (transform = component.transform).transform.GetComponentsInChildren<TextMeshPro>();
                for (var i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
                var spritesRenderer = transform.GetComponentsInChildren<SpriteRenderer>();
                var startPos = transform.localPosition;
                transform.localScale = new Vector3(1,0,1);
                foreach (var renderer in renders)
                {
                    var material = renderer.color;
                    material.a = 0;
                    renderer.color = material;
                }

                foreach (var spriteRenderer in spritesRenderer)
                {
                    var material = spriteRenderer.color;
                    material.a = 0;
                    spriteRenderer.color = material;
                }
                while (timer < endTime)
                {
                    var t = curve.Evaluate(timer) / endValue;
                    timer += Time.deltaTime;
                    component.transform.localPosition = startPos * (1 - t) + Vector3.zero * t;
                    transform.localScale = Vector3.one * t;
                    foreach (var renderer in renders)
                    {
                        var material = renderer.color;
                        material.a = t;
                        renderer.color = material;
                    }
                    foreach (var spriteRenderer in spritesRenderer)
                    {
                        var material = spriteRenderer.color;
                        material.a = t;
                        spriteRenderer.color = material;
                    }
                    await new WaitForUpdate();
                }
                foreach (var renderer in renders)
                {
                    var material = renderer.color;
                    material.a = 1;
                    renderer.color = material;
                }
                foreach (var spriteRenderer in spritesRenderer)
                {
                    var material = spriteRenderer.color;
                    material.a = 1;
                    spriteRenderer.color = material;
                }
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
            };
        }
    }
    public class UnLoadAnimation : ISequenceTask
    {
        public Func<Task> RunTask { get; }
        
        public UnLoadAnimation(Component component, AnimationCurve curve, Vector2 selectFolderPos)
        {
            RunTask = async delegate
            {
                var endTime = curve.keys[curve.keys.Length - 1].time;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                var timer = curve.keys[0].time;
                var startScale = component.transform.localScale;
                selectFolderPos *= 8.5f;
                var startPos = component.transform.position;
                var renders = component.transform.GetComponentsInChildren<SpriteRenderer>();
                while (timer < endTime)
                {
                    timer += Time.deltaTime;
                    component.transform.localScale = startScale * (1 + 7.5f * curve.Evaluate(timer) / endValue);
                    component.transform.position = startPos * (1 - curve.Evaluate(timer)/endValue) - (Vector3)selectFolderPos * curve.Evaluate(timer) / endValue;
                    foreach (var renderer in renders)
                    {
                        var materialColor = renderer.color;
                        materialColor.a = 1 - curve.Evaluate(timer) / endTime;
                        renderer.color = materialColor;
                    }
                    await new WaitForUpdate();
                }
                foreach (var renderer in renders)
                {
                    var materialColor = renderer.color;
                    materialColor.a = 0;
                    renderer.color = materialColor;
                }
                component.transform.localScale = startScale * 8.5f;
            };
        }
    }
    public class LoadAnimation : ISequenceTask
    {
        public Func<Task> RunTask { get; }

        public LoadAnimation(Component component, AnimationCurve curve)
        {
            RunTask = async delegate
            {
                var endTime = curve.keys[curve.keys.Length - 1].time;
                var endValue = curve.keys[curve.keys.Length - 1].value;
                var timer = curve.keys[0].time;
                var startScale = component.transform.localScale;
                var startPos = component.transform.position;
                var renders = component.transform.GetComponentsInChildren<SpriteRenderer>();
                while (timer < endTime)
                {
                    timer += Time.deltaTime;
                    component.transform.localScale = startScale * (1 - curve.Evaluate(timer) / endValue) + Vector3.one * curve.Evaluate(timer) / endValue;
                    component.transform.position = startPos * (1 - curve.Evaluate(timer)/endValue);
                    foreach (var renderer in renders)
                    {
                        var materialColor = renderer.color;
                        materialColor.a = curve.Evaluate(timer) / endTime;
                        renderer.color = materialColor;
                    }
                    await new WaitForUpdate();
                }
                foreach (var renderer in renders)
                {
                    var materialColor = renderer.color;
                    materialColor.a = 1;
                    renderer.color = materialColor;
                }
                component.transform.position = Vector2.zero;
            };
        }
    }
}
