using System;

namespace MicroCHAP
{
	public class SignatureFactor
	{
		public SignatureFactor(string key, string value)
		{
			if(key == null) throw new ArgumentNullException("key");
			if(value == null) throw new ArgumentNullException("value");

			Key = key;
			Value = value;
		}

		public string Key { get; private set; }
		public string Value { get; private set; }
	}
}
