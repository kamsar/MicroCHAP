namespace MicroCHAP.Server
{
	public interface IChallengeStoreLogger
	{
		/// <summary>
		/// Called when a challenge fails due to being unknown in the challenge store
		/// </summary>
		void ChallengeUnknown(string challenge);

		/// <summary>
		/// Called _when an expired challenge is removed_. Note that the challenge may or may not be
		/// a challenge you are currently requesting be validated.
		/// </summary>
		/// <param name="challenge">The challenge that was invalidated due to timeout</param>
		void ChallengeExpired(string challenge);
	}
}
