using System;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// One of the Token.
	/// </summary>
	/// <typeparam name="TokenType">Token type.</typeparam>
	public class Token<TokenType> where TokenType : struct {
		/// <summary>
		/// Token type.
		/// </summary>
		public TokenType Type;

		/// <summary>
		/// Matched text (regex matched).
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
		/// This Token in this TokenList.
		/// </summary>
		public TokenList<TokenType> TokenList;

		/// <summary>
		/// Tag.
		/// You can set a any value or object freely for set a mark.
		/// This library don't care this Tag value.
		/// </summary>
		public object Tag;

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
		/// Matched text length (char count).
		/// </summary>
		public int Length {
			get { return Text.Length; }
		}

		/// <summary>
		/// Position of Index + Length.
		/// </summary>
		public int RightIndex {
			get { return Index + Text.Length; }
		}

		/// <summary>
		/// Position of LineIndex + Length.
		/// </summary>
		public int RightLineIndex {
			get { return LineIndex + Text.Length; }
		}

		/// <summary>
		/// Check for this token is last item in TokenList.
		/// true: last item.
		/// false: not last item.
		/// </summary>
		public bool IsLast {
			get {
				if (TokenList == null) {
					return false;
				}
				return TokenList.Last(0) == this;
			}
		}

		/// <summary>
		/// Get next item in TokenList.
		/// null: next item is empty.
		/// </summary>
		/// <returns>Next item.</returns>
		public Token<TokenType> Next {
			get {
				if (TokenList == null) {
					return null;
				}
				return TokenList.Next(this);
			}
		}

		/// <summary>
		/// Get previous item in TokenList.
		/// </summary>
		/// <returns>Previous item.</returns>
		public Token<TokenType> Prev {
			get {
				if (TokenList == null) {
					return null;
				}
				return TokenList.Prev(this);
			}
		}

		/// <summary>
		/// Create Token object from TokenMatch object.
		/// </summary>
		/// <param name="tokenMatch">TokenMatch object (base).</param>
		/// <returns>Token object (created).</returns>
		public static Token<TokenType> FromTokenMatch(TokenMatch<TokenType> tokenMatch) {
			Token<TokenType> token = new Token<TokenType>(tokenMatch.RawText);
			token.Type = tokenMatch.Type;
			token.Text = tokenMatch.Text;
			token.Index = tokenMatch.Index;
			token.LineNumber = tokenMatch.LineNumber;
			token.LineIndex = tokenMatch.LineIndex;
			return token;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public Token() {
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="text">Matched text.</param>
		public Token(string text) {
			rawText = text;
			this.Text = text;
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~Token() {
			TokenList = null;
			Tag = null;
		}

		/// <summary>
		/// Check for this token is same type passed in tokenType arg.
		/// </summary>
		/// <param name="tokenType">Token type.</param>
		/// <returns>true: same, false: not same.</returns>
		public bool IsTypeOf(TokenType tokenType) {
			return Type.Equals(tokenType);
		}

		/// <summary>
		/// Check for this token is within a group passed in tokenTypeGroup arg.
		/// </summary>
		/// <param name="tokenTypeGroup">Token type group.</param>
		/// <returns>true: within, false: not within.</returns>
		public bool IsMemberOf(TokenTypeGroup<TokenType> tokenTypeGroup) {
			if (tokenTypeGroup == null) {
				return false;
			}
			return tokenTypeGroup.Contains(Type);
		}

		/// <summary>
		/// Get distance in char count, compare this token and another token.
		/// This method compares start position.
		/// </summary>
		/// <param name="token">Token used for compare.</param>
		/// <returns>Char count.</returns>
		public int GetDistance(Token<TokenType> token) {
			return Math.Abs(Index - token.Index);
		}

		/// <summary>
		/// Check for this token position is front of the another token.
		/// </summary>
		/// <param name="token">Token used for compare.</param>
		/// <returns>true: before, false: after.</returns>
		public bool IsBefore(Token<TokenType> token) {
			return Index < token.Index;
		}

		/// <summary>
		/// Check for this token position is back of the another token.
		/// </summary>
		/// <param name="token">Token used for compare.</param>
		/// <returns>true: after, false: before.</returns>
		public bool IsAfter(Token<TokenType> token) {
			return Index > token.Index;
		}

		/// <summary>
		/// Check for this token position is adjacent of the another token.
		/// </summary>
		/// <param name="token">Token used for compare.</param>
		/// <returns>true: adjacent, false: not adjacent.</returns>
		public bool IsNeighbor(Token<TokenType> token) {
			if (this.IsBefore(token)) {
				return Index + RawText.Length == token.Index;
			}
			return token.Index + token.RawText.Length == Index;
		}
	}
}
