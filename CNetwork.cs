using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sas;
using UniRx;

namespace DC
{
	public class CNetwork : MonoBehaviour
	{
		public static CNetwork s {
			get {
				return Sas.Singletons.Get<CNetwork> ();
			}
		}

		LinkedList<KeyValuePair<System.Text.RegularExpressions.Regex, Action<System.Net.HttpWebRequest, System.Exception>>> mErrHandles
		= new LinkedList<KeyValuePair<System.Text.RegularExpressions.Regex, Action<System.Net.HttpWebRequest, System.Exception>>> ();

		public Sas.User platform { get; private set; }
		public DC.COMPONENET.Chatti chatti { get; private set; }
		public Sas.Net.ProtocolHandler handler {
			get {
				return platform.context.handler;
			}
		}

		public bool ContainHandleErr (System.Exception e)
		{
			if (e is Sas.Net.ExceptionReq == false)
				return false;

			foreach (var pr in mErrHandles) {
				if (pr.Key.IsMatch (e.ToErrstrOfSas ()))
					return true;
			}

			return false;
		}

		public void AddHnadleErr (
			System.Text.RegularExpressions.Regex rgx, 
			Action<System.Net.HttpWebRequest, System.Exception> func)
		{
			var pararm = 
				new KeyValuePair<System.Text.RegularExpressions.Regex, Action<System.Net.HttpWebRequest, System.Exception>> (
					rgx, func);
			mErrHandles.AddLast (pararm);
		}

		void Awake ()
		{
			platform = new Sas.User (Config.host_server, "cert");
			chatti = new DC.COMPONENET.Chatti (platform.context);

			AddHnadleErr (new System.Text.RegularExpressions.Regex (".*"), (erq, err) => {
				CCommonPopup popup = null;
				if (CModal.CountIf (p => p.kind == "popup_network_alram") > 0)
					return;
				if (string.IsNullOrEmpty (err.Message))
					popup = CModal.Make ("", err.ToErrstrOfSas ());
				else
					popup = CModal.Make ("", err.Message);

				popup.onHandleBtn += (p, str) => p.Close ();
				popup.kind = "popup_network_alram";
			});

			platform.requester_post_handler += (req, res, err) => {
				if (err == null)
					return;
				var exception = err as Sas.Exception;
				var desc = Enum.GetName (typeof(Sas.ERRNO), exception != null ? exception.ToErrnoOfSas () : Sas.ERRNO.UNKNOWN);

				foreach (var pr in mErrHandles) {
					if (pr.Key.IsMatch (desc))
						pr.Value (req, err);
				}
			};
		}
	}
}

