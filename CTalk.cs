using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DC
{
	public class CTalk : MonoBehaviour {

		bool mIsMe = true;

		public bool is_me {
			get {
				return mIsMe;
			}
			set { 
				mIsMe = value;
				if (mIsMe) {
					nick_comp.color = new Color32 (0, 0, 255, 255);
					conversation_comp.color = new Color32 (0, 0, 255, 255);
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
				nick_comp.text = value;
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
	}
}
