# Flexible Container
A Sitecore rendering to generate a placeholder container with emmet-like syntax.

## Sample
- input
```
div.row>a[href="/search"]{Search}+div.col{{content}}
```

- output
```html
<div class="row">
    <a href="/search">Search</a>
    <div class="col">
        @Html.Sitecore().Placeholder("content")
    </div>
</div>
```

## TODO
- [x] class & id (`div#id`, `a.class1.class2`)
- [x] attribute (`input[type="checkbox" checked]`)
- [x] content (`a{Content}`)
- [x] sibling (`p+p`)
- [x] grouping (`p+(div>h1)+p>a`)
- [x] placeholder
	- [x] static (`div{{place-holder-key}}`)
	- [x] dynamic (`div{[place-holder-key|count:3|maxCount:3|seed:5]`)
- [ ] translate: (`h1{@(dictionary-key)}`)
- [ ] iterate (`a*5`)
	- [ ] iterate counter `p*5>a{text $}`

