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

---

root>
   aaa
   +
   bbb>
       (ppp
        +
        ccc>
           ddd
           +
           eee>
               fff)
       +
       ggg
       +
       hhh>
           iii

ppp+ccc>
ddd+eee>
fff

>で区切る
+で区切り、それぞれに対して再帰的にパース
+で区切った最後の要素の子供に、残りの要素をパースして追加する

```html
root>aaa+bbb>(ppp+ccc>ddd+eee>fff)+ggg+hhh>iii
<root>
    <aaa></aaa>
    <bbb>
        <ppp></ppp>
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
</root>
```