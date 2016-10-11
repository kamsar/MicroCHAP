using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using MicroCHAP.Server;
using NSubstitute;
using Xunit;

namespace MicroCHAP.Tests
{
	public class ChapServerTests
	{
		[Fact]
		public void GetChallengeToken_ShouldReturnUniqueChallenges()
		{
			var service = CreateTestServer();

			var challenge1 = service.GetChallengeToken();

			challenge1.Should().NotBe(service.GetChallengeToken());
		}

		[Fact]
		public void GetChallengeToken_ShouldBeAlphanumeric()
		{
			var service = CreateTestServer();

			var challenge = service.GetChallengeToken();

			challenge.Should().MatchRegex("^[A-Za-z0-9]+$");
		}

		[Fact]
		public void ValidateToken_ShouldReturnFalseIfChallengeDoesNotExist()
		{
			var service = CreateTestServer();

			var log = Substitute.For<IChapServerLogger>();

			service.ValidateToken("FAKE", "FAKE", "FAKE", log).Should().BeFalse();

			log.Received().RejectedDueToInvalidChallenge("FAKE", "FAKE");
		}

		[Fact]
		public void ValidateToken_ShouldReturnFalseIfChallengeIsTooOld()
		{
			var service = CreateTestServer();

			var log = Substitute.For<IChapServerLogger>();

			((ChapServer)service).TokenValidityInMs = 300;

			var token = service.GetChallengeToken();

			Thread.Sleep(350);

			service.ValidateToken(token, "RESPONSE", "FAKE", log).Should().BeFalse();
			log.Received().RejectedDueToInvalidChallenge(token, "FAKE");
		}

		[Fact]
		public void ValidateToken_ShouldReturnTrue_IfTokenIsValid()
		{
			var service = CreateTestServer();

			var token = service.GetChallengeToken();

			service.ValidateToken(token, "RESPONSE", "FAKE").Should().BeTrue();
		}

		[Fact]
		public void ValidateToken_ShouldNotAllowReusingTokens()
		{
			var service = CreateTestServer();

			var token = service.GetChallengeToken();

			service.ValidateToken(token, "RESPONSE", "FAKE").Should().BeTrue();
			service.ValidateToken(token, "RESPONSE", "FAKE").Should().BeFalse();
		}

		[Fact]
		public void ValidateToken_ShouldRejectInvalidSignature()
		{
			var service = CreateTestServer();

			var log = Substitute.For<IChapServerLogger>();

			var token = service.GetChallengeToken();

			service.ValidateToken(token, "BADRESPONSE", "FAKE", log).Should().BeFalse();
			log.Received().RejectedDueToInvalidSignature(token, "BADRESPONSE", Arg.Is<SignatureResult>(result => result.SignatureHash.Equals("RESPONSE")));
		}

		private IChapServer CreateTestServer()
		{
			var responseService = Substitute.For<ISignatureService>();
			var signatureResult = new SignatureResult { SignatureHash = "RESPONSE" };
			responseService.CreateSignature(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<SignatureFactor>>()).Returns(signatureResult);

			return new ChapServer(responseService, new InMemoryChallengeStore(null));
		}
	}
}
