using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace GameLauncher;

[Serializable]
public class UncompressionException : Exception
{
	private int mErrorCode;

	public int ErrorCode {
		[CompilerGenerated]
		get {
			return mErrorCode;
		}
	}

	public UncompressionException (int errorCode)
	{
		mErrorCode = errorCode;
	}

	public UncompressionException (int errorCode, string message)
		: base (message)
	{
		mErrorCode = errorCode;
	}

	public UncompressionException (int errorCode, string message, Exception innerException)
		: base (message, innerException)
	{
		mErrorCode = errorCode;
	}

	protected UncompressionException (SerializationInfo info, StreamingContext context)
		: base (info, context)
	{
	}
}
