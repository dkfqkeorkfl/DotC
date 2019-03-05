using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sas;
using UniRx;
using UnityEngine;

namespace DC {
    public class CLoginPage : MonoBehaviour {

        public UnityEngine.GameObject mSignUpPage;
        public UnityEngine.UI.InputField mAccountIDFld;
        public UnityEngine.UI.InputField mPasswordFld;
        public UnityEngine.UI.Button mSignUpBtn;
        public UnityEngine.UI.Button mLoginBtn;

        void Awake () {
            mSignUpBtn
                .OnClickAsOptional ()
                .Repeat ()
                .Subscribe (_ => {
                    this.gameObject.SetActive (false);
                    mSignUpPage.gameObject.SetActive (true);
                });

            RegistLogin ();
        }

        void RegistLogin () {
            mLoginBtn
                .OnClickAsOptional ()

                .SelectMany (_ => {
                    return DC.CNetwork.s.platform.account.Authentication ();
                })
                .SelectMany (err => {
                    if (Sas.Net.COMMAND.OK == err) {
                        var login_param = new Sas.Data.AccountLoginParam ();
                        var email = new System.Security.SecureString ();
                        mAccountIDFld.text.ToList ().ForEach (c => email.AppendChar (c));
                        login_param.email = email;

                        var password = new System.Security.SecureString ();
                        mPasswordFld.text.ToList ().ForEach (c => password.AppendChar (c));
                        login_param.password = password;

                        return DC.CNetwork.s.platform.account.Login (login_param);
                    } else
						throw new Sas.Exception (Sas.ERRNO.MESSAGE.ToErrCode(), "make sure that authentication.");
                })
                .SelectMany (_ => DC.CNetwork.s.platform.account.AccessOpen ())
                .SelectMany (_ => DC.CNetwork.s.platform.account.DumpAutoLogin ("test"))
//                .SelectMany (_ => {
//                    var login = new Sas.Data.AccountLoginParam ();
//                    login.sns = Sas.Data.SNS.AUTO;
//                    login.extension = "test";
//                    return DC.CNetwork.s.platform.account.Login (login);
//                })
            	.Repeat ()
                .Subscribe (ret => {
                        CModal.Make ("", "success").onHandleBtn += (CPopup arg1, string arg2) => arg1.Close ();
                        //Debug.Log ("success");
                    },
                    err => {
                        if (!CNetwork.s.ContainHandleErr (err)) {
                            if (string.IsNullOrEmpty (err.Message))
                                CModal.Make ("", err.ToErrstrOfSas ()).onHandleBtn += (CPopup arg1, string arg2) => arg1.Close ();
                            else
                                CModal.Make ("", err.Message).onHandleBtn += (CPopup arg1, string arg2) => arg1.Close ();
                        }
                        RegistLogin ();
                    });
        }
    }

}