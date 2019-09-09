using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DC
{
	public class CModal : MonoBehaviour
	{
		public static CModal s {
			get {
				return Sas.Singletons.Get<CModal> ();
			}
		}

		public readonly LinkedList<CPopup> mPopups = new LinkedList<CPopup> ();

		public int viewed { get { return mPopups.Where (data => data.visible).Count (); } }

		public int created { get { return mPopups.Count; } }

		public static int CountIf (System.Predicate<CPopup> pred)
		{
			return s.mPopups.Where (popup => pred (popup)).Count ();
		}

		public static T Make<T> (Transform view) where T : CPopup
		{
			return s._Make<T> (view);
		}

		T _Make<T> (Transform view) where T : CPopup
		{
			var res = Resources.Load ("Prefabs/modal back");
			var inst = UnityEditor.PrefabUtility.InstantiatePrefab (res) as GameObject;
			inst.transform.SetParent (transform);

			var popup = inst.AddComponent<T> ();
			popup.Init (inst.GetComponent<Canvas> ());
			popup.onClose += () => { 
				mPopups.Remove (popup);
			};
			if (view != null)
				popup.view = view;
					
			// https://issuetracker.unity3d.com/issues/modifications-dot-empty-error-is-thrown-when-instantiating-a-prefab-that-contains-a-disabled-parent-canvas-and-enabled-child-canvas
			// Please note: The bottom code has some problem. pls check that issue tracker of above
			var Scaler = inst.GetComponent<UnityEngine.UI.CanvasScaler> ();
			if (Scaler)
				Scaler.enabled = true;
			
			mPopups.AddLast (popup);
			popup.depth = mPopups.Count;
			return popup;
		}

		public static CCommonPopup Make (CCommonPopup.Config config)
		{
			var prefab = Resources.Load (config.prefab);
			if (prefab == null) {
				Debug.unityLogger.LogError (
					System.Reflection.MethodBase.GetCurrentMethod ().Name,
					string.Format ("cannot find prefab('{0}')", config.prefab));
				return null;
			}

			var inst = UnityEditor.PrefabUtility.InstantiatePrefab (prefab) as GameObject;
			var popup = CModal.Make<CCommonPopup> (inst.transform);
			popup.SetConfig (config);
			return popup;
		}

		public static CCommonPopup Make (string title, string contents, string btn1 = "Ok", string btn2 = "", string btn3 = "")
		{
			CCommonPopup.Config config = new CCommonPopup.Config ();
			config.items ["title"] = title;
			config.items ["contents"] = contents;
			config.items ["btn1"] = btn1;
			config.items ["btn2"] = btn2;
			config.items ["btn3"] = btn3;
			return Make (config);
		}
	}

}
