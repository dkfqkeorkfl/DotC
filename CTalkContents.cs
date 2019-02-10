using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DC
{
	public class CTalkContents : MonoBehaviour
	{

		public RectTransform rectTranform { private set; get; }

		public float minest_height { private set; get; }

		public float height_talks { private set; get; }

		void Awake ()
		{
			rectTranform = transform as RectTransform;

			var talk_padding = GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
			height_talks = talk_padding.vertical;

			minest_height = (rectTranform.parent as RectTransform).rect.height + 1.0f;
			rectTranform.sizeDelta = new Vector2 (0, minest_height);
		}

		public void Add (CTalk talk)
		{
			rectTranform.sizeDelta = new Vector2 (0, minest_height + height_talks);
			UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate (rectTranform);
			height_talks += talk.height_conversation;

			var height_updated = Mathf.Max (minest_height, height_talks);
			rectTranform.sizeDelta = new Vector2 (0, height_updated);

			var print = string.Format ("height{0} argc({1})", height_talks, talk.height_conversation);
			Debug.Log (print);
		}
	}

}
