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
		Sas.Net.Websocket socket;

		void ObserveEdit ()
		{
			var input = this.gameObject.GetComponent<UnityEngine.UI.InputField> ();
			input.characterLimit = 0;
			
			input.OnEndEditAsObservable ()
				.Select (x => {
				if (x.Length == 0)
					return x;
					
				input.enabled = false;
				input.text = "";

				var prefab = Resources.Load<DC.CTalk> ("Prefabs/Talk");
				var inst = UnityEngine.GameObject.Instantiate (prefab, mContents);
				var padding = mContents.GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
				inst.nick = "11";
				inst.conversation = x;
				inst.type = DC.CTalk.TYPE.ME;
				mContents.GetComponent<DC.CTalkContents> ().Add (inst);

				return x;
			})
				.Delay (new System.TimeSpan (3000000))
				.Subscribe<string> (x => {
				input.enabled = true;
				input.ActivateInputField ();
			});
		}

		System.IDisposable connection;

		void Connect ()
		{
			if (connection != null)
				connection.Dispose ();
			if (this.socket != null && this.socket.is_connected)
				this.socket.Close ();
			
			connection = UniRx.Observable.Range (0, 1)
				.SelectMany (_ => {
				if (DC.CNetwork.s.platform.context.token != null)
					return UniRx.Observable.Range (0, 1);

				return DC.CNetwork.s.platform.account.Authentication ()
						.SelectMany (err => {
					if ("ok" == err) {
						var login_param = new Sas.Data.AccountLoginParam ();
						var email = new System.Security.SecureString ();
						"dltmdgud@nate.com".ToList ().ForEach (c => email.AppendChar (c));
						login_param.email = email;

						var password = new System.Security.SecureString ();
						"QAZqaz5033!@".ToList ().ForEach (c => password.AppendChar (c));
						login_param.password = password;

						return DC.CNetwork.s.platform.account.Login (login_param);
					} else
						throw new Sas.Exception (Sas.ERRNO.MESSAGE.ToErrCode (), "make sure that authentication.");
				})
						.SelectMany (__ => {
					return DC.CNetwork.s.platform.account.AccessOpen ();
				})
						.Select (__ => 1);
			})
				.SelectMany (_ => {
				if (this.socket != null && this.socket.is_connected)
					return UniRx.Observable.Range (0, 1).Select (__ => socket);
				return DC.CNetwork.s.platform.MakeWS ("wss://localhost:8080/");
			})
				.SelectMany (socket => {
				this.socket = socket;
				return socket.Send ("/dc/chat/join", "")
						.SelectMany (_ => socket.Recv ());
			})
				.Subscribe (_ => {
			},
				err => {
					Debug.LogError (err);
					Connect ();
				});
		}

		void AttachMatchBtn ()
		{
			UniRx.Observable.Range(0,1)
				.Delay(new System.TimeSpan(1))
				.Do(_=> {
					var prefab = Resources.Load<UnityEngine.UI.Button> ("Prefabs/btn_chat_one");
					var inst = UnityEngine.GameObject.Instantiate (prefab, mContents);
					mContents.GetComponent<DC.CTalkContents> ().Add (inst);
					inst.OnClickAsOptional ()
						.Subscribe (__ => {
							inst.enabled = false;
							Connect ();
						});
				})
				.Subscribe(_=> {}, 
					err=> {
						Debug.LogError(err);
						AttachMatchBtn ();
					});
		}

		void Awake ()
		{
			if (!DC.CNetwork.s.platform.context.handler.Has ("/dc/chat/matched")) {
				DC.CNetwork.s.platform.context.handler.Add ("/dc/chat/matched", token => {
					var prefab = Resources.Load<DC.CTalk> ("Prefabs/Notice");
					var inst = UnityEngine.GameObject.Instantiate (prefab, mContents);
					var padding = mContents.GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
					inst.type = DC.CTalk.TYPE.NOTICE;
					inst.conversation = "good luck. you can tell with stranger now.";
					mContents.GetComponent<DC.CTalkContents> ().Add (inst);
				});
			}

			if (!DC.CNetwork.s.platform.context.handler.Has ("/dc/chat/join")) {
				DC.CNetwork.s.platform.context.handler.Add ("/dc/chat/join", token => {
					var value = token as Newtonsoft.Json.Linq.JValue;
					var success = (bool)value;
					if (success) {
						var prefab = Resources.Load<DC.CTalk> ("Prefabs/Notice");
						var inst = UnityEngine.GameObject.Instantiate (prefab, mContents);
						var padding = mContents.GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
						inst.type = DC.CTalk.TYPE.NOTICE;
						inst.conversation = "starting match with new stranger. please waiting a second.";
						mContents.GetComponent<DC.CTalkContents> ().Add (inst);
						
					} else {
						AttachMatchBtn ();
					}
				});
			}

			if (!DC.CNetwork.s.platform.context.handler.Has ("/dc/chat/recv")) {
				DC.CNetwork.s.platform.context.handler.Add ("/dc/chat/recv", token => {
					var text = (string)token;
					var prefab = Resources.Load<DC.CTalk> ("Prefabs/Talk");
					var inst = UnityEngine.GameObject.Instantiate (prefab, mContents);
					var padding = mContents.GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
					inst.type = DC.CTalk.TYPE.OTHER;
					inst.conversation = text;
					mContents.GetComponent<DC.CTalkContents> ().Add (inst);
				});
			}

			if (!DC.CNetwork.s.platform.context.handler.Has ("/dc/chat/leave")) {
				DC.CNetwork.s.platform.context.handler.Add ("/dc/chat/leave", token => {
					var prefab = Resources.Load<DC.CTalk> ("Prefabs/Notice");
					var inst = UnityEngine.GameObject.Instantiate (prefab, mContents);
					var padding = mContents.GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
					inst.type = DC.CTalk.TYPE.NOTICE;
					inst.conversation = "your chatting is exit. please match again";
					mContents.GetComponent<DC.CTalkContents> ().Add (inst);
					AttachMatchBtn ();
				});
			}
				
			mContents = GameObject.Find ("Content").transform;
			ObserveEdit ();
			AttachMatchBtn ();
		}
	}

}
