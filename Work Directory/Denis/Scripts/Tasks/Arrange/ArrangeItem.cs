using System.Collections.Generic;
using Tasks.Tangram;
using UnityEditor;
using UnityEngine;
using Work_Directory.Denis.Scripts.New_Folder;

namespace Tasks.Arrange
{
	public class ArrangeItem : MonoBehaviour
	{
		public SpriteRenderer Icon;
		[Space] public ArrangePosition[] RequiredPositions;
		public Vector2 RaycastOffset = new Vector2();
		private TaskArrange _taskArrange;
		[HideInInspector] public ArrangePosition PairedPosition;

		private Animate _anim;
		private Animate _iconAnim;
		private Vector2 _initialPosition;
		private Vector2 _mouseOffset;

		private void Awake()
		{
			_taskArrange = GetComponentInParent<TaskArrange>();
			_anim = new Animate(this, false);
			_iconAnim = new Animate(this, Icon.transform);
			_initialPosition = transform.position;
		}

		private void Start()
		{
			Drop();
		}

		public void OnMouseEnter()
		{
			if (_taskArrange.AllArranged()) return;
			_iconAnim.Scale(1.05f, 0.2f);
		}

		public void OnMouseExit()
		{
			if (_taskArrange.AllArranged()) return;

			_iconAnim.Scale(1f, 0.2f);
		}

		public void OnMouseDown()
		{
			if (_taskArrange.AllArranged()) return;

			_mouseOffset = _initialPosition - (Vector2)GetMouseWorldPosition();
			_iconAnim.Scale(1.1f, 0.2f);
			_anim.Colorize(0.7f, 1f);
			OnMouseDrag();
		}

		public void OnMouseDrag()
		{
			if (_taskArrange.AllArranged()) return;

			var pointer = (Vector2)GetMouseWorldPosition();
			transform.position = new Vector3(pointer.x + _mouseOffset.x, pointer.y + _mouseOffset.y);
		}

		public void OnMouseUp()
		{
			if (_taskArrange.AllArranged()) return;

			_iconAnim.Scale(1.05f, 0.2f);
			_anim.Colorize(1f, 0.2f);
			Drop();
			OnMouseExit();
		}

		private void Drop()
		{
			var raycastOffset = Quaternion.Euler(transform.eulerAngles) * RaycastOffset;
			var results = new RaycastHit2D[100];
			Physics2D.RaycastNonAlloc(new Vector2(transform.position.x + raycastOffset.x, transform.position.y + raycastOffset.y), Vector2.zero, results, Mathf.Infinity);
			var pairing = false;
			foreach (var res in results)
			{
				if(res.transform == null) continue;
				var field = res.transform.GetComponent<ArrangePosition>();
				
				if (field && Vector2.Distance(
					    new Vector2(transform.position.x, transform.position.y),
					    new Vector2(field.transform.position.x, field.transform.position.y)) <
				    _taskArrange.DroppingShift)
				{
					Pair(field);
					pairing = true;
					break;
				}
			}

			var tmp = FindObjectsOfType<Transform>();
			foreach (var transform1 in tmp)
			{
				var tmp1 = transform1.GetComponent<ArrangePosition>();
				if (tmp1 == null) continue;
				if (!tmp1 || !(Vector2.Distance(
					               new Vector2(transform.position.x, transform.position.y),
					               new Vector2(tmp1.transform.position.x, tmp1.transform.position.y)) <
				               _taskArrange.DroppingShift)) continue;
				Pair(tmp1);
				pairing = true;
				break;

			}

			if (!pairing)
			{
				_anim.Move(new Vector2(_initialPosition.x, _initialPosition.y), 0.5f);
				Timeout.Set(this, 0.5f, () => { transform.position = _initialPosition; });
			}
		}

		public void Pair(ArrangePosition arrange)
		{
			if (arrange.ArrangedItem != null && arrange.ArrangedItem != this)
			{
				PairedPosition.ArrangedItem = null;
				arrange.ArrangedItem.Pair(PairedPosition);
			}

			arrange.ArrangedItem = this;
			PairedPosition = arrange;
			_initialPosition =
				new Vector2(arrange.transform.position.x, arrange.transform.position.y);
			_anim.Move(_initialPosition, 0.5f);
			_taskArrange.CheckTangramPieces();
		}

		private Vector3 GetMouseWorldPosition()
		{
			return Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (Selection.activeObject != gameObject) return;
			Gizmos.color = Color.black;
			Gizmos.DrawRay(
				transform.position + Quaternion.Euler(transform.eulerAngles) *
				new Vector3(RaycastOffset.x, RaycastOffset.y), Vector3.forward);
		}
		
		#endif
	}
}