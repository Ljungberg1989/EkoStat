﻿using EkoStatLibrary.Dtos;

namespace EkoStatLibrary.DtoContainers;

public class EntryGroupByArticle
{
    public int TotalCount => Entries.Sum(e => e.Count);
    public decimal TotalCost => Entries.Sum(e => e.CostPerArticle * e.Count);

    public ArticleResponseDto Article { get; private set; } = null!;
    public List<EntryResponseDto> Entries { get; set; } = new();

    public EntryGroupByArticle(List<EntryResponseDto> entries)
    {
        if (entries.Count > 0)
        {
            EnsureSameArticle(entries);
            Article = entries.First().Article;
            Entries = entries;
        }
    }

    private void EnsureSameArticle(List<EntryResponseDto> entries)
    {
        var firstArticleId = entries.First().Article.Id;
        if (!entries.All(e => e.Article.Id == firstArticleId))
            throw new ArgumentException("Not all entries have the same article.");
    }
}
