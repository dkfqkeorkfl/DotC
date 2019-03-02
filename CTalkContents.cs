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
			count = 0;
			rectTranform = transform as RectTransform;

			var talk_padding = GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
			height_talks = talk_padding.vertical;

			minest_height = (rectTranform.parent as RectTransform).rect.height + 1.0f;
			rectTranform.sizeDelta = new Vector2 (0, minest_height);
		}

		public int count { get; private set; }
		public void Add (CTalk talk)
		{
			count += 1;
			rectTranform.sizeDelta = new Vector2 (0, minest_height + height_talks);
			UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate (rectTranform);
			height_talks += talk.height + GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().spacing;

			var height_updated = Mathf.Max (minest_height, height_talks);
			rectTranform.sizeDelta = new Vector2 (0, height_updated);
		}
	}

}
