using System;
using System.Web;

namespace MicroCHAP.Server
{
	/// <summary>
	/// Functionalities needed to be a server of CHAP-authenticated data over HTTP.
	/// Requires a persistent store of challenge values so we can avoid replay attacks.
	/// </summary>
	public class ChapServer : IChapServer
	{
		private readonly ISignatureService _responseService;
		private readonly IChallengeStore _challengeStore;

		public ChapServer(ISignatureService responseService, IChallengeStore challengeStore)
		{
			_responseService = responseService;
			_challengeStore = challengeStore;
		}

		public int TokenValidityInMs { get; set; } = 600000;

		public virtual string GetChallengeToken()
		{
			var token = Guid.NewGuid().ToString();

			_challengeStore.AddChallenge(token, TokenValidityInMs);

			return token;
		}

		public virtual bool ValidateRequest(HttpRequestBase request)
		{
			return ValidateRequest(request, null);
		}

		public virtual bool ValidateRequest(HttpRequestBase request, Func<HttpRequestBase, SignatureFactor[]> factorParser)
		{
			// fallback headers are for compatibility with MicroCHAP 1.0 client implementations
			// See https://github.com/kamsar/MicroCHAP/issues/1 for why the change to different headers
			var authorize = request.Headers["X-MC-MAC"] ?? request.Headers["Authorization"];
			var challenge = request.Headers["X-MC-Nonce"] ?? request.Headers["X-Nonce"];

			if (authorize == null || challenge == null) return false;

			SignatureFactor[] factors = null;
			if (factorParser != null) factors = factorParser(request);

			return ValidateToken(challenge, authorize, request.Url.AbsoluteUri, factors);
		}

		public virtual bool ValidateToken(string challenge, string response, string url, params SignatureFactor[] additionalFactors)
		{
			if (!_challengeStore.ConsumeChallenge(challenge)) return false; // invalid or expired challenge

			// we now know the challenge was valid. But what about the response?
			return _responseService.CreateSignature(challenge, url, additionalFactors).Equals(response);
		}
	}
}