using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecAll.Contrib.MaskedTextList.Api.Models;

namespace RecAll.Contrib.MaskedTextList.Api.Services;

public class MaskedTextListContext : DbContext {
    public const string DefaultSchema = "MaskedTextList";

    public DbSet<MaskedTextItem> MaskedTextItems { get; set; }

    public MaskedTextListContext(DbContextOptions<MaskedTextListContext> options) :
        base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfiguration(new MaskedTextItemConfiguration());
    }
}

public class MaskedTextItemConfiguration : IEntityTypeConfiguration<MaskedTextItem> {
    public void Configure(EntityTypeBuilder<MaskedTextItem> builder) {
        builder.ToTable("textitems");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .UseHiLo("textitemseq", MaskedTextListContext.DefaultSchema);

        builder.Property(p => p.ItemId).IsRequired(false);
        builder.HasIndex(p => p.ItemId).IsUnique();

        builder.Property(p => p.Content).IsRequired();

        builder.Property(p => p.UserIdentityGuid).IsRequired();
        builder.HasIndex(p => p.UserIdentityGuid).IsUnique(false);

        builder.Property(p => p.IsDeleted).IsRequired();
            
        builder.Property(p => p.IsHidden).IsRequired();
        builder.Property(p => p.MaskedContent).IsRequired();
    }
}

public class
    MaskedTextListContextDesignFactory : IDesignTimeDbContextFactory<
        MaskedTextListContext> {
    public MaskedTextListContext CreateDbContext(string[] args) {
        return new MaskedTextListContext(
            new DbContextOptionsBuilder<MaskedTextListContext>()
                .UseSqlServer(
                    "Server=.;Initial Catalog=RecAll.MaskedTextListDb;Integrated Security=true")
                .Options);
    }
}