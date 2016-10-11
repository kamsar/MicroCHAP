using System.Net;

namespace MicroCHAP.Client
{
	public abstract class ChapClientServiceBase
	{
		private readonly string _remoteBaseUrl;
		private readonly string _challengeUrl;
		private readonly ISignatureService _responseService;

		protected ChapClientServiceBase(string remoteBaseUrl, string challengeUrl, ISignatureService responseService)
		{
			_remoteBaseUrl = remoteBaseUrl;
			_challengeUrl = challengeUrl;
			_responseService = responseService;
		}

		protected virtual string ConvertUrlToAbsolute(string relativeUrl)
		{
			if (relativeUrl.StartsWith("http")) return relativeUrl;
			if (relativeUrl.StartsWith("~")) relativeUrl = relativeUrl.Substring(1);
			if (!relativeUrl.StartsWith("/")) relativeUrl = "/" + relativeUrl;

			return _remoteBaseUrl + relativeUrl;
		}

		protected virtual WebClient CreateAuthenticatedWebClient(string url, params SignatureFactor[] additionalFactors)
		{
			var challenge = GetChallenge();
			var client = new WebClient();

			client.Headers.Add("X-MC-MAC", _responseService.CreateSignature(challenge, url, additionalFactors).SignatureHash);
			client.Headers.Add("X-MC-Nonce", challenge);

			return client;
		}

		protected virtual string GetChallenge()
		{
			var client = new WebClient();
			return client.DownloadString(_remoteBaseUrl + _challengeUrl);
		}
	}
}
