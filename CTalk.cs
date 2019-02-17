using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DC
{
	public class CTalk : MonoBehaviour {

//		const float nratio = 0.3f;
//		const float cratio = 1.0f - nratio;

		bool mIsMe = true;

		public bool is_me {
			get {
				return mIsMe;
			}
			set { 
				mIsMe = value;
				if (is_me) {
					var prev = conversation_comp.transform;
					prev.parent = null;
					prev.parent = this.transform;
					nick_comp.alignment = TextAnchor.MiddleLeft;
					conversation_comp.alignment = TextAnchor.MiddleLeft;
				} else {
					var prev = nick_comp.transform;
					prev.parent = null;
					prev.parent = this.transform;
					nick_comp.alignment = TextAnchor.MiddleRight;
					conversation_comp.alignment = TextAnchor.MiddleRight;
				}
			}
		}

		private UnityEngine.UI.Text nick_comp { 
			get {
				return transform.Find ("Nick").GetComponent<UnityEngine.UI.Text> ();
			}
		}

		private UnityEngine.UI.Text conversation_comp { 
			get {
				return transform.Find ("Conversation").GetComponent<UnityEngine.UI.Text> ();
			}
		}

		public string nick {
			set {
				nick_comp.text = value + " :";
			}
			get { 
				return nick_comp.text;
			}
		}

		public string conversation {
			set {
				conversation_comp.text = value;
			}
			get { 
				return conversation_comp.text;
			}
		}

		public float height {
			get { 
				return rectTransform.rect.height;
			}
		}

		public RectTransform rectTransform { 
			get { 
				return (UnityEngine.RectTransform)this.transform;
			} 
		}

		public float width { get; set; }
		public float nratio { get; set; } /* name ratio of width */

		public void Vaildate()
		{
			var nwidth = nratio * width;
			var cwidth = (1.0f - nratio) * width;

			var ntransf = nick_comp.rectTransform;
			ntransf.sizeDelta = new Vector2(nwidth, ntransf.sizeDelta.y);
			var ctransf = conversation_comp.rectTransform;
			ctransf.sizeDelta = new Vector2 (cwidth, ctransf.sizeDelta.y);
		}

//		void Awake() {
//			var padding = rectTrasform.parent.GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
//			var width = (rectTrasform.parent.transform as UnityEngine.RectTransform).rect.width - padding.horizontal;
//			var nwidth = nratio * width;
//			var cwidth = cratio * width;
//
//			var ntransf = nick_comp.rectTransform;
//			ntransf.sizeDelta = new Vector2(nwidth, ntransf.sizeDelta.y);
//			var ctransf = conversation_comp.rectTransform;
//			ctransf.sizeDelta = new Vector2 (cwidth, ctransf.sizeDelta.y);
//		}
	}
}
