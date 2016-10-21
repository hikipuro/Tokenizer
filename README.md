# Tokenizer
シンプルなC#の字句解析ツール

.NET Framework 2.0 以降の環境で実行できると思います。

## ライセンス
- [MIT](LICENSE)

## 開発環境

- Visual Studio 2015

## 使い方

- Visual Studio で Tokenizer.sln を開いてください
- "CSVファイルのロード" ボタンを押すと、CSV ファイルの分解サンプルが実行されます
- "JSONファイルのロード" ボタンを押すと、JSON ファイルの分解サンプルが実行されます

より詳しい使い方
- [(C#) 字句解析ツールを作ってみた - ひきぷろのプログラミング日記](http://hikipuro.hatenadiary.jp/entry/2016/10/21/130835)

## 備考

- [Sample/CSVTokenizer.cs](Sample/CSVTokenizer.cs) に、 CSV ファイルの分解サンプルがあります
- 今のところクラスライブラリにはしていないので、ほかのところで使う場合は [Hikipuro/Text](Hikipuro/Text) 内のファイルをコピーしてください

## サンプルファイル提供

- Sample/CSV/13TOKYO.CSV: [郵便局のサイト](http://www.post.japanpost.jp/zipcode/dl/oogaki-zip.html) からお借りしました
- Sample/JSON/*.json: [JSON Example](http://json.org/example.html) からお借りしました
