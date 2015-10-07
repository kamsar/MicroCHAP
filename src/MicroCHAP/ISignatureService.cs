using System.Collections.Generic;

namespace MicroCHAP
{
	public interface ISignatureService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="challenge">The challenge received from the remote host</param>
		/// <param name="url">The URL we're requesting</param>
		/// <param name="signatureFactors">Any additional factors required to authenticate the request (post params, etc); the remote host must look for the same parameters</param>
		/// <returns></returns>
		string CreateSignature(string challenge, string url, IEnumerable<SignatureFactor> signatureFactors);
	}
}