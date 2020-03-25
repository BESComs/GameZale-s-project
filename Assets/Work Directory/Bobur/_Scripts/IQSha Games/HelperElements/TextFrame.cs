using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TextFrame : MonoBehaviour
{
    [SerializeField] private TextMeshPro textObject;
    public SpriteRenderer spriteRenderer { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public TextMeshPro TextObject => textObject;
    public void SetText(string text) => textObject.text = text;
}
