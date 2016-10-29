using Hikipuro.Text.Tokenizer;
using TokenType = Tokenizer.Sample.JsonTokenizer.TokenType;

namespace Tokenizer.Sample {
	class JsonTokenizer2 {
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

			// ステップ実行
			TokenList<TokenType> tokens = new TokenList<TokenType>();
			SteppingTokenizer<TokenType> steppingTokenizer
				= tokenizer.CreateSteppingTokenizer(text);

			while (steppingTokenizer.HasNext) {
				Token<TokenType> token = steppingTokenizer.Next();
				switch (token.Type) {
				case TokenType.NewLine:
				case TokenType.Space:
					break;
				default:
					tokens.Add(token);
					break;
				}
				/*
				System.Console.WriteLine(
					"Line: {0,3}, Index: {1,3}, Text: {2}",
					token.LineNumber,
					token.LineIndex,
					token.Text
				);
				//*/
			}

			return tokens;
		}
	}
}
