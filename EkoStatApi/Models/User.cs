﻿namespace EkoStatApi.Models;
#nullable disable

public class User
{
    // Keys
    public int Id { get; set; }

    // Data
    public string Name { get; set; }

    // Navigation
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<Article> Articles { get; set; } = new List<Article>();
    public ICollection<Entry> Entries { get; set; } = new List<Entry>();
}
