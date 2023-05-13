﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecAll.Core.List.Infrastructure;

#nullable disable

namespace RecAll.Core.List.Infrastructure.Migrations
{
    [DbContext(typeof(ListContext))]
    partial class ListContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.HasSequence("itemseq", "list")
                .IncrementsBy(10);

            modelBuilder.HasSequence("listseq", "list")
                .IncrementsBy(10);

            modelBuilder.HasSequence("setseq", "list")
                .IncrementsBy(10);

            modelBuilder.Entity("RecAll.Core.List.Domain.AggregateModels.ItemAggregate.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseHiLo(b.Property<int>("Id"), "itemseq", "list");

                    b.Property<string>("ContribId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("ContribId");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("IsDeleted");

                    b.Property<string>("UserIdentityGuid")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserIdentityGuid");

                    b.Property<int>("_setId")
                        .HasColumnType("int")
                        .HasColumnName("SetId");

                    b.Property<int>("_typeId")
                        .HasColumnType("int")
                        .HasColumnName("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("ContribId");

                    b.HasIndex("_setId");

                    b.HasIndex("_typeId");

                    b.ToTable("items", (string)null);
                });

            modelBuilder.Entity("RecAll.Core.List.Domain.AggregateModels.ListAggregate.List", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseHiLo(b.Property<int>("Id"), "listseq", "list");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("IsDeleted");

                    b.Property<string>("UserIdentityGuid")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserIdentityGuid");

                    b.Property<string>("_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<int>("_typeId")
                        .HasColumnType("int")
                        .HasColumnName("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("_typeId");

                    b.ToTable("lists", (string)null);
                });

            modelBuilder.Entity("RecAll.Core.List.Domain.AggregateModels.ListType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("listtypes", (string)null);
                });

            modelBuilder.Entity("RecAll.Core.List.Domain.AggregateModels.SetAggregate.Set", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseHiLo(b.Property<int>("Id"), "setseq", "list");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("IsDeleted");

                    b.Property<string>("UserIdentityGuid")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserIdentityGuid");

                    b.Property<int>("_listId")
                        .HasColumnType("int")
                        .HasColumnName("ListId");

                    b.Property<string>("_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<int>("_typeId")
                        .HasColumnType("int")
                        .HasColumnName("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("_listId");

                    b.HasIndex("_typeId");

                    b.ToTable("sets", (string)null);
                });

            modelBuilder.Entity("RecAll.Core.List.Domain.AggregateModels.ItemAggregate.Item", b =>
                {
                    b.HasOne("RecAll.Core.List.Domain.AggregateModels.SetAggregate.Set", null)
                        .WithMany()
                        .HasForeignKey("_setId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("RecAll.Core.List.Domain.AggregateModels.ListType", "Type")
                        .WithMany()
                        .HasForeignKey("_typeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Type");
                });

            modelBuilder.Entity("RecAll.Core.List.Domain.AggregateModels.ListAggregate.List", b =>
                {
                    b.HasOne("RecAll.Core.List.Domain.AggregateModels.ListType", "Type")
                        .WithMany()
                        .HasForeignKey("_typeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Type");
                });

            modelBuilder.Entity("RecAll.Core.List.Domain.AggregateModels.SetAggregate.Set", b =>
                {
                    b.HasOne("RecAll.Core.List.Domain.AggregateModels.ListAggregate.List", null)
                        .WithMany()
                        .HasForeignKey("_listId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RecAll.Core.List.Domain.AggregateModels.ListType", "Type")
                        .WithMany()
                        .HasForeignKey("_typeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Type");
                });
#pragma warning restore 612, 618
        }
    }
}
