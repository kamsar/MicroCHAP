namespace MicroCHAP.Server
{
	public interface IChapServerLogger
	{
		void RejectedDueToMissingHttpHeaders();
		void RejectedDueToInvalidChallenge(string challengeProvided, string url);
		void RejectedDueToInvalidSignature(string challenge, string signatureProvided, SignatureResult signatureExpected);
	}
}
