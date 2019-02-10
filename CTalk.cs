using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DC
{
	public class CTalk : MonoBehaviour {

		const float width_ratio_nick = 0.3f;
		const float width_ratio_converstion = 1.0f - width_ratio_nick;

		bool mIsMe = true;
		UnityEngine.UI.Text mNick = null, mConversation = null;
		RectTransform mRectTranf = null;

		public bool is_me {
			get {
				return mIsMe;
			}
			set { 
				is_me = value;
				if (is_me) {
					nick_comp.transform.SetAsFirstSibling ();
				} else {
					conversation_comp.transform.SetAsLastSibling ();
				}
			}
		}

		private UnityEngine.UI.Text nick_comp { 
			get {
				if(mNick == null)
					mNick = transform.Find ("Nick").GetComponent<UnityEngine.UI.Text> ();
				return mNick;
			}
		}

		private UnityEngine.UI.Text conversation_comp { 
			get {
				if(mConversation == null)
					mConversation = transform.Find ("Conversation").GetComponent<UnityEngine.UI.Text> ();
				return mConversation;
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

		public float height_conversation {
			get { 
				return rectTrasform .rect.height;
			}
		}

		public RectTransform rectTrasform { 
			get { 
				if( mRectTranf == null)
					mRectTranf = (UnityEngine.RectTransform)this.transform;
				return mRectTranf;
			} 
		}

		void Awake() {

			var padding = rectTrasform.parent.GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
			var width = (rectTrasform.parent.transform as UnityEngine.RectTransform).rect.width - padding.horizontal;
			var width_nick = width_ratio_nick * width;
			var width_converstion = width_ratio_converstion * width;

			var rtrasf_nick = nick_comp.rectTransform;
			rtrasf_nick.sizeDelta = new Vector2(width_nick, rtrasf_nick.sizeDelta.y);
			var rtrasf_conversation = conversation_comp.rectTransform;
			rtrasf_conversation.sizeDelta = new Vector2 (width_converstion, rtrasf_conversation.sizeDelta.y);
		}
	}
}
