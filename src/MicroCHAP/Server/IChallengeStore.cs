namespace MicroCHAP.Server
{
	public interface IChallengeStore
	{
		/// <summary>
		/// Adds an issued challenge to the store
		/// </summary>
		/// <param name="challenge">Challenge value</param>
		/// <param name="expirationTimeInMsec">Expiration of validity in milliseconds</param>
		void AddChallenge(string challenge, int expirationTimeInMsec);

		/// <summary>
		/// Consumes a challenge. If the challenge is valid, it should be removed from the store.
		/// </summary>
		/// <returns>True if the challenge existed, false if it did not exist or was expired</returns>
		bool ConsumeChallenge(string challenge);
	}
}
