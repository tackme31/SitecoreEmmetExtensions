[English](./README.md) | [日本語](./README.ja.md)

# Sitecore Emmet Extensions
*Sitecore Emmet Extensions* is a Sitecore rendering to generate a placeholder container with Emmet abbreviation.

![](./img/demo.gif)

**This software is in early stage of development.**

## Installation
Download the package from [here](https://github.com/xirtardauq/flexible-container/releases) and install it to Sitecore.

## Usage
1. Add `Emmet Abbreviation` rendering (located in `Renderings/Feature/Sitecore Emmet Extensions`) to the page layout.
1. Set an abbreviation to the `Abbreviation` rendering parameter.

## Special Syntax
In *Sitecore Emmet Extensions*, the following syntax can be used in addition to [Emmet syntax](https://github.com/xirtardauq/EmmetSharp).

- [Static Placeholder](#user-content-static-placeholder)
- [Dynamic Placeholder](#user-content-dynamic-placeholder)
- [Field](#user-content-field)
- [Link](#user-content-link)
- [Translation](#user-content-translation)

### Static Placeholder
The static placeholder is rendered with `[placeholder-key]` syntax in the text position.

**Abbreviation:**
```
div{[placeholder-key]}
```

**Result:**
```html
<div>
    @Html.Sitecore().Placeholder("placeholder-key")
</div>
```

**NOTE:**
Using this syntax within the text is not allowed (e.g. `{foo[ph-within-text]bar}`). Split before and after the placeholder like `{foo}+{[ph-within-text]}+{bar}`.

### Dynamic Placeholder
The dynamic placeholder is similar to the static one: `@[placeholder-key]`.

**Abbreviation:**
```
div{@[placeholder-key]}
```

**Result:**
```html
<div>
    @Html.Sitecore().DynamicPlaceholer("placeholder-key")
</div>
```

You can use this syntax with the `count`, `maxCount`, `seed` parameters like `{@[key|count:3|maxCount:5|seed:10]}`

### Field
To display a field value, use the `$(field-name)` syntax in the text part.

**Abbreviation:**
```
p{Value is: {Title}}
```

**Result:**
```html
<p>Value is @Html.Sitecore().Field("Title")</p>
```

You can use the `editable` parameter for specifying enable/disable editing (e.g. `{Title|editable:false}`).

Additionaly, a field that is in a context page can be used ignoring its datasouce by using the `fromPage` parameter like `{{Title|fromPage:true}}`.

If you want to use a link field, write a period after a link field, and write a field name to follow the period (e.g. `{{Category.CategoryName}}`).


### Translation
The `@(dictionary-key)` syntax allows you to translate a text with the `Translate.Text`.

**Abbreviation:**
```
h1{@(Title)}
```

**Result:**
```html
<h1>@Translate.Text("Title")</h1>
```

## See also
- [Emmet &#8212; the essential toolkit for web-developers](https://emmet.io/)
- [xirtardauq/EmmetSharp: An Emmet abbreviation parser written in C#](https://github.com/xirtardauq/EmmetSharp)

## License
*Sitecore Emmet Extensions* is licensed unther the MIT license. See LICENSE.txt.

## Author
- Takumi Yamada (xirtardauq@gmail.com)
