using System.IO;
using System.Text;

namespace UnitTest {
	class TestUtility {
		/// <summary>
		/// テキストファイルを読み込む.
		/// </summary>
		/// <param name="path">ファイルのパス.</param>
		/// <param name="encoding">文字エンコード.</param>
		/// <returns>テキスト.</returns>
		public static string ReadTextFile(string path, Encoding encoding = null) {
			if (encoding == null) {
				encoding = Encoding.UTF8;
			}
			StreamReader reader = new StreamReader(
				path, encoding
			);
			string text = reader.ReadToEnd();
			reader.Close();
			return text;
		}
	}
}
