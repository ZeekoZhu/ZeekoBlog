const render = require('./asciidoc');

const testContent = `
= Test

== header1

=== level2

==== level3

stem:[sqrt(4) = 2]

[stem] 
++++ 
sqrt(4) = 2
++++

== 附言

上面提到的 F# 版本的实现实际上是要比 F# 程序员的标准做法要更加冗长的！下面就是一个更加简洁的常见做法：

[source,fsharp]
----
// 一句注释
let rec quicksort2 = function // <1>
    | [] -> []                         
    | first::rest -> 
        let smaller,larger = List.partition ((>=) first) rest 
        List.concat [quicksort2 smaller; [first]; quicksort2 larger]

// test code
printfn "%A" (quicksort2 [1;5;23;18;9;1;3])
----

只需要 4 行代码就可以实现相同的功能，而且当你习惯了 F# 的语法之后，它的可读性仍然很高。

=== Lorem

==== lorem2333

=== SameLevel
`;
render(console.log, testContent)
