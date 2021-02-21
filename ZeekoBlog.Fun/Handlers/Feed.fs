module ZeekoBlog.Fun.Handlers.Feed

open System
open System.IO
open System.Xml
open FSharp.Control.Tasks
open FSharpx
open FsToolkit.ErrorHandling
open Giraffe
open System.ServiceModel.Syndication
open ZeekoBlog.Application.Services
open ZeekoBlog.Core.Models
open ZeekoBlog.Fun

let generateFeedEntry (article: Article) =
    let url =
        Constants.blogPath "/a/" + article.Id.ToString()
    let entry = SyndicationItem()
    entry.Title <- TextSyndicationContent(article.Title)
    entry.Content <- TextSyndicationContent.CreateHtmlContent(article.RenderedContent)
    entry.Links.Add(SyndicationLink(Uri(url)))
    entry.LastUpdatedTime <- DateTimeOffset(article.LastEdited.ToLocalTime())
    entry.Summary <- TextSyndicationContent(article.RenderedSummary, TextSyndicationContentKind.Html)
    entry.PublishDate <- DateTimeOffset(article.Created.ToLocalTime())
    entry

let generateFeed (articles: Article seq) =
    let feedUri = Uri(Constants.blogPath "/feed")

    let feed =
        SyndicationFeed("网上冲浪指南", "Zeeko's Blog", feedUri)

    feed.BaseUri <- Uri(Constants.address)
    feed.Id <- Constants.address
    feed.LastUpdatedTime <- DateTimeOffset((articles |> Seq.head).LastEdited.ToLocalTime())
    feed.Authors.Add(SyndicationPerson("vaezt@outlook.com", "Zeeko Zhu", Constants.address))
    feed.ElementExtensions.Add("icon", "", Constants.blogPath "/favicon-192x192.png")
    feed.Items <- articles |> Seq.map generateFeedEntry
    feed


let handler (user: string): HttpHandler =
    fun next ctx ->
        let articleSvc = ctx.GetService<ArticleService>()
        let accountSvc = ctx.GetService<AccountService>()

        task {
            let! blogUser = accountSvc.GetUserByNameAsync(user)

            let blogUserId =
                Option.ofObj blogUser
                |> Option.map (fun x -> x.Id)
                |> Option.defaultValue 1

            let! articles = articleSvc.GetRecentAsync(10, blogUserId)
            let feed = generateFeed articles
            use ms = new MemoryStream()
            use feedWriter = XmlWriter.Create(ms)
            feed.SaveAsAtom10(feedWriter)
            feedWriter.Flush()
            ctx.SetContentType("application/atom+xml")
            ms.Seek(0L, SeekOrigin.Begin) |> ignore
            return! ctx.WriteStreamAsync false ms None None
        }
