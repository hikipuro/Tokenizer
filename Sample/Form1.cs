using Hikipuro.Text;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CSVTokenType = Tokenizer.Sample.CSVTokenizer.TokenType;
using JsonTokenType = Tokenizer.Sample.JsonTokenizer.TokenType;

namespace Tokenizer.Sample {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}

		/// <summary>
		/// CSV ファイルのロードボタンが押された時.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonLoadCsv_Click(object sender, EventArgs e) {
			// 
			// Sample/CSV/13TOKYO.CSV を読み込む
			// 
			// - このファイルは郵便局のサイトからお借りしました
			//   http://www.post.japanpost.jp/zipcode/dl/oogaki-zip.html
			// 
			StreamReader reader = new StreamReader(
				"Sample/CSV/13TOKYO.CSV",
				Encoding.GetEncoding("Shift_JIS")
			);
			string text = reader.ReadToEnd();
			reader.Close();
			
			// CSV ファイルを分解する
			TokenList<CSVTokenType> tokens = CSVTokenizer.Tokenize(text);

			// 分解されたトークンを巡回する
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Token<CSVTokenType> token in tokens) {
				// 改行文字
				if (token.Type == CSVTokenType.NewLine) {
					stringBuilder.AppendLine();
					continue;
				}
				// カンマ
				if (token.Type == CSVTokenType.Comma) {
					stringBuilder.Append("\t");
					continue;
				}
				// 数値と文字列
				stringBuilder.Append(token.Text);
			}

			// 画面に表示する
			textBox.Text = stringBuilder.ToString();
		}

		/// <summary>
		/// JSON ファイルのロードボタンが押された時.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonLoadJson_Click(object sender, EventArgs e) {
			// 
			// Sample/JSON/Test1.json を読み込む
			// 
			// - このファイルは次のサイトからお借りしました
			//   http://json.org/example.html
			// 
			StreamReader reader = new StreamReader(
				"Sample/JSON/Test1.json",
				Encoding.UTF8
			);
			string text = reader.ReadToEnd();
			reader.Close();

			// CSV ファイルを分解する
			TokenList<JsonTokenType> tokens = JsonTokenizer.Tokenize(text);

			// 分解されたトークンを巡回する
			// (JSON ファイルをフォーマットする)
			int indentSize = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Token<JsonTokenType> token in tokens) {
				// コロン
				if (token.Type == JsonTokenType.Colon) {
					stringBuilder.Append(token.Text);
					stringBuilder.Append(" ");
					continue;
				}
				// カンマ
				if (token.Type == JsonTokenType.Comma) {
					stringBuilder.Append(token.Text);
					JsonNewLine(stringBuilder, indentSize);
					continue;
				}
				// 波括弧 {
				if (token.Type == JsonTokenType.OpenBrace) {
					indentSize++;
					stringBuilder.Append(token.Text);
					JsonNewLine(stringBuilder, indentSize);
					continue;
				}
				// 波括弧 }
				if (token.Type == JsonTokenType.CloseBrace) {
					indentSize--;
					Token<JsonTokenType> next = tokens.Next(token);
					if (next.Type == JsonTokenType.Comma) {
						stringBuilder.Append(token.Text);
						continue;
					}
					stringBuilder.Append(token.Text);
					JsonNewLine(stringBuilder, indentSize - 1);
					continue;
				}
				// 角括弧 [
				if (token.Type == JsonTokenType.OpenBracket) {
					indentSize++;
					stringBuilder.Append(token.Text);
					JsonNewLine(stringBuilder, indentSize);
					continue;
				}
				// 角括弧 ]
				if (token.Type == JsonTokenType.CloseBracket) {
					indentSize--;
					stringBuilder.Append(token.Text);
					JsonNewLine(stringBuilder, indentSize - 1);
					continue;
				}
				// 文字列, 数値, null, true, false
				if (token.Type == JsonTokenType.String
				|| token.Type == JsonTokenType.Number
				|| token.Type == JsonTokenType.Null
				|| token.Type == JsonTokenType.True
				|| token.Type == JsonTokenType.False) {
					Token<JsonTokenType> next = tokens.Next(token);
					if (next.Type == JsonTokenType.CloseBrace
					|| next.Type == JsonTokenType.CloseBracket) {
						stringBuilder.Append(token.Text);
						JsonNewLine(stringBuilder, indentSize - 1);
						continue;
					}
				}
				stringBuilder.Append(token.Text);
			}

			// 画面に表示する
			textBox.Text = stringBuilder.ToString();
		}

		/// <summary>
		/// StringBuilder に改行とインデントを追加する.
		/// JSON の処理中に使用する.
		/// </summary>
		/// <param name="stringBuilder">文字を追加する StringBuilder.</param>
		/// <param name="indentSize">インデント幅.</param>
		private void JsonNewLine(StringBuilder stringBuilder, int indentSize) {
			if (indentSize < 0) {
				stringBuilder.AppendLine();
				return;
			}
			stringBuilder.AppendLine();
			stringBuilder.Insert(stringBuilder.Length, "  ", indentSize);
		}
	}
}
