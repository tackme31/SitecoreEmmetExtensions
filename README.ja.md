[English](./README.md) | [日本語](./README.ja.md)

# Flexible Container
*Flexible Container* はEmmet記法でレンダリングを作成することができるSitecoreの拡張モジュールです。

![](./img/demo.gif)

**注意：このソフトウェアは開発初期の段階にあります。**

## インストール方法
[こちら](https://github.com/xirtardauq/flexible-container/releases)からパッケージをダウンロードし、インストールウィザードを使用してSitecoreにインストールしてください。

## 使い方
1. レイアウト詳細に`Flexible Container`レンダリングを追加します。レンダリングは`Renderings/Feature/Flexible Container`にあります。
1. レンダリングの`Abbreviation`パラメータにEmmetの式を入力します。

## 特殊記法
*Flexible Container*には、[通常のEmmet記法](https://github.com/xirtardauq/EmmetSharp)に加えて、以下のような特殊な記法が追加されています。

- [静的プレースホルダ](#user-content-static-placeholder)
- [動的プレースホルダ](#user-content-dynamic-placeholder)
- [フィールド埋め込み](#user-content-field-interpolation)
- [翻訳](#user-content-translation)

### 静的プレースホルダ
HTMLタグのテキスト部分に`[placeholder-key]`と記述することで、静的なプレースホルダを生成することができます。

**式:**
```
div{[placeholder-key]}
```

**結果:**
```html
<div>
    @Html.Sitecore().Placeholder("placeholder-key")
</div>
```

テキストの間にプレースホルダを挿入する場合は、`{text1}+{[ph-within-text]}+{text2}`のようにプレースホルダの前後でテキストを区切ってください。

### 動的プレースホルダ
動的プレースホルダは、`@[placeholder-key]`のように記述することで生成することができます。  

**式:**
```
div{@[placeholder-key]}
```

**結果:**
```html
<div>
    @Html.Sitecore().DynamicPlaceholer("placeholder-key")
</div>
```

`count`, `maxCount`, `seed`パラメータを使用するには`{@[key|count:3|maxcount:5|seed:10]}`のようにパイプ区切りで指定します。

### フィールド埋め込み
属性値やテキストなどにフィールドを埋め込むには`{field-name}`という記法を使用します。

**式:**
```
p{Value is: {Title}}
```

**結果:**
```html
<p>Value is @Html.Sitecore().Field("Title")</p>
```

`editable`パラメータでエクスペリエンスエディタでの編集の有効化/無効化を切り替えることができます。デフォルトでは編集可能なフィールドとして表示されます。(例: `{Title|editalbe:false}`)  

さらに`fromPage`パラメータに`true`を指定することで、データソースではなく現在のページのフィールドを表示することもできます。  

またフィールド名をピリオドで区切ることで、リンクフィールドの参照先アイテムのフィールドを指定することもできます。例えば`{Category.Name}`と入力すると、Categoryフィールドで指定したアイテムのNameフィールドを表示する、という意味になります。

### 翻訳
`@(dictionary-key)`と入力することで、Sitecoreの辞書機能による翻訳を行うことができます。

**式:**
```
h1{@(Title)}
```

**結果:**
```html
<h1>@Translate.Text("Title")</h1>
```

## 参考リンク
- [Emmet &#8212; the essential toolkit for web-developers](https://emmet.io/)
- [xirtardauq/EmmetSharp: An Emmet abbreviation parser written in C#](https://github.com/xirtardauq/EmmetSharp)

## ライセンス
*Flexible Container*はMITライセンスでリリースされています。

## 作者
- 山田拓実 (xirtardauq@gmail.com)
