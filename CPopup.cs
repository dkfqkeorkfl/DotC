using System;
using UnityEngine;
using System.Collections.Generic;
using UniRx;
using UnityEditor;
using Sas;

namespace DC
{
	public abstract class CPopup : MonoBehaviour
	{
		UnityEngine.Canvas mCanvas;

		public event Action onClose;

		public int depth {
			set { mCanvas.sortingOrder = value; }
			get { return mCanvas.sortingOrder; }
		}

		public bool visible {
			get { return gameObject.activeSelf; }
			set { gameObject.SetActive (value); }
		}

		public Transform view {
			set {
				var self = transform;
				for (var i = 0; i < self.childCount; ++i) {
					var child = self.GetChild (i);
					OnDetachView (child);
				}
				self.DetachChildren ();
				value.SetParent (self);
				OnAttachView (value);
			}

			get {
				return transform.GetChild (0);
			}
		}



		public void Init (UnityEngine.Canvas canvas)
		{
			mCanvas = canvas;
			Debug.Assert (mCanvas != null);
		}

		public void Close ()
		{
			if (onClose != null)
				onClose ();
			GameObject.Destroy (gameObject);
		}

		public abstract void OnAttachView (Transform view);

		public abstract void OnDetachView (Transform view);
	}

	public class CCommonPopup : CPopup
	{
		public class Config
		{
			public string prefab = "Prefabs/common popup";
			public Dictionary<string, string> items = new Dictionary<string, string> () {
				{ "title", "Title" },
				{ "contents", "Contents" },
				{ "btn1", "Ok" },
				{ "btn2", "No" },
				{ "btn3", "Retry" }
			};
		}

		LinkedList<IDisposable> mClickStream = new LinkedList<IDisposable> ();

		public event Action<CPopup,string> onHandleBtn;

		public void SetConfig (Config config)
		{
			if (!string.IsNullOrEmpty (config.items ["btn3"])) {
				Switch (3);
			} else if (!string.IsNullOrEmpty (config.items ["btn2"])) {
				Switch (2);
			} else {
				Switch (1);
			}

			var txts = FindObjectsOfType<UnityEngine.UI.Text> ();
			foreach (var txt in txts) {
				string desc;
				if (config.items.TryGetValue (txt.name, out desc))
					txt.text = desc;
			}
		}

		void Switch (int iCnt)
		{
			var btn1 = transform.FindDST (data => data.name == "1");
			var btn2 = transform.FindDST (data => data.name == "2");
			var btn3 = transform.FindDST (data => data.name == "3");
			btn1.gameObject.SetActive (false);
			btn2.gameObject.SetActive (false);
			btn3.gameObject.SetActive (false);

			switch (iCnt) {
			case 1:
				btn1.gameObject.SetActive (true);
				break;
			case 2:
				btn2.gameObject.SetActive (true);
				break;
			case 3:
				btn3.gameObject.SetActive (true);
				break;
			default :
				break;
			}
		}

		public override void OnAttachView (Transform view)
		{
			transform.OnChildrenAsObserable ()
				.Select (child => child.GetComponent<UnityEngine.UI.Button> ())
				.Where (btn => null != btn)
				.Select (btn => {
				return btn.OnClickAsOptional ()
						.Subscribe (_ => {
					if (onHandleBtn != null)
						onHandleBtn (this, btn.name);
				});
			})
				.Subscribe (data => {
				mClickStream.AddLast (data);
			});

		}

		public override void OnDetachView (Transform view)
		{
			if (mClickStream == null)
				return;

			foreach (var dispose in mClickStream) {
				dispose.Dispose ();
			}
			mClickStream.Clear ();
		}
	}
}

