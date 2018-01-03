using Hikipuro.Text.Tokenizer;
using System;
using System.Diagnostics;
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
		/// when clicked "Load CSV" button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonLoadCsv_Click(object sender, EventArgs e) {
			// 
			// read Sample/CSV/13TOKYO.CSV file
			// 
			// - this file borrowed from next url
			//   http://www.post.japanpost.jp/zipcode/dl/oogaki-zip.html
			// 
			StreamReader reader = new StreamReader(
				"Sample/CSV/13TOKYO.CSV",
				Encoding.GetEncoding("Shift_JIS")
			);
			string text = reader.ReadToEnd();
			reader.Close();

			// tokenize CSV
			TokenList<CSVTokenType> tokens = null;
			long time = Benchmark((i) => {
				tokens = CSVTokenizer.Tokenize(text);
			}, 1);
			textBoxTime.Text = time + " ms";

			// traverse all tokenized tokens
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Token<CSVTokenType> token in tokens) {
				// new line
				if (token.Type == CSVTokenType.NewLine) {
					stringBuilder.AppendLine();
					continue;
				}
				// comma
				if (token.Type == CSVTokenType.Comma) {
					stringBuilder.Append("\t");
					continue;
				}
				// numbers or strings
				stringBuilder.Append(token.Text);
			}

			// show results in text box
			textBox.Text = stringBuilder.ToString();
		}

		/// <summary>
		/// when clicked "Load JSON" button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonLoadJson_Click(object sender, EventArgs e) {
			// 
			// read Sample/JSON/Test1.json file
			// 
			// - this file borrowed from next url
			//   http://json.org/example.html
			// 
			StreamReader reader = new StreamReader(
				"Sample/JSON/Test1.json",
				Encoding.UTF8
			);
			string text = reader.ReadToEnd();
			reader.Close();

			// token grouping
			TokenTypeGroup<JsonTokenType> openBlockGroup
				= new TokenTypeGroup<JsonTokenizer.TokenType>();
			openBlockGroup.Add(JsonTokenType.OpenBrace);
			openBlockGroup.Add(JsonTokenType.OpenBracket);

			TokenTypeGroup<JsonTokenType> closeBlockGroup
				= new TokenTypeGroup<JsonTokenizer.TokenType>();
			closeBlockGroup.Add(JsonTokenType.CloseBrace);
			closeBlockGroup.Add(JsonTokenType.CloseBracket);

			// tokenize JSON text
			TokenList<JsonTokenType> tokens = null;
			long time = Benchmark((i) => {
				tokens = JsonTokenizer.Tokenize(text);
			}, 1);
			textBoxTime.Text = time + " ms";

			// traverse all tokenized tokens
			// (format JSON text)
			int indentSize = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Token<JsonTokenType> token in tokens) {
				// colon
				if (token.IsTypeOf(JsonTokenType.Colon)) {
					stringBuilder.Append(token.Text);
					stringBuilder.Append(" ");
					continue;
				}
				// comma
				if (token.IsTypeOf(JsonTokenType.Comma)) {
					stringBuilder.Append(token.Text);
					JsonNewLine(stringBuilder, indentSize);
					continue;
				}
				// open brace { or open bracket [
				if (token.IsMemberOf(openBlockGroup)) {
					indentSize++;
					stringBuilder.Append(token.Text);
					JsonNewLine(stringBuilder, indentSize);
					continue;
				}
				// close brace } or close bracket ]
				if (token.IsMemberOf(closeBlockGroup)) {
					indentSize--;
					if (token.Next != null && token.Next.IsTypeOf(JsonTokenType.Comma)) {
						stringBuilder.Append(token.Text);
						continue;
					}
					stringBuilder.Append(token.Text);
					JsonNewLine(stringBuilder, indentSize - 1);
					continue;
				}
				// strings, numbers, null, true and false
				if (token.Next != null && token.Next.IsMemberOf(closeBlockGroup)) {
					stringBuilder.Append(token.Text);
					JsonNewLine(stringBuilder, indentSize - 1);
					continue;
				}
				stringBuilder.Append(token.Text);
			}

			// show results in text box
			textBox.Text = stringBuilder.ToString();
		}

		/// <summary>
		/// Add to new line and indent to StringBuilder object.
		/// This method used in JSON text processing.
		/// </summary>
		/// <param name="stringBuilder">target StringBuilder.</param>
		/// <param name="indentSize">indent width.</param>
		private void JsonNewLine(StringBuilder stringBuilder, int indentSize) {
			if (indentSize < 0) {
				stringBuilder.AppendLine();
				return;
			}
			stringBuilder.AppendLine();
			stringBuilder.Insert(stringBuilder.Length, "  ", indentSize);
		}

		/// <summary>
		/// Measure method processing time (ms).
		/// </summary>
		/// <param name="act">measure method</param>
		/// <param name="iterations">iterations count</param>
		/// <returns>time in milli seconds.</returns>
		private static long Benchmark(System.Action<int> act, int iterations) {
			if (iterations <= 0) {
				return 0;
			}
			GC.Collect();
			//act.Invoke(1); // run once outside of loop to avoid initialization costs
			Stopwatch sw = Stopwatch.StartNew();
			for (int i = 0; i < iterations; i++) {
				act.Invoke(1);
			}
			sw.Stop();
			return sw.ElapsedMilliseconds / iterations;
		}
	}
}
