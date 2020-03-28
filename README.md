# Flexible Container
*Flexible Container* is a Sitecore rendering to generate a placeholder container with Emmet abbreviation.

**This software is in early stage of development.**

## Usage
1. Download the package from [here](https://github.com/xirtardauq/flexible-container/releases) and install it to Sitecore.
1. Add the `Flexible Container` rendering (located in `Renderings/Feature/Flexible Container`) to a page layout.
1. Set an expression to the `Expression` rendering parameter.

![](./img/control-properties.png)

The example expression above is rendered like the following.

```html
<div class="row">
    <a href="/search">Search</a>
    <div class="col">
        @Html.Sitecore().Placeholder("container")
    </div>
</div>
```

## Special Syntax
*Flexible Container* supports [a part of emmet syntax](https://github.com/xirtardauq/EmmetSharp), and some special syntax is added.


### Static Placeholder
A static placeholder is rendered with `{[placeholder-key]}` syntax at the text position.

**Expression:**
```
div{[placeholder-key]}
```

**Rendered:**
```html
<div>
    @Html.Sitecore().Placeholder("placeholder-key")
</div>
```

### Dynamic Placeholder
A dynamic placeholder is rendered with `{@[placeholder-key]}` syntax.

**Expression:**
```
div{@[placeholder-key]}
```

**Rendered:**
```html
<div>
    @Html.Sitecore().DynamicPlaceholer("placeholder-key")
</div>
```

You can use this syntax with the `count`, `maxCount`, `seed` parameters like `{@[key|count:3|maxcount:5|seed:10]}`

### Field Interpolation
To display a field value, use `{field name}` syntax in the text part.

**Expression:**
```
p{Value is: {Title}}
```

**Rendered:**
```html
<p>Value is @Html.Sitecore().Field("Title")</p>
```

You can use the `editable` parameter for specifying enable/disable editing (ex: `{Title|editable:false}`).

### Translation
The `@(dictionary key)` syntax allows you to translate a text with the `Translate.Text`.


**Expression:**
```
h1{@(Title)}
```

**Rendered:**
```html
<h1>@Translate.Text("Title")</h1>
```

## See also
- [Emmet &#8212; the essential toolkit for web-developers](https://emmet.io/)
- [xirtardauq/EmmetSharp: An Emmet abbreviation parser written in C#](https://github.com/xirtardauq/EmmetSharp)

## License
*Flexible Container* is licensed unther the MIT license. See LICENSE.txt.

## Author
- Takumi Yamada (xirtardauq@gmail.com)
