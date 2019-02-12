using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UniRx;
using System.Net;
using System.Linq;
using UniRx;
using Sas;

namespace DC
{
	public class CEditbox : MonoBehaviour
	{
		// Use this for initialization
		Sas.Net.ClientSocket mSocket = null;
		Transform mContents;

		void Awake ()
		{
			mContents = GameObject.Find ("Content").transform;

			var input = this.gameObject.GetComponent<UnityEngine.UI.InputField> ();
			input.characterLimit = 0;


			//		mRequest.Post ("https://localhost:3700/", System.Text.Encoding.UTF8.GetBytes ("hi?"))
			//			.Subscribe (x => {
			//			Debug.Log ("good");
			//		});

			//		mSocket = mRequest.MakeSocket ("wss://localhost:3700/dsadasda");
			//		mSocket.on_connect += (bool ret) => { 
			//			Debug.Log (ret);
			//		};
			//
			//		mSocket.on_recv += (protocol) => { 
			//			string a = protocol.payload.ToString ();
			//			Debug.Log (a);
			//		};
			//
			//		mSocket.on_close += () => { 
			//			Debug.Log ("socket is closed");
			//		};
			input.OnEndEditAsObservable ()
				.Subscribe<string> (x => {
					if (x.Length == 0)
						return;

					var prefab = Resources.Load<DC.CTalk> ("Talk");
					var inst = UnityEngine.GameObject.Instantiate (prefab, mContents);
					inst.nick = "11";
					inst.conversation = x;
					input.text = "";
					input.ActivateInputField ();
					mContents.GetComponent<DC.CTalkContents> ().Add (inst);
				});
		}
	}

}
