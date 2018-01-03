using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// Tokenizer.
	/// </summary>
	/// <typeparam name="TokenType">Token type.</typeparam>
	public class Tokenizer<TokenType> where TokenType : struct {
		/// <summary>
		/// Event for a matched token will add to TokenList
		// - e.Cancel = true;
		//   this is specify that don't add to list
		/// </summary>
		public event BeforeAddTokenEventHandler<TokenType> BeforeAddToken;

		/// <summary>
		/// Event for a matched token was added to TokenList
		/// </summary>
		public event TokenAddedEventHandler<TokenType> TokenAdded;

		/// <summary>
		/// This context will create each Tokenize() method calling.
		/// </summary>
		public class Context {
			/// <summary>
			/// Processing target text.
			/// </summary>
			public string Text;

			/// <summary>
			/// List of start position of each line.
			/// </summary>
			public List<int> LineIndexList;

			/// <summary>
			/// Processing char index.
			/// </summary>
			public int Index = 0;

			/// <summary>
			/// Processing line number.
			/// </summary>
			public int LineNumber = 1;

			/// <summary>
			/// Processing char index of current line.
			/// </summary>
			public int LineIndex = 1;

			/// <summary>
			/// Match patterns.
			/// To add this list, use Tokenizer.AddPattern() method.
			/// </summary>
			public TokenPattern<TokenType>[] Patterns;

			/// <summary>
			/// Constructor.
			/// </summary>
			public Context() {
			}
		}

		/// <summary>
		/// To insert System.Threading.Sleep() in constant time.
		/// Defalt value: 0.
		/// Tokenize process loop reached "SleepWait" count when sleep once (token count).
		/// Don't sleep when this value is zero or more small value.
		/// </summary>
		public int SleepWait = 0;

		/// <summary>
		/// Token match pattern list.
		/// To add this list, use AddPattern() method.
		/// </summary>
		List<TokenPattern<TokenType>> patterns;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Tokenizer() {
			patterns = new List<TokenPattern<TokenType>>();
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~Tokenizer() {
			if (patterns != null) {
				patterns.Clear();
				patterns = null;
			}
			if (BeforeAddToken != null) {
				foreach (Delegate d in BeforeAddToken.GetInvocationList()) {
					BeforeAddToken -= (BeforeAddTokenEventHandler<TokenType>)d;
				}
			}
			if (TokenAdded != null) {
				foreach (Delegate d in TokenAdded.GetInvocationList()) {
					TokenAdded -= (TokenAddedEventHandler<TokenType>)d;
				}
			}
		}

		/// <summary>
		/// Add to a Regex match pattern.
		/// </summary>
		/// <param name="type">Token type.</param>
		/// <param name="patternText">Regex pattern text.</param>
		/// <returns>Added pattern object.</returns>
		public TokenPattern<TokenType> AddPattern(TokenType type, string patternText) {
			if (patternText == null || patternText == string.Empty) {
				return null;
			}
			if (HasPatternType(type)) {
				return null;
			}
			TokenPattern<TokenType> pattern;
			pattern = new TokenPattern<TokenType>(type, patternText);
			patterns.Add(pattern);
			return pattern;
		}

		/// <summary>
		/// Add to a Regex match pattern.
		/// </summary>
		/// <param name="type">Token type.</param>
		/// <param name="patternText">Regex pattern text.</param>
		/// <param name="options">Regex options.</param>
		/// <returns>Added pattern object.</returns>
		public TokenPattern<TokenType> AddPattern(TokenType type, string patternText, RegexOptions options) {
			if (patternText == null || patternText == string.Empty) {
				return null;
			}
			if (HasPatternType(type)) {
				return null;
			}
			TokenPattern<TokenType> pattern;
			pattern = new TokenPattern<TokenType>(type, patternText, options);
			patterns.Add(pattern);
			return pattern;
		}

		/// <summary>
		/// Remove Regex pattern.
		/// </summary>
		/// <param name="type">Token type.</param>
		public void RemovePattern(TokenType type) {
			foreach (TokenPattern<TokenType> pattern in patterns) {
				if (pattern.Type.Equals(type) == false) {
					continue;
				}
				patterns.Remove(pattern);
				break;
			}
		}

		/// <summary>
		/// Check for already have a pattern type specified in argument.
		/// </summary>
		/// <param name="type">Token type.</param>
		/// <returns>true: have, false: not have.</returns>
		public bool HasPatternType(TokenType type) {
			foreach (TokenPattern<TokenType> pattern in patterns) {
				if (pattern.Type.Equals(type)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Tokenize.
		/// Throw ParseException when failed match.
		/// </summary>
		/// <param name="text">target text.</param>
		/// <returns>Token list.</returns>
		public TokenList<TokenType> Tokenize(string text) {
			// return value
			TokenList<TokenType> tokens = new TokenList<TokenType>();

			if (text == null || text == string.Empty) {
				return tokens;
			}

			// create context object
			Context context = new Context();
			context.Text = text;

			// prepare patterns
			context.Patterns = new TokenPattern<TokenType>[patterns.Count];
			patterns.CopyTo(context.Patterns);

			// check start of line indices
			context.LineIndexList = CreateLineIndexList(text);

			// to Sleep() constant count
			int loopCount = 0;
			int sleepWait = SleepWait;

			// try to match for last text index
			while (context.Index < text.Length) {
				// try to match registered patterns
				TokenMatch<TokenType> tokenMatch = TryMatchToken(context);

				// throw exception when not matched
				if (tokenMatch == null) {
					ThrowParseException(context);
				}

				// matched

				// move char positions
				int matchLength = tokenMatch.Match.Length;
				context.Index += matchLength;
				context.LineIndex += matchLength;

				if (BeforeAddToken != null) {
					// dispatch event if registered event handlers
					// don't add token to list when Cancel is false
					BeforeAddTokenEventArgs<TokenType> args
						= new BeforeAddTokenEventArgs<TokenType>(tokenMatch);
					BeforeAddToken(this, args);
					if (args.Cancel == false) {
						AddToken(tokens, tokenMatch);
					}
				} else {
					// add token to list
					AddToken(tokens, tokenMatch);
				}

				// when arrive on new line position
				if (context.LineIndexList[context.LineNumber] == context.Index) {
					context.LineIndex = 1;
					context.LineNumber++;
				}

				// insert Sleep() constant
				if (sleepWait > 0) {
					loopCount++;
					if (loopCount > sleepWait) {
						loopCount = 0;
						System.Threading.Thread.Sleep(1);
					}
				}
			}

			return tokens;
		}

		/// <summary>
		/// Create one by one Tokenizer.
		/// </summary>
		/// <param name="text">target text.</param>
		/// <returns>Stepping tokenizer</returns>
		public SteppingTokenizer<TokenType> CreateSteppingTokenizer(string text) {
			if (text == null || text == string.Empty) {
				return null;
			}

			// create context object
			Context context = new Context();
			context.Text = text;

			// prepare patterns
			context.Patterns = new TokenPattern<TokenType>[patterns.Count];
			patterns.CopyTo(context.Patterns);

			// check start of line indices
			context.LineIndexList = CreateLineIndexList(text);

			SteppingTokenizer<TokenType> tokenizer = new SteppingTokenizer<TokenType>(context);
			return tokenizer;
		}

		/// <summary>
		/// Return each start of line index.
		/// </summary>
		/// <param name="text">target text.</param>
		/// <returns>List of index.</returns>
		private List<int> CreateLineIndexList(string text) {
			List<int> lineIndexList = new List<int>();
			lineIndexList.Add(0);

			Regex regex = new Regex("\r\n|\r|\n", RegexOptions.None);
			MatchCollection matches = regex.Matches(text);
			foreach (Match match in matches) {
				if (match.Success == false) {
					continue;
				}
				lineIndexList.Add(match.Index + match.Length);
			}

			lineIndexList.Add(0);
			return lineIndexList;
		}

		/// <summary>
		/// Try to match token.
		/// Return null when don't match.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <returns>Token match object.</returns>
		private TokenMatch<TokenType> TryMatchToken(Context context) {
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
			TokenMatch<TokenType> tokenMatch = new TokenMatch<TokenType>(match.Value);
			tokenMatch.Type = tokenPattern.Type;
			tokenMatch.Index = context.Index;
			tokenMatch.LineNumber = context.LineNumber;
			tokenMatch.LineIndex = context.LineIndex;
			tokenMatch.Match = match;
			//tokenMatch.Text = match.Value;

			return tokenMatch;
		}

		/// <summary>
		/// Add token to list.
		/// </summary>
		/// <param name="tokenList">Target token list.</param>
		/// <param name="tokenMatch">Match object to add.</param>
		private void AddToken(TokenList<TokenType> tokenList, TokenMatch<TokenType> tokenMatch) {
			tokenList.Add(tokenMatch);

			// dispatch event when added
			if (TokenAdded != null) {
				Token<TokenType> token = tokenList.Last();
				TokenAddedEventArgs<TokenType> args
					= new TokenAddedEventArgs<TokenType>(tokenList, token);
				TokenAdded(this, args);
			}
		}

		/// <summary>
		/// Get current line text.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <returns>line text.</returns>
		private string GetLine(Context context) {
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
		/// Throw new ParseException.
		/// </summary>
		/// <param name="context">Context.</param>
		private void ThrowParseException(Context context) {
			string lineText = GetLine(context);
			ParseException exception = new ParseException(string.Format(
				"Parse Error (Line:{0}, Index:{1}){2}{3}",
				context.LineNumber,
				context.LineIndex,
				Environment.NewLine,
				lineText
			));

			exception.Text = context.Text;
			exception.LineText = lineText;
			exception.Index = context.Index;
			exception.LineNumber = context.LineNumber;
			exception.LineIndex = context.LineIndex;

			throw exception;
		}
	}
}
