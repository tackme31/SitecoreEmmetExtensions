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
- placeholder
- field
- translation

## Todo List
- [x] Nest (`div>p`)
- [x] Class & ID (`div#id`, `a.class1.class2`)
- [x] Attributes (`input[type="checkbox" checked]`)
- [x] Content (`a{Content}`)
- [x] Sibling (`p+p`)
- [x] Grouping (`p+(div>h1)+p>a`)
- [x] Placeholder
	- [x] Static (`div{[place-holder-key]}`)
	- [x] Dynamic (`div{@[place-holder-key]}`)
        - [x] With args (`div{@[key|count:3|maxCount:3|seed:5]}`)
- [ ] Field interpolation (`h1{Title: {Title}}`)
- [ ] Translation: (`h1{@(dictionary-key)}`)
- [ ] Iteration (`a*5`)
	- [ ] Iterate counter `p*5>a{text $}`

## License    

## Author
- Takumi Yamada (xirtardauq@gmail.com)