using System.Text.RegularExpressions;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// Position of token matched.
	/// Represent for matched position and text.
	/// </summary>
	/// <typeparam name="TokenType">Token type.</typeparam>
	public class TokenMatch<TokenType> where TokenType : struct {
		/// <summary>
		/// Token type.
		/// </summary>
		public TokenType Type;

		/// <summary>
		/// Matched text.
		/// This text can revise in BeforeAddToken event.
		/// </summary>
		public string Text;

		/// <summary>
		/// Char index of matched text (in entire text).
		/// </summary>
		public int Index;

		/// <summary>
		/// Line number of matched text.
		/// </summary>
		public int LineNumber;

		/// <summary>
		/// Char index of matched line.
		/// </summary>
		public int LineIndex;

		/// <summary>
		/// Regex Match object when used matching.
		/// </summary>
		public Match Match;

		/// <summary>
		/// Matched text (before processing).
		/// </summary>
		string rawText;

		/// <summary>
		/// Matched text (before processing).
		/// </summary>
		public string RawText {
			get { return rawText; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public TokenMatch() {
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="text">Matched text.</param>
		public TokenMatch(string text) {
			rawText = text;
			this.Text = text;
		}
	}
}
