﻿namespace CardboardBox.Http;

/// <summary>
/// Represents an item that has been cache to the file system
/// </summary>
public class FileCacheItem
{
    /// <summary>
    /// The name of the file
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The mime type of the file
    /// </summary>
    public string MimeType { get; set; } = string.Empty;
    /// <summary>
    /// When the file was created
    /// </summary>
    public DateTime Created { get; set; }


    /// <summary></summary>
    public FileCacheItem() { }

    /// <summary></summary>
    /// <param name="name"></param>
    /// <param name="mimeType"></param>
    /// <param name="created"></param>
    public FileCacheItem(string name, string mimeType, DateTime created)
    {
        Name = name;
        MimeType = mimeType;
        Created = created;
    }

    /// <summary></summary>
    /// <param name="name"></param>
    /// <param name="mimeType"></param>
    /// <param name="created"></param>
    public void Deconstruct(out string name, out string mimeType, out DateTime created)
    {
        name = Name;
        mimeType = MimeType;
        created = Created;
    }
}
