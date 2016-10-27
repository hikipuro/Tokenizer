using Hikipuro.Text.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tokenizer.Sample {
	class JsonTokenizer {
		/// <summary>
		/// JSON ファイルで使用するトークンの種類.
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
		/// 渡された JSON ファイルを分解する.
		/// </summary>
		/// <param name="text">JSON ファイル.</param>
		/// <returns>トークンのリスト.</returns>
		public static TokenList<TokenType> Tokenize(string text) {
			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
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

			// リストにトークンを追加する直前に発生するイベント
			// - e.Cancel = true; で追加しない
			tokenizer.BeforeAddToken += (object sender, BeforeAddTokenEventArgs<TokenType> e) => {
				if (e.TokenMatch.Type == TokenType.NewLine) {
					e.Cancel = true;
					return;
				}
				if (e.TokenMatch.Type == TokenType.Space) {
					e.Cancel = true;
					return;
				}

				// 文字列にマッチした場合
				if (e.TokenMatch.Type == TokenType.String) {
					/*
					// デバッグ用
					Console.WriteLine(
						"token: {0} ({1},{2}): {3}: {4}",
						e.TokenMatch.Index,
						e.TokenMatch.LineNumber, e.TokenMatch.LineIndex,
						e.TokenMatch.Type, e.TokenMatch.Text
					);
					//*/
					// 前後からダブルクォートを取り除く
					//string matchText = e.TokenMatch.Text;
					//matchText = matchText.Trim('"');
					//e.TokenMatch.Text = matchText;
				}
			};

			// リストにトークンを追加した直後に発生するイベント
			tokenizer.TokenAdded += (object sender, TokenAddedEventArgs<TokenType> e) => {
				/*
				// デバッグ用
				Console.WriteLine(
					"token: {0} ({1},{2}): {3}: {4}",
					e.Token.Index,
					e.Token.LineNumber, e.Token.LineIndex,
					e.Token.Type, e.Token.Text
				);
				//*/
			};

			// トークンに分解する
			TokenList<TokenType> tokens = tokenizer.Tokenize(text);

			// 末尾に EOF を追加して, 番兵にする場合
			/*
			Token<TokenType> tokenEOF = new Token<TokenType>();
			tokenEOF.Type = TokenType.EOF;
			tokens.Add(tokenEOF);
			//*/

			/*
			// 分解した内容を表示する (デバッグ用)
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
