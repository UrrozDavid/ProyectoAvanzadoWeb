using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TBA.Models.Entities;

namespace TBA.Data.Models;

public partial class TrelloDbContext : DbContext
{
    public TrelloDbContext()
    {
    }

    public TrelloDbContext(DbContextOptions<TrelloDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<BoardMember> BoardMembers { get; set; }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=TAMARA;Database=TrelloDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PK__Attachme__442C64DEAB9D5C8D");

            entity.Property(e => e.AttachmentId).HasColumnName("AttachmentID");
            entity.Property(e => e.CardId).HasColumnName("CardID");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Card).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.CardId)
                .HasConstraintName("FK__Attachmen__CardI__5DCAEF64");
        });

        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.BoardId).HasName("PK__Boards__F9646BD2DEB8162D");

            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Boards)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Boards__CreatedB__3B75D760");
        });

        modelBuilder.Entity<BoardMember>(entity =>
        {
            entity.HasKey(e => new { e.BoardId, e.UserId }).HasName("PK__BoardMem__281CE718FA57FD0B");

            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("member");

            entity.HasOne(d => d.Board).WithMany(p => p.BoardMembers)
                .HasForeignKey(d => d.BoardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BoardMemb__Board__3F466844");

            entity.HasOne(d => d.User).WithMany(p => p.BoardMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BoardMemb__UserI__403A8C7D");
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.CardId).HasName("PK__Cards__55FECD8E390AA84F");

            entity.Property(e => e.CardId).HasColumnName("CardID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.ListId).HasColumnName("ListID");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.List).WithMany(p => p.Cards)
                .HasForeignKey(d => d.ListId)
                .HasConstraintName("FK__Cards__ListID__46E78A0C");

            entity.HasMany(d => d.Labels).WithMany(p => p.Cards)
                .UsingEntity<Dictionary<string, object>>(
                    "CardLabel",
                    r => r.HasOne<Label>().WithMany()
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CardLabel__Label__5070F446"),
                    l => l.HasOne<Card>().WithMany()
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CardLabel__CardI__4F7CD00D"),
                    j =>
                    {
                        j.HasKey("CardId", "LabelId").HasName("PK__CardLabe__76692F34E070FB07");
                        j.ToTable("CardLabels");
                        j.IndexerProperty<int>("CardId").HasColumnName("CardID");
                        j.IndexerProperty<int>("LabelId").HasColumnName("LabelID");
                    });

            entity.HasMany(d => d.Users).WithMany(p => p.Cards)
                .UsingEntity<Dictionary<string, object>>(
                    "CardAssignment",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CardAssig__UserI__4AB81AF0"),
                    l => l.HasOne<Card>().WithMany()
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CardAssig__CardI__49C3F6B7"),
                    j =>
                    {
                        j.HasKey("CardId", "UserId").HasName("PK__CardAssi__848641445B22991B");
                        j.ToTable("CardAssignments");
                        j.IndexerProperty<int>("CardId").HasColumnName("CardID");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                    });
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAA1B19AFA7");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.CardId).HasColumnName("CardID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Card).WithMany(p => p.Comments)
                .HasForeignKey(d => d.CardId)
                .HasConstraintName("FK__Comments__CardID__5441852A");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comments__UserID__5535A963");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__Labels__397E2BA38245D5DC");

            entity.Property(e => e.LabelId).HasColumnName("LabelID");
            entity.Property(e => e.Color).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.ListId).HasName("PK__Lists__E38328651AB4F9D7");

            entity.Property(e => e.ListId).HasColumnName("ListID");
            entity.Property(e => e.BoardId).HasColumnName("BoardID");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Board).WithMany(p => p.Lists)
                .HasForeignKey(d => d.BoardId)
                .HasConstraintName("FK__Lists__BoardID__4316F928");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32916742F8");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CardId).HasColumnName("CardID");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.NotifyAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Card).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.CardId)
                .HasConstraintName("FK__Notificat__CardI__59FA5E80");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__59063A47");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC36137E38");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534C5AD6E43").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
