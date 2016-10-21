using Hikipuro.Text;
using System;

namespace Tokenizer.Sample {
	/// <summary>
	/// CSV ファイルを分解するサンプル.
	/// インスタンスは作らず, 直接 CSVTokenizer.Tokenize() メソッドを呼ぶ.
	/// </summary>
	class CSVTokenizer {
		/// <summary>
		/// CSV ファイルで使用するトークンの種類.
		/// </summary>
		public enum TokenType {
			Comma,
			NewLine,
			Number,
			String,
			// EOF,
		}

		/// <summary>
		/// 渡された CSV ファイルを分解する.
		/// </summary>
		/// <param name="text">CSV ファイル.</param>
		/// <returns>トークンのリスト.</returns>
		public static TokenList<TokenType> Tokenize(string text) {
			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			tokenizer.AddPattern(TokenType.Comma, ",");
			tokenizer.AddPattern(TokenType.NewLine, "\r\n|\r|\n");
			tokenizer.AddPattern(TokenType.Number, @"\d+[.]?\d*");
			// - ダブルクォート内での改行を許可する文字列
			//tokenizer.AddPattern(TokenType.String, @"""((?<=\\)""|[^""])*""");
			// - ダブルクォート内での改行を許可しない文字列
			tokenizer.AddPattern(TokenType.String, @"""((?<=\\)""|[^\r\n""])*""");

			// リストにトークンを追加する直前に発生するイベント
			// - 戻り値 true で追加, false で追加しない
			tokenizer.BeforeAddToken += (TokenMatch<TokenType> tokenMatch) => {
				/*
				if (tokenMatch.type == TokenType.NewLine) {
					return false;
				}
				//*/

				// 文字列にマッチした場合
				if (tokenMatch.type == TokenType.String) {
					/*
					// デバッグ用
					Console.WriteLine(
						"token: {0} ({1},{2}): {3}: {4}",
						tokenMatch.index,
						tokenMatch.lineNumber, tokenMatch.lineIndex,
						tokenMatch.type, tokenMatch.text
					);
					//*/
					// 前後からダブルクォートを取り除く
					string matchText = tokenMatch.text;
					matchText = matchText.Trim('"');
					tokenMatch.text = matchText;
				}

				return true;
			};

			// リストにトークンを追加した直後に発生するイベント
			tokenizer.TokenAdded += (TokenList<TokenType> tokenList, Token<TokenType> token) => {
				/*
				// デバッグ用
				Console.WriteLine(
					"token: {0} ({1},{2}): {3}: {4}",
					token.index,
					token.lineNumber, token.lineIndex,
					token.type, token.text
				);
				//*/
			};

			// タイムアウト時間を設定する (ミリ秒)
			//tokenizer.timeout = 1000;

			// トークンに分解する
			TokenList<TokenType> tokens = tokenizer.Tokenize(text);

			// 末尾に EOF を追加して, 番兵にする場合
			//Token<TokenType> tokenEOF = new Token<TokenType>();
			//tokenEOF.type = TokenType.EOF;
			//tokens.Add(tokenEOF);

			/*
			// 分解した内容を表示する (デバッグ用)
			foreach (Token<TokenType> token in tokens) {
				Console.WriteLine(
					"token: ({0},{1}): {2}: {3}",
					token.lineNumber, token.lineIndex,
					token.type, token.text
				);
			}
			//*/
			return tokens;
		}
	}
}
