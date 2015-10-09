using System;
using System.Collections.Concurrent;
using System.Web;

namespace MicroCHAP.Server
{
	public class ChapServer : IChapServer
	{
		private readonly ISignatureService _responseService;
		private readonly ConcurrentDictionary<string, DateTime> _activeChallenges = new ConcurrentDictionary<string, DateTime>();

		public ChapServer(ISignatureService responseService)
		{
			_responseService = responseService;
		}

		protected virtual int TokenValidityInMs { get { return 3000; } }

		public virtual string GetChallengeToken()
		{
			var token = Guid.NewGuid().ToString();

			_activeChallenges.TryAdd(token, DateTime.UtcNow);

			return token;
		}

		public virtual bool ValidateRequest(HttpRequestBase request)
		{
			return ValidateRequest(request, null);
		}

		public virtual bool ValidateRequest(HttpRequestBase request, Func<HttpRequestBase, SignatureFactor[]> factorParser)
		{
			var authorize = request.Headers["Authorization"];
			var challenge = request.Headers["X-Nonce"];

			if (authorize == null || challenge == null) return false;

			SignatureFactor[] factors = null;
			if (factorParser != null) factors = factorParser(request);

			return ValidateToken(challenge, authorize, request.Url.AbsoluteUri, factors);
		}

		public virtual bool ValidateToken(string challenge, string response, string url, params SignatureFactor[] additionalFactors)
		{
			DateTime existingChallengeTimestamp;

			// note that we remove the challenge if it exists: you get one shot
			if (!_activeChallenges.TryRemove(challenge, out existingChallengeTimestamp)) return false; // challenge was unknown

			if ((DateTime.UtcNow - existingChallengeTimestamp).TotalMilliseconds > TokenValidityInMs) return false; // challenge was too old to be used

			CleanupExpiredTokens();

			// we now know the challenge was valid. But what about the response?
			return _responseService.CreateSignature(challenge, url, additionalFactors).Equals(response);
		}

		protected virtual void CleanupExpiredTokens()
		{
			foreach (var challenge in _activeChallenges)
			{
				DateTime temp;
				if ((DateTime.UtcNow - challenge.Value).TotalMilliseconds > TokenValidityInMs)
					_activeChallenges.TryRemove(challenge.Key, out temp);
			}
		}
	}
}