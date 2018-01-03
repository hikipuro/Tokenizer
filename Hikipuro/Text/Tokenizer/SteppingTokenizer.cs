using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// One by one Tokenizer.
	/// This instance will created by Tokenizer.CreateSteppingTokenizer() static method.
	/// </summary>
	/// <typeparam name="TokenType"></typeparam>
	public class SteppingTokenizer<TokenType> : IEnumerable<Token<TokenType>> where TokenType : struct {
		/// <summary>
		/// Context for tokenize.
		/// </summary>
		Tokenizer<TokenType>.Context context;

		/// <summary>
		/// Current token.
		/// </summary>
		Token<TokenType> token;

		/// <summary>
		/// List of matched tokens.
		/// </summary>
		TokenList<TokenType> tokens;

		/// <summary>
		/// Current token.
		/// </summary>
		public Token<TokenType> Current {
			get { return token; }
		}

		/// <summary>
		/// Check for tokenizer has next items.
		/// true: have, false: not have.
		/// </summary>
		public bool HasNext {
			get { return context.Index < context.Text.Length; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="context"></param>
		public SteppingTokenizer(Tokenizer<TokenType>.Context context) {
			this.context = context;
			tokens = new TokenList<TokenType>();
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~SteppingTokenizer() {
			token = null;
			if (tokens != null) {
				tokens.Clear();
				tokens = null;
			}
		}

		/// <summary>
		/// Reset processing position.
		/// </summary>
		public void Reset() {
			context.Index = 0;
			context.LineNumber = 1;
			context.LineIndex = 1;
			token = null;
			tokens.Clear();
		}

		/// <summary>
		/// Get next item.
		/// Return null when not matched.
		/// </summary>
		/// <returns>Token.</returns>
		public Token<TokenType> Next() {
			// try to match patterns in registered patterns
			Token<TokenType> token = TryMatchToken(context);

			// return null when not matching
			if (token == null) {
				return null;
			}

			// pattern matched
			tokens.Add(token);

			// move char positions
			int matchLength = token.Length;
			context.Index += matchLength;
			context.LineIndex += matchLength;
			
			// when arrive on new line position
			if (context.LineIndexList[context.LineNumber] == context.Index) {
				context.LineIndex = 1;
				context.LineNumber++;
			}

			this.token = token;
			return token;
		}

		/// <summary>
		/// Get next item, type of tokenType arg.
		/// Return null when not matched.
		/// </summary>
		/// <param name="tokenType">Token type.</param>
		/// <returns>Token.</returns>
		public Token<TokenType> Next(TokenType tokenType) {
			// try to match patterns in registered patterns
			Token<TokenType> token = TryMatchToken(context, tokenType);

			// return null when not matching
			if (token == null) {
				return null;
			}

			// pattern matched
			tokens.Add(token);

			// move char positions
			int matchLength = token.Length;
			context.Index += matchLength;
			context.LineIndex += matchLength;

			// when arrive on new line position
			if (context.LineIndexList[context.LineNumber] == context.Index) {
				context.LineIndex = 1;
				context.LineNumber++;
			}

			this.token = token;
			return token;
		}

		/// <summary>
		/// Back to processing point to the previous token.
		/// Return null when the previous token does not exist.
		/// </summary>
		/// <returns>Previous token.</returns>
		public Token<TokenType> Back() {
			if (token == null) {
				context.Index = 0;
				context.LineNumber = 1;
				context.LineIndex = 1;
				return null;
			}

			if (tokens.Count == 1) {
				token = null;
				tokens.RemoveAt(0);
				context.Index = 0;
				context.LineNumber = 1;
				context.LineIndex = 1;
				return null;
			}

			// get the previous token
			int position = tokens.Count - 1;
			Token<TokenType> prevToken = tokens[position - 1];
			tokens.RemoveAt(position);

			// back to processing point
			context.Index = token.Index;
			context.LineNumber = token.LineNumber;
			context.LineIndex = token.LineIndex;

			token = prevToken;
			return token;
		}

		/// <summary>
		/// Check for next item will match type, type of tokenType arg.
		/// </summary>
		/// <param name="tokenType">Token type.</param>
		/// <returns>true: match, false: not match.</returns>
		public bool IsMatchNext(TokenType tokenType) {
			// traverse all patterns
			foreach (TokenPattern<TokenType> pattern in context.Patterns) {
				// continue when don't match type
				if (pattern.Type.Equals(tokenType) == false) {
					continue;
				}
				Match match = pattern.Regex.Match(context.Text, context.Index);

				// matching failed
				if (!match.Success) {
					return false;
				}
				// matching failed most front position
				if (match.Index != context.Index) {
					return false;
				}
				// to avoid zero length matching
				if (match.Length <= 0) {
					return false;
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Get current line text.
		/// </summary>
		/// <returns>line text.</returns>
		public string GetLine() {
			return GetLine(context);
		}

		/// <summary>
		/// Try to match token.
		/// Return null when not matched.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <returns>Token.</returns>
		private Token<TokenType> TryMatchToken(Tokenizer<TokenType>.Context context) {
			TokenPattern<TokenType> tokenPattern = null;
			Match match = null;

			// traverse all patterns
			foreach (TokenPattern<TokenType> pattern in context.Patterns) {
				match = pattern.Regex.Match(context.Text, context.Index);

				// matching failed
				if (!match.Success) {
					continue;
				}
				// matching failed most front position
				if (match.Index != context.Index) {
					continue;
				}
				// to avoid zero length matching
				if (match.Length <= 0) {
					continue;
				}
				// matched
				tokenPattern = pattern;
				break;
			}

			// not matched
			if (tokenPattern == null) {
				return null;
			}

			// matched
			Token<TokenType> token = new Token<TokenType>(match.Value);
			token.Type = tokenPattern.Type;
			token.Index = context.Index;
			token.LineNumber = context.LineNumber;
			token.LineIndex = context.LineIndex;
			return token;
		}

		/// <summary>
		/// Try to match token, type of tokenType arg.
		/// Return null when not matched.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="tokenType">Token type.</param>
		/// <returns>Token.</returns>
		private Token<TokenType> TryMatchToken(Tokenizer<TokenType>.Context context, TokenType tokenType) {
			TokenPattern<TokenType> tokenPattern = null;
			Match match = null;

			// traverse all patterns
			foreach (TokenPattern<TokenType> pattern in context.Patterns) {
				// continue when don't match type
				if (pattern.Type.Equals(tokenType) == false) {
					continue;
				}

				match = pattern.Regex.Match(context.Text, context.Index);

				// matching failed
				if (!match.Success) {
					continue;
				}
				// matching failed most front position
				if (match.Index != context.Index) {
					continue;
				}
				// to avoid zero length matching
				if (match.Length <= 0) {
					continue;
				}
				// matched
				tokenPattern = pattern;
				break;
			}

			// not matched
			if (tokenPattern == null) {
				return null;
			}

			// matched
			Token<TokenType> token = new Token<TokenType>(match.Value);
			token.Type = tokenPattern.Type;
			token.Index = context.Index;
			token.LineNumber = context.LineNumber;
			token.LineIndex = context.LineIndex;
			return token;
		}

		/// <summary>
		/// Get current line text.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <returns>Line text.</returns>
		private string GetLine(Tokenizer<TokenType>.Context context) {
			int lineNumber = context.LineNumber;
			lineNumber--;
			if (lineNumber < 0 || lineNumber >= context.LineIndexList.Count) {
				return string.Empty;
			}
			int index = context.LineIndexList[lineNumber];
			if (index >= context.Text.Length) {
				return string.Empty;
			}
			Regex regex = new Regex("\r\n|\r|\n", RegexOptions.None);
			Match match = regex.Match(context.Text, index);
			if (match.Success) {
				string lineText = context.Text.Substring(index, match.Index - index);
				//lineText.Replace("\t", "    ");
				return lineText;
			}
			return context.Text.Substring(index);
		}

		/// <summary>
		/// Implement of IEnumerable interface.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Token<TokenType>> GetEnumerator() {
			while (HasNext) {
				Token<TokenType> token = Next();
				yield return token;
			}
		}

		/// <summary>
		/// Implement of IEnumerable interface.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
