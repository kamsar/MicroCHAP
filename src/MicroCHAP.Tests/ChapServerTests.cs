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
		public void ValidateToken_ShouldReturnFalseIfChallengeDoesNotExist()
		{
			var service = CreateTestServer();

			service.ValidateToken("FAKE", "FAKE", "FAKE").Should().BeFalse();
		}

		[Fact]
		public void ValidateToken_ShouldReturnFalseIfChallengeIsTooOld()
		{
			var service = CreateTestServer();

			var token = service.GetChallengeToken();

			Thread.Sleep(3050);

			service.ValidateToken(token, "RESPONSE", "FAKE").Should().BeFalse();
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

		private IChapServer CreateTestServer()
		{
			var responseService = Substitute.For<ISignatureService>();
			responseService.CreateSignature(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<SignatureFactor>>()).Returns("RESPONSE");

			return new ChapServer(responseService, new InMemoryChallengeStore());
		}
	}
}
