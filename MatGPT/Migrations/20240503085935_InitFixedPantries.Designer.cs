﻿// <auto-generated />
using System;
using MatGPT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MatGPT.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240503085935_InitFixedPantries")]
    partial class InitFixedPantries
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.29")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MatGPT.Models.Credential", b =>
                {
                    b.Property<int>("CredentialId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CredentialId"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("CredentialId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Credentials");
                });

            modelBuilder.Entity("MatGPT.Models.FoodPreference", b =>
                {
                    b.Property<int>("FoodPreferenceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FoodPreferenceId"), 1L, 1);

                    b.Property<string>("FoodPreferenceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("FoodPreferenceId");

                    b.HasIndex("UserId");

                    b.ToTable("FoodPreferences");
                });

            modelBuilder.Entity("MatGPT.Models.Ingredient", b =>
                {
                    b.Property<int>("IngredientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IngredientId"), 1L, 1);

                    b.Property<double?>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("IngredientName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("IngredientId");

                    b.HasIndex("UserId");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("MatGPT.Models.KitchenSupply", b =>
                {
                    b.Property<int>("KitchenSupplyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("KitchenSupplyId"), 1L, 1);

                    b.Property<string>("KitchenSupplyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("KitchenSupplyId");

                    b.HasIndex("UserId");

                    b.ToTable("KitchenSupply");
                });

            modelBuilder.Entity("MatGPT.Models.Pantry", b =>
                {
                    b.Property<int>("PantryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PantryId"), 1L, 1);

                    b.Property<string>("PantryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("PantryId");

                    b.HasIndex("UserId");

                    b.ToTable("Pantries");
                });

            modelBuilder.Entity("MatGPT.Models.PantryIngredient", b =>
                {
                    b.Property<int>("PantryIngredientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PantryIngredientId"), 1L, 1);

                    b.Property<int>("IngredientId")
                        .HasColumnType("int");

                    b.Property<int>("PantryId")
                        .HasColumnType("int");

                    b.HasKey("PantryIngredientId");

                    b.HasIndex("IngredientId");

                    b.HasIndex("PantryId");

                    b.ToTable("PantryIngredients");
                });

            modelBuilder.Entity("MatGPT.Models.Recipe", b =>
                {
                    b.Property<int>("RecipeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RecipeId"), 1L, 1);

                    b.Property<int?>("CookingTime")
                        .HasColumnType("int");

                    b.Property<string>("Ingredients")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Instructions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("RecipeId");

                    b.HasIndex("UserId");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("MatGPT.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MatGPT.Models.Credential", b =>
                {
                    b.HasOne("MatGPT.Models.User", "User")
                        .WithOne("Credential")
                        .HasForeignKey("MatGPT.Models.Credential", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MatGPT.Models.FoodPreference", b =>
                {
                    b.HasOne("MatGPT.Models.User", "User")
                        .WithMany("FoodPreferences")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MatGPT.Models.Ingredient", b =>
                {
                    b.HasOne("MatGPT.Models.User", "User")
                        .WithMany("Ingredients")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MatGPT.Models.KitchenSupply", b =>
                {
                    b.HasOne("MatGPT.Models.User", "User")
                        .WithMany("KitchenSupplies")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MatGPT.Models.Pantry", b =>
                {
                    b.HasOne("MatGPT.Models.User", "User")
                        .WithMany("Pantries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MatGPT.Models.PantryIngredient", b =>
                {
                    b.HasOne("MatGPT.Models.Ingredient", "Ingredient")
                        .WithMany("PantryIngredients")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MatGPT.Models.Pantry", "Pantry")
                        .WithMany("PantryIngredients")
                        .HasForeignKey("PantryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Ingredient");

                    b.Navigation("Pantry");
                });

            modelBuilder.Entity("MatGPT.Models.Recipe", b =>
                {
                    b.HasOne("MatGPT.Models.User", "User")
                        .WithMany("Recipes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MatGPT.Models.Ingredient", b =>
                {
                    b.Navigation("PantryIngredients");
                });

            modelBuilder.Entity("MatGPT.Models.Pantry", b =>
                {
                    b.Navigation("PantryIngredients");
                });

            modelBuilder.Entity("MatGPT.Models.User", b =>
                {
                    b.Navigation("Credential")
                        .IsRequired();

                    b.Navigation("FoodPreferences");

                    b.Navigation("Ingredients");

                    b.Navigation("KitchenSupplies");

                    b.Navigation("Pantries");

                    b.Navigation("Recipes");
                });
#pragma warning restore 612, 618
        }
    }
}
