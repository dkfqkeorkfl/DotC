using System;
using Sas;
using UniRx;
using Newtonsoft.Json;
namespace DC
{
	namespace COMPONENET
	{
		public class Chatti
		{
			Sas.Component.Context context { get; set; }
			public Chatti (Sas.Component.Context context)
			{
				this.context = context;
			}

			public UniRx.IObservable<bool> Likes(ulong room_id, bool like)
			{
				var json = new Newtonsoft.Json.Linq.JObject ();
				json ["room_id"] = room_id;
				json ["type"] = like ? "like" : "unlike";
				return this.context.Post ("/dc/chat/likes", json)
					.Select (_ => true);
			}
		}
	}
}

