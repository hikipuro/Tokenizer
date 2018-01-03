using Hikipuro.Text.Tokenizer;

namespace Tokenizer.Sample {
	/// <summary>
	/// Sample: tokenize JSON text.
	/// This class doesn't have to instanciate.
	/// To use this class, call JsonTokenizer.Tokenize() static method.
	/// </summary>
	class JsonTokenizer {
		/// <summary>
		/// Token type of JSON text.
		/// Each tokenizer shoud have another TokenType enum object.
		/// </summary>
		public enum TokenType {
			NewLine,
			Comma,
			Colon,
			OpenBrace,
			CloseBrace,
			OpenBracket,
			CloseBracket,
			Null,
			True,
			False,
			Number,
			String,
			Space
		}

		/// <summary>
		/// Tokeninze JSON text.
		/// </summary>
		/// <param name="text">JSON text.</param>
		/// <returns>Token list.</returns>
		public static TokenList<TokenType> Tokenize(string text) {
			// prepare Tokenizer object
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// prepare token patterns
			tokenizer.AddPattern(TokenType.NewLine, "\\G\r\n|\r|\n");
			tokenizer.AddPattern(TokenType.Comma, "\\G,");
			tokenizer.AddPattern(TokenType.Colon, "\\G:");
			tokenizer.AddPattern(TokenType.OpenBrace, "\\G{");
			tokenizer.AddPattern(TokenType.CloseBrace, "\\G}");
			tokenizer.AddPattern(TokenType.OpenBracket, @"\G\[");
			tokenizer.AddPattern(TokenType.CloseBracket, @"\G\]");
			tokenizer.AddPattern(TokenType.Null, "\\Gnull");
			tokenizer.AddPattern(TokenType.True, "\\Gtrue");
			tokenizer.AddPattern(TokenType.False, "\\Gfalse");
			tokenizer.AddPattern(TokenType.Number, @"\G\d+[.]?\d*");
			tokenizer.AddPattern(TokenType.String, @"\G""((?<=\\)""|[^\r\n""])*""");
			tokenizer.AddPattern(TokenType.Space, @"\G\s+");

			// this event will be dispatched when the token will add to list
			// - e.Cancel = true;
			//   this is specify that don't add to list
			tokenizer.BeforeAddToken += (object sender, BeforeAddTokenEventArgs<TokenType> e) => {
				if (e.TokenMatch.Type == TokenType.NewLine) {
					e.Cancel = true;
					return;
				}
				if (e.TokenMatch.Type == TokenType.Space) {
					e.Cancel = true;
					return;
				}

				// type matching (string)
				if (e.TokenMatch.Type == TokenType.String) {
					/*
					// for debugging
					Console.WriteLine(
						"token: {0} ({1},{2}): {3}: {4}",
						e.TokenMatch.Index,
						e.TokenMatch.LineNumber, e.TokenMatch.LineIndex,
						e.TokenMatch.Type, e.TokenMatch.Text
					);
					//*/
					// trim double quotes
					//string matchText = e.TokenMatch.Text;
					//matchText = matchText.Trim('"');
					//e.TokenMatch.Text = matchText;
				}
			};

			// this event will be dispatched when the token was added to list
			tokenizer.TokenAdded += (object sender, TokenAddedEventArgs<TokenType> e) => {
				/*
				// for debugging
				Console.WriteLine(
					"token: {0} ({1},{2}): {3}: {4}",
					e.Token.Index,
					e.Token.LineNumber, e.Token.LineIndex,
					e.Token.Type, e.Token.Text
				);
				//*/
			};

			// Tokenize
			TokenList<TokenType> tokens = tokenizer.Tokenize(text);

			// add EOF to last of list, if you want to have list guard
			/*
			Token<TokenType> tokenEOF = new Token<TokenType>();
			tokenEOF.Type = TokenType.EOF;
			tokens.Add(tokenEOF);
			//*/

			/*
			// show tokenized items (for debugging)
			foreach (Token<TokenType> token in tokens) {
				Console.WriteLine(
					"token: ({0},{1}): {2}: {3}",
					token.LineNumber, token.LineIndex,
					token.Type, token.Text
				);
			}
			//*/
			return tokens;
		}
	}
}
