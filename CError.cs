using System;

namespace DC
{
	public enum ERRNO
	{
		UNKNOWN,
		MESSAGE,
		INVALID_MATCH_START,
	}

	public static class ExceptionExt
	{
		static Sas.Emaaper<DC.ERRNO> mapper = new Sas.Emaaper<ERRNO>();
		public static ERRNO ToErrnoOfDC(this System.Exception e)
		{
			var exception = e as Sas.Exception;
			return exception != null ? mapper.ToEnum(exception.code) : ERRNO.MESSAGE;
		}

		public static string ToErrstrOfDC (this System.Exception e)
		{
			var exception = e as Sas.Exception;
			return Enum.GetName (typeof(ERRNO), exception != null ? exception.ToErrnoOfDC() : ERRNO.MESSAGE);

		}

		public static long ToErrCode(this ERRNO e)
		{
			return mapper.ToHash (e);
		}
	}
}