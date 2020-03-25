# Flexible Container
A Sitecore rendering to generate a placeholder with emmet-like syntax.

## Sitecore コンポーネント
- [ ] class & id (`div#id`, `a.class1.class2`)
- [ ] sibling (`p+p`)
- [ ] content (`a{Content}`)
- [ ] placeholder
	- [ ] static (`div{{place-holder-key}}`)
	- [ ] dynamic (`div{{}}`)
- [ ] iterate (`a*5`)
	- [ ] `$`
- [ ] grouping (`(div+p)*5`)
- [ ] translate: (`h1{@(dictionary-key)}`)

---

- input
```
div.row>h1{Hello, test}+div.col{{content}}+div{{}}
```

- output
```html
<div class="row">
    <h1>Hello, test</h1>
    <div class="col">
        @Html.Sitecore().Placeholder("content")
    </div>
    <div>
        @Html.Sitecore().DynamicPlaceholder()
    </div>
</div>
```

---

```html
aaa+bbb>(ccc>ddd+eee>fff)+ggg+hhh>iii
<aaa></aaa>
<bbb>
    <ccc>
        <ddd></ddd>
        <eee>
            <fff></fff>
        </eee>
    </ccc>
    <ggg></ggg>
    <hhh>
        <iii></iii>
    </hhh>
</bbb>
```