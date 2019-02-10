using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace DC
{
	public static class CUtil
	{
		public static IObservable<Unit> OnClickAsOptional (this UnityEngine.UI.Button btn)
		{
			return btn
				.OnClickAsObservable ()
				.ThrottleFirst (TimeSpan.FromSeconds (Config.ignore_btn_continuity))
				.First ();
		}
	}

}
