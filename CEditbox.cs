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
		Transform mContents;

		void ObserveEdit()
		{
			var input = this.gameObject.GetComponent<UnityEngine.UI.InputField> ();
			input.characterLimit = 0;
			
			input.OnEndEditAsObservable ()
				.Select(x=> {
					if (x.Length == 0)
						return x;
					
					input.enabled = false;
					input.text = "";

					var prefab = Resources.Load<DC.CTalk> ("Talk");
					var inst = UnityEngine.GameObject.Instantiate (prefab, mContents);
					var padding = mContents.GetComponent<UnityEngine.UI.VerticalLayoutGroup>().padding;
					inst.nick = "11";
					inst.conversation = x;
					inst.is_me = true;
					mContents.GetComponent<DC.CTalkContents> ().Add (inst);

					return x;
				})
				.Delay(new System.TimeSpan(3000000))
				.Subscribe<string> (x => {
					input.enabled = true;
					input.ActivateInputField ();
				});
		}
		void Awake ()
		{
			mContents = GameObject.Find ("Content").transform;
			ObserveEdit ();

			Sas.Net.ClientSocket.Connect("wss://localhost:8080/")
				.SelectMany(socket=> {
					socket.Send(2, "111");
					return socket.Recv();
				})
				.Subscribe(protocol=> {
					Debug.Log(protocol.utc);
				},
					exc=> {
						Debug.Log(exc);
					});
//					mSocket = mRequest.MakeSocket ("wss://localhost:3700/dsadasda");
//					mSocket.on_connect += (bool ret) => { 
//						Debug.Log (ret);
//					};
//			
//					mSocket.on_recv += (protocol) => { 
//						string a = protocol.payload.ToString ();
//						Debug.Log (a);
//					};
//			
//					mSocket.on_close += () => { 
//						Debug.Log ("socket is closed");
//					};
		}
	}

}
