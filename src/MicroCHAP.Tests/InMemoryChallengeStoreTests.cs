using System.Threading;
using FluentAssertions;
using MicroCHAP.Server;
using Xunit;

namespace MicroCHAP.Tests
{
	public class InMemoryChallengeStoreTests
	{
		[Fact]
		public void ConsumeChallenge_ShouldReturnFalseIfChallengeDoesNotExist()
		{
			var store = CreateTestStore();

			store.ConsumeChallenge("FAKE").Should().BeFalse();
		}

		[Fact]
		public void ConsumeChallenge_ShouldReturnFalseIfChallengeIsTooOld()
		{
			var store = CreateTestStore();

			store.AddChallenge("FAKE", 100);

			Thread.Sleep(150);

			store.ConsumeChallenge("FAKE").Should().BeFalse();
		}

		[Fact]
		public void ConsumeChallenge_ShouldReturnTrue_IfTokenIsValid()
		{
			var store = CreateTestStore();

			store.AddChallenge("FAKE", 100);

			store.ConsumeChallenge("FAKE").Should().BeTrue();
		}

		[Fact]
		public void ConsumeChallenge_ShouldNotAllowReusingTokens()
		{
			var store = CreateTestStore();

			store.AddChallenge("FAKE", 100);

			store.ConsumeChallenge("FAKE").Should().BeTrue();
			store.ConsumeChallenge("FAKE").Should().BeFalse();
		}

		private IChallengeStore CreateTestStore()
		{
			return new InMemoryChallengeStore();
		}
	}
}
