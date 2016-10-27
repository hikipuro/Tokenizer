using System.Collections.Generic;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// トークンの種類のグループ.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	public class TokenTypeGroup<TokenType> : List<TokenType> where TokenType : struct {
	}
}
