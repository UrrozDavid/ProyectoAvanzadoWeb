using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;
using TBA.Models.TBAModels;

namespace TBA.Models.Entities;

public partial class User : Entity
{
    [JsonPropertyName ("userId")]
    public int UserId { get; set; }
    
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("passwordHash")]
    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<BoardMember> BoardMembers { get; set; } = new List<BoardMember>();

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public override bool Equals(object? obj)
    {
        if (obj is not User other) return false;
        return this.UserId == other.UserId;
    }

    public override int GetHashCode()
    {
        return UserId.GetHashCode();
    }
}
