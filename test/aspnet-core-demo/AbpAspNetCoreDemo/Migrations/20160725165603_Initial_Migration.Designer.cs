using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AbpAspNetCoreDemo.Db;

namespace AbpAspNetCoreDemo.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20160725165603_Initial_Migration")]
    partial class Initial_Migration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AbpAspNetCoreDemo.Core.Domain.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<float?>("Price");

                    b.HasKey("Id");

                    b.ToTable("AppProducts");
                });
        }
    }
}
