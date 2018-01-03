using System.Collections.Generic;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// A token type group.
	/// </summary>
	/// <typeparam name="TokenType">Token type.</typeparam>
	public class TokenTypeGroup<TokenType> : List<TokenType> where TokenType : struct {
	}
}
