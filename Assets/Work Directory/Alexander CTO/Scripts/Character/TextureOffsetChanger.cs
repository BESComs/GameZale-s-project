using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOffsetChanger : MonoBehaviour
{

	private Renderer _renderer;
	private MaterialPropertyBlock _propBlock;

	public void Awake()
	{
		_propBlock = new MaterialPropertyBlock();
		_renderer = GetComponent<Renderer>();
	}

	public void SetOffset(Vector2 offset)
	{
		if (_renderer == null) return;
	
		var tilingOffset = new Vector4(1f, 1f, offset.x, offset.y);

		_propBlock.SetVector("_MainTex_ST", tilingOffset);
		_renderer.SetPropertyBlock(_propBlock);
	}


}