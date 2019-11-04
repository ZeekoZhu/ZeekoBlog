module HttpRequestMessage

open System
open System.Net.Http

module Builder =
    type Operator = HttpRequestMessage -> HttpRequestMessage
    let zero() = new HttpRequestMessage()

    /// Set request url
    let url uri: Operator =
        fun req ->
            req.RequestUri <- Uri uri
            req

    /// Set request url
    let uri uri: Operator =
        fun req ->
            req.RequestUri <- uri
            req

    let method m: Operator =
        fun req ->
            req.Method <- m
            req

    let get u: Operator =
        method HttpMethod.Get >> url u

    let post u: Operator =
        method HttpMethod.Get >> url u

    let body content: Operator =
        fun req ->
            req.Content <- content
            req

    let json jsonStr: Operator =
        fun req ->
            req.Content <-
                new StringContent(jsonStr, Text.Encoding.UTF8, "application/json")
            req

    let header key (value: string): Operator =
        fun req ->
            req.Headers.Add(key, value)
            req

type HttpRequestMessageBuilder() =
    member __.Yield (()) = Builder.zero()
    [<CustomOperation("url")>]
    member __.Url(req, url) = Builder.url url req
    [<CustomOperation("uri")>]
    member __.Uri(req, uri) = Builder.uri uri req
    [<CustomOperation("get")>]
    member __.Get(req, url) = Builder.get url req
    [<CustomOperation("post")>]
    member __.Post(req, url) = Builder.post url req
    [<CustomOperation("json")>]
    member __.Json(req, jsonStr) = Builder.json jsonStr req
    [<CustomOperation("body")>]
    member __.Body(req, content) = Builder.body content req
    [<CustomOperation("method")>]
    member __.Method(req, m) = Builder.method m req
    [<CustomOperation("header")>]
    member __.Header(req, key, value) = Builder.header key value req



let requestBuilder = HttpRequestMessageBuilder()
