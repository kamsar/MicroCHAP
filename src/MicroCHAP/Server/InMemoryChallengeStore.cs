using System;
using System.Collections.Concurrent;

namespace MicroCHAP.Server
{
	/// <summary>
	/// Stores challenges in an in-memory concurrent dictionary.
	/// Appropriate, when registered as a singleton in IoC, for single server environments.
	/// For multi-server environments without sticky sessions you'll need to put these somewhere shared.
	/// Such as a database. Or a shared cache (Redis, etc).
	/// </summary>
	public class InMemoryChallengeStore : IChallengeStore
	{
		private readonly ConcurrentDictionary<string, DateTime> _activeChallenges = new ConcurrentDictionary<string, DateTime>();

		public void AddChallenge(string challenge, int expirationTimeInMsec)
		{
			_activeChallenges.TryAdd(challenge, DateTime.UtcNow.AddMilliseconds(expirationTimeInMsec));
		}

		public bool ConsumeChallenge(string challenge)
		{
			CleanupExpiredTokens();

			DateTime existingChallengeTimestamp;

			// note that we remove the challenge if it exists: you get one shot
			if (!_activeChallenges.TryRemove(challenge, out existingChallengeTimestamp)) return false; // challenge was unknown

			// we know the token's timestamp was valid because we cleaned up expired tokens before getting it

			// we now know the challenge was valid.
			return true;
		}

		protected virtual void CleanupExpiredTokens()
		{
			foreach (var challenge in _activeChallenges)
			{
				DateTime temp;
				if (DateTime.UtcNow > challenge.Value)
					_activeChallenges.TryRemove(challenge.Key, out temp);
			}
		}
	}
}
