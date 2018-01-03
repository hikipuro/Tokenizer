using Hikipuro.Text.Tokenizer;
using System;

namespace Tokenizer.Sample {
	/// <summary>
	/// Sample: tokenize CSV text.
	/// This class doesn't have to instanciate.
	/// To use this class, call CSVTokenizer.Tokenize() static method.
	/// </summary>
	class CSVTokenizer {
		/// <summary>
		/// Token type of CSV text.
		/// Each tokenizer shoud have another TokenType enum object.
		/// </summary>
		public enum TokenType {
			Comma,
			Number,
			String,
			NewLine,
			// EOF,
		}

		/// <summary>
		/// Tokeninze CSV text.
		/// </summary>
		/// <param name="text">CSV text.</param>
		/// <returns>Token list.</returns>
		public static TokenList<TokenType> Tokenize(string text) {
			// prepare Tokenizer object
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// prepare token patterns
			tokenizer.AddPattern(TokenType.Comma, "\\G,");
			tokenizer.AddPattern(TokenType.Number, @"\G\d+[.]?\d*");
			// - in double quotes, permit to use new line
			//tokenizer.AddPattern(TokenType.String, @"\G""((?<=\\)""|[^""])*""");
			// - in double quotes, not permit to use new line
			tokenizer.AddPattern(TokenType.String, @"\G""((?<=\\)""|[^\r\n""])*""");
			tokenizer.AddPattern(TokenType.NewLine, "\\G\r\n|\r|\n");

			// this event will be dispatched when the token will add to list
			// - e.Cancel = true;
			//   this is specify that don't add to list
			tokenizer.BeforeAddToken += (object sender, BeforeAddTokenEventArgs<TokenType> e) => {
				/*
				if (e.TokenMatch.Type == TokenType.NewLine) {
					e.Cancel = true;
					return;
				}
				//*/

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
					string matchText = e.TokenMatch.Text;
					matchText = matchText.Trim('"');
					e.TokenMatch.Text = matchText;
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
