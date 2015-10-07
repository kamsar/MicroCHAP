using System.Linq;
using FluentAssertions;
using Xunit;

namespace MicroCHAP.Tests
{
	public class SignatureServiceTests
	{
		[Fact]
		public void ShouldReturnExpectedHash()
		{
			var service = new SignatureService("TEST");

			var response = service.CreateResponse("CHALLENGE", "http://URL.com/foo", Enumerable.Empty<SignatureFactor>());

			response.Should().Be("kuKRmO5ds6wyiI15C7XuUdaa4oXCR0SjsLGLu911pFIYme+8JRaoLj6n7RjKqASYcAPMAg7dlat7jQDlzLkNQg==");
		}

		[Fact]
		public void ShouldReturnExpectedHash_WithFactors()
		{
			var service = new SignatureService("TEST");

			var response = service.CreateResponse("CHALLENGE", "http://URL.com/foo", new[] { new SignatureFactor("TestFactor", "TestValue"), new SignatureFactor("SecondTest", "SecondValue") });

			response.Should().Be("1kLQk1bHuIShUZTqwyQGoI0gToacPK/LGmGREZa3WT4QV0CPVbOqUu91TWP5x2LpXLFR4G6e29C60KSztGnJ2A==");
		}

		[Fact]
		public void ShouldReturnEquivalentHashes_WhenFactorsAreInDifferentOrder()
		{
			var service = new SignatureService("TEST");

			var factor1 = new SignatureFactor("TestFactor", "TestValue");
			var factor2 = new SignatureFactor("SecondTest", "SecondValue");

			var response = service.CreateResponse("CHALLENGE", "http://URL.com/foo", new[] { factor1, factor2 });
			var response2 = service.CreateResponse("CHALLENGE", "http://URL.com/foo", new[] { factor2, factor1 });

			response.Should().Be(response2);
		}
	}
}
