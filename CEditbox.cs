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
		public Sas.User inst { get; private set; }
		// Use this for initialization
		Sas.Net.ClientSocket mSocket = null;
		Transform mContents;

		void Awake ()
		{
			inst = new Sas.User ("https://127.0.0.1:3700", "public");

			//		var login = new Sas.USER.AccountLogInParam ();
			//		login.id = new System.Security.SecureString ();
			//		"dkfqkeorkfl".ToList ().ForEach (c => login.id.AppendChar (c));
			//		login.password = new System.Security.SecureString ();
			//		"QAZqaz5033!@".ToList ().ForEach (c => login.password.AppendChar (c));


			//		inst.account
			//			.Login (login)
			//			.Subscribe (
			//			data => {
			//					Debug.Log("1");
			//			},
			//			err => {
			//					Debug.Log("1");
			//			},
			//			() => {
			//					Debug.Log("1");
			//			});

			//		var aaa = Sas.SecureHelper.APP_SALT;
			//		var bytes = new System.Byte[64];
			//		var aa = System.Security.Cryptography.RandomNumberGenerator.Create ();
			//		aa.GetBytes (bytes);
			//		var aaaa = System.Convert.ToBase64String (bytes);

			inst.account.Authentication ()
				.SelectMany (err => {
					if (Sas.Net.COMMAND.OK == err) {
						var signup_param = new Sas.Data.AccountSignupParam ();

						var password = new System.Security.SecureString ();
						"QAZqaz5033!@".ToList ().ForEach (c => password.AppendChar (c));
						signup_param.password = password;

						var email = new System.Security.SecureString ();
						"dltmdgud@nate.com".ToList ().ForEach (c => email.AppendChar (c));
						signup_param.email = email;

						return inst.account.SignUp (signup_param);
					} else
						throw new System.Exception ("err");
				})
				.SelectMany (ret => {

					var login = new Sas.Data.AccountLoginParam ();
					login.email = new System.Security.SecureString ();
					"dltmdgud@nate.com".ToList ().ForEach (c => login.email.AppendChar (c));
					login.password = new System.Security.SecureString ();
					"QAZqaz5033!@".ToList ().ForEach (c => login.password.AppendChar (c));
					return inst.account.Login (login);
				})
				.SelectMany (ret => {
					return inst.account.AccessOpen ();
				})
				//		.SelectMany (ret => {
				//			return inst.account.AccessDel (ret.Value.serial);
				//		})
				.SelectMany (ret => {
					var login = new Sas.Data.AccountLoginParam ();
					login.sns = Sas.Data.SNS.AUTO;
					login.extension = "test.dd";
					return inst.account.DumpAutoLogin ("test.dd").SelectMany (_=>inst.account.Login(login));
				})
				.Subscribe (
					packet => {
						Debug.Log ("1");
					},
					err => {
						var exception = err as Sas.Exception;
						if (exception != null) {
							var errno = exception.errno;
							//exception.error.
						}
						Debug.Log (err.Message);
					});


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
