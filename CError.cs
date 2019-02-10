using System;
using Sas;

namespace DC
{
	public class ExceptionWithMsg : Sas.Exception
	{
		public ExceptionWithMsg(string what) : base(ERRNO.MESSAGE, what)
		{
		}
	}
}