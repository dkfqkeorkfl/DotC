using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DC
{
	public class CTalk : MonoBehaviour
	{
		public enum TYPE
		{
			NONE,
			ME,
			OTHER,
			NOTICE,
		}

		public TYPE mType = TYPE.NONE;

		public TYPE type {
			get {
				return mType;
			}
			set { 
				mType = value;
				switch (mType) {
				case TYPE.ME: 
					{
						var obj = DC.CNetwork.s.platform.context.local_db.Get ("txt_me_color") as Newtonsoft.Json.Linq.JObject;
						var vec = Sas.SasUtil.CovertTxtToVec4 ((string)obj ["value"]);
						var color = new Color (vec.x, vec.y, vec.z, vec.w);
						nick_comp.color = color;
					}
					{
						var obj = DC.CNetwork.s.platform.context.local_db.Get ("txt_me_color") as Newtonsoft.Json.Linq.JObject;
						var vec = Sas.SasUtil.CovertTxtToVec4 ((string)obj ["value"]);
						var color = new Color (vec.x, vec.y, vec.z, vec.w);
						conversation_comp.color = color;
					}
					break;
				case TYPE.OTHER: 
					{
						var obj = DC.CNetwork.s.platform.context.local_db.Get ("txt_other_color") as Newtonsoft.Json.Linq.JObject;
						var vec = Sas.SasUtil.CovertTxtToVec4 ((string)obj ["value"]);
						var color = new Color (vec.x, vec.y, vec.z, vec.w);
						nick_comp.color = color;
					}
					{
						var obj = DC.CNetwork.s.platform.context.local_db.Get ("txt_other_color") as Newtonsoft.Json.Linq.JObject;
						var vec = Sas.SasUtil.CovertTxtToVec4 ((string)obj ["value"]);
						var color = new Color (vec.x, vec.y, vec.z, vec.w);
						conversation_comp.color = color;
					}
					break;
				case TYPE.NOTICE: 
					break;

				}
			}
		}

		private UnityEngine.UI.Text nick_comp { 
			get {
				if (type == TYPE.ME || type == TYPE.OTHER)
					return transform.Find ("Nick").GetComponent<UnityEngine.UI.Text> ();
				return null;
			}
		}

		private UnityEngine.UI.Text conversation_comp { 
			get {
				return transform.Find ("Conversation").GetComponent<UnityEngine.UI.Text> ();
			}
		}

		public string nick {
			set {
				if (type == TYPE.ME || type == TYPE.OTHER)
					nick_comp.text = value;
			}
			get { 
				if (type == TYPE.ME || type == TYPE.OTHER)
					return nick_comp.text;
				
				return "";
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
