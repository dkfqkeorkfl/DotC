using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Sas;
using System;

namespace DC
{
	public class CSignupPage : MonoBehaviour
	{
		public UnityEngine.GameObject mLoginPage;
		public UnityEngine.UI.InputField mAccountIDFld;
		public UnityEngine.UI.InputField mPasswordFld;
		public UnityEngine.UI.InputField mPasswordConfirmFld;
		public UnityEngine.UI.Button mBackBtn;
		public UnityEngine.UI.Button mConfirmBtn;

		void BackToLogin ()
		{
			this.gameObject.SetActive (false);
			mLoginPage.gameObject.SetActive (true);
		}

		void Awake ()
		{
			mBackBtn
				.OnClickAsOptional ()
				.Repeat ()
				.Subscribe (_ => BackToLogin ());
			
			RegistConfirm ();
		}

		void RegistConfirm ()
		{
			mConfirmBtn
				.OnClickAsOptional ()

				.SelectMany (_ => {
				if (mPasswordFld.text != mPasswordConfirmFld.text)
						throw new System.Exception ("missmatch password and confirm password. please check that a both is matched.");

				return DC.CNetwork.s.platform.account.Authentication ();
			})
				.SelectMany (err => {
				if (Sas.Net.COMMAND.OK == err) {

					var signup_param = new Sas.Data.AccountSignupParam ();
					var email = new System.Security.SecureString ();
					mAccountIDFld.text.ToList ().ForEach (c => email.AppendChar (c));
					signup_param.email = email;

					var password = new System.Security.SecureString ();
					mPasswordFld.text.ToList ().ForEach (c => password.AppendChar (c));
					signup_param.password = password;

					return DC.CNetwork.s.platform.account.SignUp (signup_param);
				} else
					throw new Sas.Exception (Sas.ERRNO.MESSAGE.ToErrCode(), "make sure that authentication.");
			})
				.Repeat ()
				.Subscribe (ret => {
				CModal.Make ("", "Wellcome to Dot chat!!").onHandleBtn += (CPopup arg1, string arg2) => arg1.Close ();
				BackToLogin ();
			},
				err => {
					if (!CNetwork.s.ContainHandleErr (err)) {
						if (string.IsNullOrEmpty (err.Message))
							CModal.Make ("", err.ToErrstrOfSas ()).onHandleBtn += (CPopup arg1, string arg2) => arg1.Close ();
						else
							CModal.Make ("", err.Message).onHandleBtn += (CPopup arg1, string arg2) => arg1.Close ();
					}
					RegistConfirm ();
				});
		}
	}
}
