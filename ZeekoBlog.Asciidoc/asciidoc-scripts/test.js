const render = require('./asciidoc');

const testContent = `
= Asciidoc Cheatsheet

<<<

Asciidoc is a rich text based markup language. A document written with Asciidoc can easily be converted to *HTML*, *_PDF_*, *Docbook*, *_Mobi_*, *Epub*, and *Odt* formats. This cheatsheet shows you the common features of Asciidoc Markup language.

'''
 
== [underline]#Basic formats#

*Bold* , _İtalic_ , [underline]#Underscore# , To^p^ , Dow~n~

'''

== [underline]#Title levels#

There are 6 Level titles as HTML. First one is for document title.

[source,adoc]
----
= Level 1

Content ...

== Level 2

Content ...

=== Level 3

Content ...

==== Level 4

Content ...

===== Level 5

Content ...
----

'''

== [underline]#Lists#

There are ordered, unordered, checked and labeled list elements.

.Unordered list
* deep 1
** deep 2
*** deep 3
**** deep 4
***** deep 5
* deep 1

.Ordered list
. Order 1
. Order 2
.. Order 2a
.. Order 2b
. Order 3

.Checked list
- [*] checked
- [x] checked
- [ ] unchecked
-     normal

.Labeled list
Elma:: Eski Türkçe'de "alma" diye bilinen adının, meyvenin rengi olan "al" (kırmızı)'dan geldiği bilinmektedir

Armut:: Gülgiller (Rosaceae) familyasının Maloideae alt familyasında sınıflanan Pyrus cinsine ait ağaç nitelikli bitki türleriyle, bu türlerden bazılarının yenilebilir meyvelerinin ortak adı.

'''

== [underline]#Links#

You can use links:

http://asciidocfx.org - AsciidocFX

http://asciidocfx.org[AsciidocFX]

'''

== [underline]#Images#

You can declare images with \`image::\` prefix and \`[]\` suffix. Path may be _relative_ or _absolute_ .

=== Basit

image::https://kodedu.com/wp-content/uploads/2017/02/kodedu-logo-e1487527571657.png[]

=== Detaylı

image::https://kodedu.com/wp-content/uploads/2017/02/kodedu-logo-e1487527571657.png[caption="Şekil 1. ",title="kodedu.com",alt="kodedu.com"]

'''

== [underline]#Code higlighting#

You can declare inline or block based codes with Asciidoc syntax.

.Inline
Reference code like \`types\` or \`methods\` inline.

.Code block
[source,java]
----
public interface Hello {

    void hello();

}
----

You can use numbered elements named \`callouts\` in Asciidoc.

.Numbered code block
[source,ruby]
----
require 'sinatra' // <1>

get '/hi' do // <2>
  "Hello World!" // <3>
end
----
<1> Library import
<2> URL mapping
<3> Content for response

'''

== [underline]#Blocks#

Sınırlandırılmış bloklar 4'er özel karakter ile sınırlandırılmış alanlardır.

=== _Sidebar_ block

.Başlık (opsiyonel)
****
Bu blok türünün adı *Sidebar* bloktur.
****

=== Example block

.Başlık (opsiyonel)
====
Bu blok türünün adı *Example* bloktur.
====

ifdef::backend-html5[]
=== Passthrough block

++++
Bu blok türünün adı <b>Passthrough</b> bloktur. Bu blok içerinde HTML elemanları kullanabilirsiniz.
<br/>
<br/>
<u>Örneğin;</u>
<br/><br/>
<ul>
    <li>Ali</li>
    <li>Veli</li>
    <li>Selami</li>
</ul>
++++
endif::[]

=== BlockquoteS block

.Başlık (opsiyonel)
[quote, Hakan Özler, AspectJ Ebook]
____
AspectJ dilinin kullandığı yapılar 3 kısımda toplanmıştır.

Bunlar:: Ortak, Dinamik ve Statik crosscutting (enine kesen) bölümlerdir.

Bu bölümler, içlerinde farklı bileşenleri toplayarak bizim mevcut *OOP* sistemimizi *AOP* mantığı ile harmanlamamıza imkan veriyorlar.
____

'''

== [underline]#Uyarı blokları#

Asciidoc işaretleme dilinde 5 tip uyarı (admonition) bloğu bulunmaktadır. Bu blokların kendine has ikonları bulunmaktadır.

.Not bloğu
[NOTE]
====
Burası bir not bloğu
====

.Önemli bloğu
[IMPORTANT]
====
Burası bir önemli bloğu
====

.İpucu bloğu
[TIP]
====
Burası bir ipucu bloğu
====

.Dikkat bloğu
[CAUTION]
====
Burası bir dikkat bloğu
====

.Uyarı bloğu
[WARNING]
====
Burası bir uyarı bloğu
====

////
.Icon bloğu

AsciidocFX ile http://fortawesome.github.io/Font-Awesome/icons/[FontAwesome] ikonlarını kullanabilirsiniz.

icon:tags[] ruby, asciidoctor +
icon:folder[] ruby, asciidoctor +
icon:file[] ruby, asciidoctor +
icon:facebook[] ruby, asciidoctor +
icon:github[] ruby, asciidoctor +
icon:twitter[] ruby, asciidoctor
////

'''
== [underline]#Tablolar#

Asciidoc ile hemen hemen tüm kompleks tablo yapılarını kurabilirsiniz.

=== Basit bir tablo

.Başlık (opsiyonel)
[options="header,footer"]
|=======================
|Col 1|Col 2      |Col 3
|1    |Item 1     |a
|2    |Item 2     |b
|3    |Item 3     |c
|6    |Three items|d
|=======================

=== Kompleks bir tablo

.Başlık (opsiyonel)
|====
|Date |Duration |Avg HR |Notes
|22-Aug-08 .2+^.^|10:24 | 157 |
Worked out MSHR (max sustainable
heart rate) by going hard
for this interval.
|22-Aug-08 | 152 |
Back-to-back with previous interval.
|24-Aug-08 3+^|none
|====

`;
render(console.log, testContent)
