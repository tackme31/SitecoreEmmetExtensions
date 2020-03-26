# Flexible Container
A Sitecore rendering to generate a placeholder container with emmet-like syntax.

## Sample
- Input
```
div.row>a[href="/search"]{Search}+div.col{[container]}
```

- Output
```html
<div class="row">
    <a href="/search">Search</a>
    <div class="col">
        @Html.Sitecore().Placeholder("container")
    </div>
</div>
```

## Usage

### Special Syntax
Some special syntax is added.

#### Static Placeholder
```
div{[placeholder-key]}
```
```html
<div>
    @Html.Sitecore().Placeholder("placeholder-key")
</div>
```

#### Dynamic Placeholder
```
div{@[placeholder-key]}
```
```html
<div>
    @Html.Sitecore().DynamicPlaceholer("placeholder-key")
</div>
```

with parameters.

```
div{@[placeholder-key|count:3|maxCount:10|seed:5]}
```
```html
<div>
    @Html.Sitecore().DynamicPlaceholer("placeholder-key", count: 3, maxCount: 10, seed: 5)
</div>
```

#### Field interpolation (WIP)
```
div>p{Hello {Name}}
```

```html
<div>
    <p>Hello @Html.Sitecore().Field("Name")</p>
</div>
```

#### Translation (WIP)

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