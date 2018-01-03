# Tokenizer
.NET Simple Tokenizer (2.0 or later)

This library can use with .NET Framework 2.0 or later.

## License
- [MIT](LICENSE)

## How to use

- download or clone entire this repository.
- open "Tokeniser.sln" in your Visual Studio.
- run this project.
- CSV tokenize sample will be running on click "Load CSV" button.
- JSON tokenize sample will be running on click "Load JSON" button.

please use previous version Visual Studio, if you get some errors.
- open "Tokenizer(vs2010).sln" in your Visual Studio.
- this solution file can use with Visual Studio 2010 or later.

## Note

- CSV file tokenize sample: [Sample/CSVTokenizer.cs](Sample/CSVTokenizer.cs)
- JSON file tokenize sample: [Sample/JsonTokenizer.cs](Sample/JsonTokenizer.cs)
- This project does not correspond to build for class library for now, copy files in [Hikipuro/Text](Hikipuro/Text) on your project folder, if you want to use in your project.
- This project can't easy to use with NuGet for now.

## Development environment

- Visual Studio 2015

## Sample files

- Sample/CSV/13TOKYO.CSV: borrowed from [Japanese post office](http://www.post.japanpost.jp/zipcode/dl/oogaki-zip.html).
- Sample/JSON/*.json: borrowed from [JSON Example](http://json.org/example.html).



# Tokenizer
(in Japanese)
シンプルなC#の字句解析ツール

.NET Framework 2.0 以降の環境で実行できると思います。

## ライセンス
- [MIT](LICENSE)

## 開発環境

- Visual Studio 2015

## 使い方

- Visual Studio で Tokenizer.sln を開いてください
- "Load CSV" ボタンを押すと、CSV ファイルの分解サンプルが実行されます
- "Load JSON" ボタンを押すと、JSON ファイルの分解サンプルが実行されます

古いバージョンの Visual Studio で開く場合
- Tokenizer(vs2010).sln を開いてください
- Visual Studio 2010 以降に対応しています

より詳しい使い方
- [(C#) 字句解析ツールを作ってみた - ひきぷろのプログラミング日記](http://hikipuro.hatenadiary.jp/entry/2016/10/21/130835)

## 備考

- CSV ファイルのサンプルコード: [Sample/CSVTokenizer.cs](Sample/CSVTokenizer.cs)
- JSON ファイルのサンプルコード: [Sample/JsonTokenizer.cs](Sample/JsonTokenizer.cs)
- 今のところクラスライブラリにはしていないので、ほかのプロジェクトから使用する場合は [Hikipuro/Text](Hikipuro/Text) 内のファイルをコピーしてください

## サンプルファイル提供

- Sample/CSV/13TOKYO.CSV: [郵便局のサイト](http://www.post.japanpost.jp/zipcode/dl/oogaki-zip.html) からお借りしました
- Sample/JSON/*.json: [JSON Example](http://json.org/example.html) からお借りしました
