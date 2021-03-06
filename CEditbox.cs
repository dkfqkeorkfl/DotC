﻿using System.Collections;
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
		UnityEngine.UI.Button mBtnLastMatch = null;

		public ulong current_chat_id { get; private set; }

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
				var inst = Instantiate (prefab, mContents);
				var padding = mContents.GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
				var obj = DC.CNetwork.s.platform.context.local_db.Get ("txt_me_viewed") as Newtonsoft.Json.Linq.JObject;

				inst.type = DC.CTalk.TYPE.ME;
				inst.nick = (string)obj ["value"];
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

		void Connect (int retry = 0)
		{
			if (connection != null)
				connection.Dispose ();
			
			connection = Sas.SasUtil.StartRx ()
				.SelectMany (_ => {
				if (DC.CNetwork.s.platform.context.token != null)
					return Sas.SasUtil.StartRx ();

				return DC.CNetwork.s.platform.account.Authentication ()
						.SelectMany (err => {
					if ("ok" == err) {
						var login_param = new Sas.Data.AccountLoginParam ();
						var email = new System.Security.SecureString ();
						"dltmdgud@nate.com".ToList ().ForEach (c => email.AppendChar (c));
						login_param.email = email;

						var password = new System.Security.SecureString ();
						"QAZqaz1289!@".ToList ().ForEach (c => password.AppendChar (c));
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
					return Sas.SasUtil.StartRx ().Select (__ => this.socket);
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
					var error = err as Sas.Exception;
					if (error != null) {
						Debug.LogError (error.ToErrstrOfDC ());
						Debug.LogError (error.ToErrnoOfSas ());
					}

					Debug.LogError (err.Message);
					AttachMatchBtn ();
				});
		}

		UniRx.IObservable<bool> InitRecommand (ulong current, UnityEngine.UI.VerticalLayoutGroup inst)
		{
			var obj = DC.CNetwork.s.platform.context.local_db.Get ("confirm_color") as Newtonsoft.Json.Linq.JObject;
			var vec = Sas.SasUtil.CovertTxtToVec4 ((string)obj ["value"]);
			var color = new Color (vec.x, vec.y, vec.z, vec.w);
				
			var likes = inst.transform.FindDST (child => child.name == "likes");
			var unlikes = inst.transform.FindDST (child => child.name == "unlikes");

			return Sas.SasUtil.StartRx ()
				.SelectMany (_ => {
				return UniRx.Observable.Range (1, 2).Select (i => {
					switch (i) {
					case 1:
						return likes.GetComponent<UnityEngine.UI.Button> ()
								.OnClickAsOptional ()
								.Select (__ => {
									
							likes.GetComponent<UnityEngine.UI.Image> ().color = color;
							
							return i;
						});
					case 2:
						return unlikes.GetComponent<UnityEngine.UI.Button> ()
								.OnClickAsOptional ()
								.Select (__ => {
							unlikes.GetComponent<UnityEngine.UI.Image> ().color = color;
							
							return i;
						});
					}
					return Sas.SasUtil.StartRx ();
				})
						.SelectMany (exce => exce)
						.SelectMany (i => {
					likes.GetComponent<UnityEngine.UI.Button> ().enabled = false;
					unlikes.GetComponent<UnityEngine.UI.Button> ().enabled = false;

					return DC.CNetwork.s.chatti.Likes (current, i == 1)
						.Catch<bool, System.Exception> (err => {
						var serr = err as Sas.Exception;
						if (serr == null || serr.code != Sas.ERRNO.ALREADY_SETTED.ToErrCode ()) {
							likes.GetComponent<UnityEngine.UI.Button> ().enabled = true;
							unlikes.GetComponent<UnityEngine.UI.Button> ().enabled = true;
							var src = (i == 1 ? unlikes : likes);
							var dst = (i == 1 ? likes : unlikes);
							dst.GetComponent<UnityEngine.UI.Image> ().color = src.GetComponent<UnityEngine.UI.Image> ().color;
						}

						return InitRecommand (current, inst);
					});
				});
			});
		}

		void AttachRecommand (ulong current)
		{
			Sas.SasUtil.StartRx ()
				.SelectMany (_ => {
				var prefab = Resources.Load<UnityEngine.UI.VerticalLayoutGroup> ("Prefabs/recommand");
				var inst = Instantiate(prefab, mContents);
				mContents.GetComponent<DC.CTalkContents> ().Add (inst);
				return InitRecommand (current, inst);
			})
				.First ()
				.Subscribe (_ => {
			}, 
				err => {
					Debug.LogError (err);
				});
		}

		void AttachMatchBtn ()
		{
			if (mBtnLastMatch != null) {
				mBtnLastMatch.enabled = false;
			}

			if (this.current_chat_id != 0) {
				AttachRecommand (this.current_chat_id);
				this.current_chat_id = 0;
			}
			
			// to delay 1 frame
			Sas.SasUtil.StartRx ()
				.Delay (new System.TimeSpan (1))
							.SelectMany (_ => {
				AttachNotice ("for starting, please click the matching button");
				var prefab = Resources.Load<UnityEngine.UI.Button> ("Prefabs/btn_chat_one");
				mBtnLastMatch = Instantiate(prefab, mContents);
				mContents.GetComponent<DC.CTalkContents> ().Add (mBtnLastMatch);
				return mBtnLastMatch.OnClickAsOptional ();
			})
				.Catch<UniRx.Unit, System.Exception> (err => {
				AttachMatchBtn ();
				throw err;
				return Sas.SasUtil.StartRx ().Select (_ => UniRx.Unit.Default);
			})
				.First ()
				.Do (__ => {
				mBtnLastMatch.enabled = false;
				Connect ();
			})
				.Subscribe (_ => {
			}, 
				err => {
					Debug.LogError (err);
					if (mBtnLastMatch != null)
						mBtnLastMatch.enabled = true;
				});
		}


		void AttachNotice (string str)
		{
			var prefab = Resources.Load<DC.CTalk> ("Prefabs/Notice");
			var inst = Instantiate(prefab, mContents);
			var padding = mContents.GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
			inst.type = DC.CTalk.TYPE.NOTICE;
			inst.conversation = str;

			var comp = mContents.GetComponent<DC.CTalkContents> ();
			if (comp != null)
				comp.Add (inst);
		}

		void Awake ()
		{
			current_chat_id = 0;
			if (!DC.CNetwork.s.handler.Has ("/dc/chat/matched")) {
				DC.CNetwork.s.handler.Add ("/dc/chat/matched", token => {
					var obj = token as Newtonsoft.Json.Linq.JObject;
					this.current_chat_id = (ulong)obj ["room_idx"];
					AttachNotice ("good luck. you can tell with stranger now.");
				});
			}

			if (!DC.CNetwork.s.handler.Has ("/dc/chat/join")) {
				DC.CNetwork.s.handler.Add ("/dc/chat/join", token => {
					var value = token as Newtonsoft.Json.Linq.JValue;
					var success = (bool)value;
					if (success) {
						AttachNotice ("starting match with new stranger. please waiting a second.");
					} else {
						AttachMatchBtn ();
					}
				});
			}

			if (!DC.CNetwork.s.handler.Has ("/dc/chat/recv")) {
				DC.CNetwork.s.handler.Add ("/dc/chat/recv", token => {
					var obj = (Newtonsoft.Json.Linq.JObject)token;
					var nick = obj ["nick"];
					var chat = obj ["chat"];

					var dobj = DC.CNetwork.s.platform.context.local_db.Get ("txt_default_name") as Newtonsoft.Json.Linq.JObject;
					var prefab = Resources.Load<DC.CTalk> ("Prefabs/Talk");
					var inst = Instantiate (prefab, mContents);
					var padding = mContents.GetComponent<UnityEngine.UI.VerticalLayoutGroup> ().padding;
					inst.type = DC.CTalk.TYPE.OTHER;
					inst.nick = (string)nick == "" ? (string)dobj ["value"] : (string)nick;
					inst.conversation = (string)chat;
					mContents.GetComponent<DC.CTalkContents> ().Add (inst);
				});
			}

			if (!DC.CNetwork.s.handler.Has ("/dc/chat/leave")) {
				DC.CNetwork.s.handler.Add ("/dc/chat/leave", token => {
					AttachNotice ("your chatting is exit. please match again");
					AttachMatchBtn ();
				});
			}
				
			mContents = GameObject.Find ("Content").transform;
			ObserveEdit ();

			AttachMatchBtn ();

		}
	}

}
