using System;
using System.Web;

namespace MicroCHAP.Server
{
	/// <summary>
	/// Note: if using DI, this should be registered with singleton lifespan
	/// </summary>
	public interface IChapServer
	{
		string GetChallengeToken();
		bool ValidateToken(string challenge, string response, string url, params SignatureFactor[] additionalFactors);
		bool ValidateToken(string challenge, string response, string url, IChapServerLogger logger, params SignatureFactor[] additionalFactors);
		bool ValidateRequest(HttpRequestBase request);
		bool ValidateRequest(HttpRequestBase request, IChapServerLogger logger);
		bool ValidateRequest(HttpRequestBase request, Func<HttpRequestBase, SignatureFactor[]> factorParser);
		bool ValidateRequest(HttpRequestBase request, Func<HttpRequestBase, SignatureFactor[]> factorParser, IChapServerLogger logger);
	}
}