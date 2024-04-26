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
    [Migration("20240426125017_InitFixedContex")]
    partial class InitFixedContex
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

            modelBuilder.Entity("MatGPT.Models.FoodItem", b =>
                {
                    b.Property<int>("FoodItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FoodItemId"), 1L, 1);

                    b.Property<double?>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("FoodItemName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("FoodItemId");

                    b.HasIndex("UserId");

                    b.ToTable("FoodItems");
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

            modelBuilder.Entity("MatGPT.Models.PantryFoodItem", b =>
                {
                    b.Property<int>("PantryFoodItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PantryFoodItemId"), 1L, 1);

                    b.Property<int>("FoodItemId")
                        .HasColumnType("int");

                    b.Property<int>("PantryId")
                        .HasColumnType("int");

                    b.HasKey("PantryFoodItemId");

                    b.HasIndex("FoodItemId");

                    b.HasIndex("PantryId");

                    b.ToTable("PantryFoodItems");
                });

            modelBuilder.Entity("MatGPT.Models.Recipe", b =>
                {
                    b.Property<int>("RecipeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RecipeId"), 1L, 1);

                    b.Property<int?>("CookingTime")
                        .HasColumnType("int");

                    b.Property<string>("RecipeDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecipeName")
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

            modelBuilder.Entity("MatGPT.Models.FoodItem", b =>
                {
                    b.HasOne("MatGPT.Models.User", "User")
                        .WithMany("FoodItems")
                        .HasForeignKey("UserId")
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
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MatGPT.Models.PantryFoodItem", b =>
                {
                    b.HasOne("MatGPT.Models.FoodItem", "FoodItem")
                        .WithMany("PantryFoodItems")
                        .HasForeignKey("FoodItemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MatGPT.Models.Pantry", "Pantry")
                        .WithMany("PantryFoodItems")
                        .HasForeignKey("PantryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("FoodItem");

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

            modelBuilder.Entity("MatGPT.Models.FoodItem", b =>
                {
                    b.Navigation("PantryFoodItems");
                });

            modelBuilder.Entity("MatGPT.Models.Pantry", b =>
                {
                    b.Navigation("PantryFoodItems");
                });

            modelBuilder.Entity("MatGPT.Models.User", b =>
                {
                    b.Navigation("Credential")
                        .IsRequired();

                    b.Navigation("FoodItems");

                    b.Navigation("FoodPreferences");

                    b.Navigation("KitchenSupplies");

                    b.Navigation("Recipes");
                });
#pragma warning restore 612, 618
        }
    }
}
