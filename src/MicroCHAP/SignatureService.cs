using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MicroCHAP
{
	public class SignatureService : ISignatureService
	{
		private readonly string _sharedSecret;

		public SignatureService(string sharedSecret)
		{
			_sharedSecret = sharedSecret;
		}

		public string CreateSignature(string challenge, string url, IEnumerable<SignatureFactor> signatureFactors)
		{
			if (signatureFactors == null) signatureFactors = Enumerable.Empty<SignatureFactor>();

			List<string> factors = signatureFactors
					.OrderBy(factor => factor.Key.ToUpperInvariant())
					.Select(factor => string.Concat(factor.Key, "^", factor.Value))
					.ToList();

			factors.Add(challenge);
			factors.Add(_sharedSecret);
			factors.Add(ProcessUrl(url));

			var signature = string.Join("|", factors);
			using (SHA512 sha = new SHA512Managed())
			{
				var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(signature));
				return Convert.ToBase64String(hash);
			}
		}

		private string ProcessUrl(string url)
		{
			var uri = new Uri(url);
			return (uri.Host + uri.PathAndQuery).ToUpperInvariant();
		}
	}
}
