using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Timers;

namespace Hikipuro.Text {
	/// <summary>
	/// 文字列をトークンに分割するためのクラス.
	/// </summary>
	/// <typeparam name="TokenType"></typeparam>
	class Tokenizer<TokenType> where TokenType : struct {
		/// <summary>
		/// トークンのリストに追加する直前に呼ばれるイベントのデリゲート.
		/// </summary>
		/// <param name="tokenMatch"></param>
		/// <returns></returns>
		public delegate bool BeforeAddTokenEventHandler(TokenMatch<TokenType> tokenMatch);

		/// <summary>
		/// トークンのリストに追加する直前に呼ばれるイベント.
		/// </summary>
		public event BeforeAddTokenEventHandler BeforeAddToken;

		/// <summary>
		/// タイムアウト時間 (ミリ秒).
		/// デフォルト値: 10 秒.
		/// </summary>
		public int timeout = 10000;

		/// <summary>
		/// トークンのマッチ用パターン.
		/// AddPattern() で追加する.
		/// </summary>
		List<TokenPattern<TokenType>> patterns;

		/// <summary>
		/// 改行にマッチする正規表現.
		/// コンストラクタで作成する.
		/// </summary>
		Regex newLineRegex;

		/// <summary>
		/// 処理中の文字位置.
		/// </summary>
		int index = 0;

		/// <summary>
		/// 処理中の行の文字位置.
		/// </summary>
		int lineIndex = 0;

		/// <summary>
		/// 処理中の行番号.
		/// </summary>
		int lineNumber = 1;

		/// <summary>
		/// タイムアウトの処理用.
		/// 正規表現で無限ループになってしまう時があるため.
		/// </summary>
		Timer timer = new Timer();

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		public Tokenizer() {
			patterns = new List<TokenPattern<TokenType>>();
			newLineRegex = new Regex("\r\n|\r|\n", RegexOptions.Compiled);
		}

		/// <summary>
		/// スペース文字列を削除する.
		/// (調整中)
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public string TrimSpaces(string text) {
			return Regex.Replace(text, @"[\f\t\v\x85\p{Z}]", string.Empty);
		}

		/// <summary>
		/// 正規表現のマッチパターンを追加する.
		/// </summary>
		/// <param name="type">トークンの種類.</param>
		/// <param name="patternText">マッチ用の正規表現文字列.</param>
		/// <returns></returns>
		public TokenPattern<TokenType> AddPattern(TokenType type, string patternText) {
			if (HasPatternType(type)) {
				return null;
			}
			TokenPattern<TokenType> pattern;
			pattern = new TokenPattern<TokenType>(type, patternText);
			patterns.Add(pattern);
			return pattern;
		}

		/// <summary>
		/// 引数で指定したトークンの種類がパターンに追加されているかチェックする.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool HasPatternType(TokenType type) {
			foreach (TokenPattern<TokenType> pattern in patterns) {
				if (pattern.type.Equals(type)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 正規表現のマッチパターンを追加する.
		/// </summary>
		/// <param name="type">トークンの種類.</param>
		/// <param name="patternText">マッチ用の正規表現文字列.</param>
		/// <param name="options">正規表現のオプション.</param>
		/// <returns></returns>
		public TokenPattern<TokenType> AddPattern(TokenType type, string patternText, RegexOptions options) {
			TokenPattern<TokenType> pattern;
			pattern = new TokenPattern<TokenType>(type, patternText, options);
			patterns.Add(pattern);
			return pattern;
		}

		/// <summary>
		/// 登録されたパターンを使ってトークンに分割する.
		/// 失敗した場合, "Parse Error" を例外として投げる.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public TokenList<TokenType> Tokenize(string text) {
			if (text == null || text == string.Empty) {
				return null;
			}
			
			// メンバ変数の初期化
			index = 0;
			lineIndex = 0;
			lineNumber = 1;

			// 改行位置のチェック用
			bool lineResetFlag = false;

			// Sleep() を定期的に入れる用
			int count = 0;

			// タイムアウトのチェック
			timer = new Timer();
			timer.Elapsed += (object sender, ElapsedEventArgs e) => {
				throw new TimeoutException("Timeout: Tokenizer.Tokenize()");
			};
			timer.Interval = timeout;
			timer.Start();

			// 戻り値
			TokenList<TokenType> tokens = new TokenList<TokenType>();

			// テキストの終わりまでマッチを試す
			while (index < text.Length) {
				// 改行チェック
				lineResetFlag = false;
				Match match = newLineRegex.Match(text, index);
				if (match.Index == index) {
					lineResetFlag = true;
				}

				// 登録されたパターンでマッチを試す
				TokenMatch<TokenType> tokenMatch = TryMatchToken(text);

				// マッチしなかった場合
				if (tokenMatch == null) {
					throw new Exception(string.Format(
						"Parse Error (Line:{0}, Index:{1})",
						lineNumber,
						lineIndex
					));
				}

				// マッチした場合
				bool addFlag = true;
				if (BeforeAddToken != null) {
					addFlag = BeforeAddToken(tokenMatch);
				}

				// トークンをリストに追加する
				if (addFlag) {
					Token<TokenType> token = new Token<TokenType>();
					token.type = tokenMatch.type;
					token.text = tokenMatch.value;
					token.lineNumber = lineNumber;
					token.lineIndex = lineIndex - token.text.Length;
					tokens.Add(token);
				}

				// ループの開始位置で改行文字が見つかった時
				if (lineResetFlag) {
					lineIndex = 0;
					lineNumber++;
				}

				// Sleep() を定期的に入れる
				count++;
				if (count > 1000) {
					count = 0;
					System.Threading.Thread.Sleep(1);
				}
			}
			timer.Stop();
			return tokens;
		}

		/// <summary>
		/// トークンのマッチを試す.
		/// マッチしなかった場合, null を返す.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private TokenMatch<TokenType> TryMatchToken(string text) {
			TokenPattern<TokenType> tokenPattern = null;
			Match match = null;

			// すべてのパターンを試す
			foreach (TokenPattern<TokenType> pattern in patterns) {
				match = pattern.regex.Match(text, index);
				if (match.Index != index) {
					continue;
				}
				// 長さ 0 でマッチする場合があるので回避
				if (match.Length <= 0) {
					continue;
				}
				tokenPattern = pattern;
				index += match.Length;
				lineIndex += match.Length;
				break;
			}

			// マッチしなかった場合
			if (tokenPattern == null) {
				return null;
			}

			// マッチした場合
			TokenMatch<TokenType> tokenMatch = new TokenMatch<TokenType>();
			tokenMatch.type = tokenPattern.type;
			tokenMatch.index = index;
			tokenMatch.lineNumber = lineNumber;
			tokenMatch.match = match;
			tokenMatch.value = match.Value;
			return tokenMatch;
		}
	}
}
