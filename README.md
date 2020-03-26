# Flexible Container
A Sitecore rendering to generate a placeholder container with emmet-like syntax.

## Sample
**Expression:**
```
div.row>a[href="/search"]{Search}+div.col{[container]}
```

**Rendered:**
```html
<div class="row">
    <a href="/search">Search</a>
    <div class="col">
        @Html.Sitecore().Placeholder("container")
    </div>
</div>
```

## Usage
Add a `Flexible Container` rendering to your page layout and set an expression to the `Expression` rendering parameter.

## Special Syntax
*Flexible Container* supports a part of emmet syntax, and some special syntax is added.

### Static Placeholder
A static placeholder is rendered with `{[placeholder-key]}` syntax at the content position.

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
A dynamic placeholder is rendered with `{@{placeholder-key]}` syntax.

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

\
You can use this syntax with the `count`, `maxCount`, `seed` parameters.

**Expression:**
```
div{@[placeholder-key|count:3|maxCount:10|seed:5]}
```

**Rendered:**
```html
<div>
    @Html.Sitecore().DynamicPlaceholer("placeholder-key", count: 3, maxCount: 10, seed: 5)
</div>
```

### Field interpolation (Not supported yet)
```
div>p{Hello {Name}}
```

```html
<div>
    <p>Hello @Html.Sitecore().Field("Name")</p>
</div>
```

### Translation (Not supported yet)

```
div>h1{@(Filtering by)}
```

```html
<div>
    <h1>@Translate.Text("Filtering by")</h1>
</div>
```

## Todo List
- [x] Nest (`div>p`)
- [x] Class & ID (`div#id`, `a.class1.class2`)
- [x] Attributes (`input[type="checkbox" checked]`)
- [x] Content (`a{Content}`)
- [x] Sibling (`p+p`)
- [x] Grouping (`p+(div>h1)+p>a`)
- [ ] Iteration (`a*5`)
	- [ ] Iterate counter `p*5>a{text $}`
- [x] Placeholder
	- [x] Static (`div{[place-holder-key]}`)
	- [x] Dynamic (`div{@[place-holder-key]}`)
        - [x] With parameters (`div{@[key|count:3|maxCount:3|seed:5]}`)
- [ ] Field interpolation (`h1{Title: {Title}}`)
- [ ] Translation: (`h1{@(dictionary-key)}`)

## License
*Flexible Container* is licensed unther the MIT license. See LICENSE.txt.

## Author
- Takumi Yamada (xirtardauq@gmail.com)