using System;
using System.Linq;
using FSharpx;
using Microsoft.FSharp.Collections;
using ZeekoBlog.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace ZeekoBlog.Application.Services
{
    public class ArticleRenderer : IArticleRenderer
    {
        private readonly TextMaid.TextMaidClient _textMaid;

        public ArticleRenderer(TextMaid.TextMaidClient textMaid)
        {
            _textMaid = textMaid;
        }

        private async Task RenderWithTextMaidAsync(Article article)
        {
            var textType = article.DocType switch
            {
                ArticleDocType.AsciiDoc => TextMaid.TextType.AsciiDoc,
                ArticleDocType.Markdown => TextMaid.TextType.Markdown,
                _ => throw new ArgumentOutOfRangeException(nameof(article.DocType))
            };
            var contentInput = new TextMaid.RenderTextInput(
                article.Content,
                textType,
                true,
                new TextMaid.RenderOption(true, true));
            var summaryInput = new TextMaid.RenderTextInput(
                article.Summary,
                textType,
                false,
                new TextMaid.RenderOption(true, true));
            var result = await _textMaid.RenderText(contentInput);
            if (result.HasValue())
            {
                article.RenderedContent = result.Value.Doc.Html;
                article.TOCList = result.Value.Toc.Select(
                        (x, i) => new TOCItem
                            { AnchorName = x.Id.TrimStart('#'), Level = x.Level, Order = i, Name = x.Name })
                    .ToList();
                article.Languages = string.Join(',', result.Value.Language ?? FSharpList<string>.Empty);
            }

            result = await _textMaid.RenderText(summaryInput);
            if (result.HasValue())
            {
                article.RenderedSummary = result.Value.Doc.Html;
            }
        }

        public async Task RenderAsync(Article article)
        {
            switch (article.DocType)
            {
                case ArticleDocType.Markdown:
                case ArticleDocType.AsciiDoc:
                    await RenderWithTextMaidAsync(article);
                    break;
                case ArticleDocType.Raw:
                    article.RenderedSummary = article.Summary;
                    article.RenderedContent = article.Content;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
