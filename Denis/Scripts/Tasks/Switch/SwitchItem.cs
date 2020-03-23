using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Switch
{
	public class SwitchItem : MonoBehaviour
	{
		private TaskSwitch _taskSwitch;
		public Sprite[] Frames;
		public int RightFrame;
		public bool DefaultIsRight;
		[Space]
		public Vector3 Moving = Vector3.zero;

		private Animate _anim;
		private SpriteRenderer _render;
		private int _activeFrame = -1;
		[HideInInspector] public bool Success;
		private bool _startNextFrame;
		private void Awake()
		{
			_taskSwitch = GetComponentInParent<TaskSwitch>();
			_anim = new Animate(this, true);
			_render = GetComponent<SpriteRenderer>();
			Success = DefaultIsRight;
		}

		private async void SetNextFrame()
		{
			_startNextFrame = true;
			_activeFrame = (_activeFrame + 1) % Frames.Length;
			_anim.Scale(0.9f, .15f, _anim.BounceCurve);
			Timeout.Set(this, .075f, () => { _render.sprite = Frames[_activeFrame]; });
			await new WaitForSeconds(.225f);
			_startNextFrame = false;
		}

		private void CheckSuccess()
		{
			Success = _activeFrame == RightFrame;
		}

		private void OnMouseDown()
		{
			if (_startNextFrame || _taskSwitch.IsComplete || _taskSwitch.inLoadNextLevel) return;
			SetNextFrame();
			CheckSuccess();
			_taskSwitch.CheckObjects();
		}

		public void MoveUp()
		{
			if (!Moving.Equals(Vector3.zero))
				Timeout.Set(this, 0.4f, () =>
				{
					_anim.MoveAdd(Moving);
				});
		}
	}
}
