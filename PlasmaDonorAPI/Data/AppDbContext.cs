using Microsoft.EntityFrameworkCore;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Dto.Dashboard;
using NewPlasmaDonorsAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NewPlasmaDonorsAPI.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public  DbSet<UserModel>? Users { get; set; } 
        public  DbSet<Company>? companies { get; set; }
        public DbSet<CompanyLocation>? companyLocation { get; set; }
        public DbSet<Roles>? roles { get; set; }
        public DbSet<ProfileModel>? profiles { get; set; }
        public DbSet<Address>? addresses { get; set; }
        public DbSet<MasterData>? MasterData { get; set; }
        public DbSet<DonarInfluencerMap>? DonarInfluencerMaps { get; set; }
        public DbSet<ProfileMdMap>? ProfileMdMaps { get; set; }
        public DbSet<DonorTimeSeriesResult>? DonorTimeSeriesResults { get; set; }
        public DbSet<InfluencerTimeSeriesResult>? InfluencerTimeSeriesResults { get; set; }
        public DbSet<ProfileByStateResult>? ProfileByStateResults { get; set; }
        public DbSet<DonorByOccupationResults>? DonorByOccupationResults { get; set; }
        public DbSet<TopInfluencerInfo>? TopInfluencersinfo { get; set; }
        public DbSet<CountDto> countDto { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users Table Configuration
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.ToTable("users");

                // Primary Key
                entity.HasKey(e => e.id);

                // Unique Constraint on Email
                entity.HasIndex(e => e.email).IsUnique();

                // Properties
                entity.Property(e => e.id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                entity.Property(e => e.deleted).HasColumnName("deleted").HasColumnType("bit");
                entity.Property(e => e.email).HasColumnName("email").HasMaxLength(255).IsRequired();
                entity.Property(e => e.firstName).HasColumnName("first_name").HasMaxLength(255);
                entity.Property(e => e.lastName).HasColumnName("last_name").HasMaxLength(255);
                entity.Property(e => e.password).HasColumnName("password").HasMaxLength(255);
                entity.Property(e => e.phoneNumber).HasColumnName("phone_number").HasMaxLength(255);
                entity.Property(e => e.role).HasColumnName("role").HasMaxLength(255);
                entity.Property(e => e.username).HasColumnName("username").HasMaxLength(255);
                entity.Property(e => e.company).HasColumnName("company_id").HasMaxLength(255);
                entity.Property(e => e.companyLocation).HasColumnName("company_location_id");
                entity.Property(e => e.roleType).HasColumnName("role_type").HasMaxLength(255);
                entity.Property(e => e.roleId).HasColumnName("role_id");
                entity.Property(e => e.status).HasColumnName("status").HasMaxLength(255);
                entity.Property(e => e.createdOn).HasColumnName("created_on").HasColumnType("datetime(6)").IsRequired(false);
                entity.Property(e => e.updatedOn).HasColumnName("updated_on").HasColumnType("datetime(6)").IsRequired(false);
                entity.Property(e => e.createdBy).HasColumnName("created_by");
                entity.Property(e => e.updatedBy).HasColumnName("updated_by");

                // Foreign Key Relationships
                entity.HasOne(e => e.Company)
                      .WithMany() // Adjust this if the Company entity has a navigation property for users
                      .HasForeignKey(e => e.company)
                      .HasConstraintName("FKin8gn4o1hpiwe6qe4ey7ykwq7")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CompanyLocation)
                      .WithMany() // Adjust this if the CompanyLocation entity has a navigation property for users
                      .HasForeignKey(e => e.companyLocation)
                      .HasConstraintName("FKj8wlutnj1on6p9b8m9lhuyylh")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RoleNavigation)
                      .WithMany() // Adjust this if the Role entity has a navigation property for users
                      .HasForeignKey(e => e.roleId)
                      .HasConstraintName("FKp56c1712k691lhsyewcssf40f")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.ToTable("roles"); // Table name

                // Primary Key
                entity.HasKey(e => e.id);

                // Properties
                entity.Property(e => e.id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();

                entity.Property(e => e.role).HasColumnName("role").HasMaxLength(255);

                entity.Property(e => e.status).HasColumnName("status").HasMaxLength(255);

                entity.Property(e => e.title).HasColumnName("title").HasMaxLength(255);
            });

            modelBuilder.Entity<CompanyLocation>(static entity =>
            {
                entity.ToTable("company_locations");

                // Primary Key
                entity.HasKey(e => e.id);

                // Properties
                entity.Property(e => e.id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                entity.Property(e => e.addressLine1).HasColumnName("address_line1").HasMaxLength(255);
                entity.Property(e => e.addressLine2).HasColumnName("address_line2").HasMaxLength(255);
                entity.Property(e => e.city).HasColumnName("city").HasMaxLength(255);
                entity.Property(e => e.country).HasColumnName("country").HasMaxLength(255);
                entity.Property(e => e.countryCode).HasColumnName("country_code").HasMaxLength(255);
                entity.Property(e => e.fullAddress).HasColumnName("full_address").HasMaxLength(255);
                entity.Property(e => e.latitude).HasColumnName("latitude");
                entity.Property(e => e.longitude).HasColumnName("longitude");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(255);
                entity.Property(e => e.state).HasColumnName("state").HasMaxLength(255);
                entity.Property(e => e.stateCode).HasColumnName("state_code").HasMaxLength(255);
                entity.Property(e => e.status).HasColumnName("status").HasMaxLength(255);
                entity.Property(e => e.phoneNumber).HasColumnName("phone_number").HasMaxLength(255);
                entity.Property(e => e.siteId).HasColumnName("site_id").HasMaxLength(255);
          });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("companies");
                entity.Property(e => e.id).HasColumnName("id");
                entity.Property(e => e.name).HasColumnName("name").HasMaxLength(255);
                entity.Property(e => e.status).HasColumnName("status").HasMaxLength(255);
            });

            modelBuilder.Entity<ProfileModel>(entity =>
            {
                entity.ToTable("profiles");

                // Primary Key
                entity.HasKey(e => e.id);

                // Properties
                entity.Property(e => e.id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                entity.Property(e => e.age).HasColumnName("age");
                entity.Property(e => e.createdOn).HasColumnName("created_on").HasColumnType("datetime(6)").IsRequired(false);
                entity.Property(e => e.dob).HasColumnName("dob").HasColumnType("datetime(6)").IsRequired(false);
                entity.Property(e => e.email).HasColumnName("email").HasMaxLength(255);
                entity.Property(e => e.firstName).HasColumnName("first_name").HasMaxLength(255).IsRequired();
                entity.Property(e => e.gender).HasColumnName("gender").HasMaxLength(255);
                entity.Property(e => e.isDonor).HasColumnName("is_donor").HasColumnType("bit");
                entity.Property(e => e.isInfluencer).HasColumnName("is_influencer").HasColumnType("bit");
                entity.Property(e => e.lastName).HasColumnName("last_name").HasMaxLength(255);
                entity.Property(e => e.phoneNumber).HasColumnName("phone_number").HasMaxLength(255);
                entity.Property(e => e.schoolAttended).HasColumnName("school_attended").HasMaxLength(255);
                entity.Property(e => e.updatedOn).HasColumnName("updated_on").HasColumnType("datetime(6)").IsRequired(false);
                entity.Property(e => e.address_id).HasColumnName("address_id");
                entity.Property(e => e.createdBy).HasColumnName("created_by");
                entity.Property(e => e.education_id).HasColumnName("education_id");
                entity.Property(e => e.language_id).HasColumnName("language_id");
                entity.Property(e => e.occupation_id).HasColumnName("occupation_id");
                entity.Property(e => e.race_id).HasColumnName("race_id");
                entity.Property(e => e.relationship_id).HasColumnName("relationship_id");
                entity.Property(e => e.relship_reason_id).HasColumnName("relship_reason_id");
                entity.Property(e => e.updatedBy).HasColumnName("updated_by");
                entity.Property(e => e.HomeCenter).HasColumnName("home_center");
                entity.Property(e => e.InfluencedBy).HasColumnName("influenced_by").HasMaxLength(255);
                entity.Property(e => e.IsRelationshipActive).HasColumnName("is_relationship_active").HasColumnType("bit");
                entity.Property(e => e.RelshipScoreId).HasColumnName("relship_score_id");
                entity.Property(e => e.RelshipStatus).HasColumnName("relship_status").HasMaxLength(255);
                entity.Property(e => e.HomeCenterId).HasColumnName("home_center_id");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(255);

                // Foreign Key Relationships
                entity.HasOne(e => e.Address)
                      .WithMany()
                      .HasForeignKey(e => e.address_id)
                      .HasConstraintName("FK7qb4kty1ih8s2hauc511hxn6n")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.createdBy)
                      .HasConstraintName("FKiaff94dptc6thrqcc7lup4f68")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UpdatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.updatedBy)
                      .HasConstraintName("FKmfmr6vf2ktvwggcqquiumitq7")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Education)
                      .WithMany()
                      .HasForeignKey(e => e.education_id)
                      .HasConstraintName("FK6dvtxtc424ogal2jtnxdh7c0")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Language)
                      .WithMany()
                      .HasForeignKey(e => e.language_id)
                      .HasConstraintName("FKrjn1ucsoa08h67xg24ypuqwxw")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Occupation)
                      .WithMany()
                      .HasForeignKey(e => e.occupation_id)
                      .HasConstraintName("FK45nymxuj55tpmnrj2v2cm08vk")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Race)
                      .WithMany()
                      .HasForeignKey(e => e.race_id)
                      .HasConstraintName("FKryh63mrluhx6dv2xhewpn88yk")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Relationship)
                      .WithMany()
                      .HasForeignKey(e => e.relationship_id)
                      .HasConstraintName("FK7h9k3wpqia2jss4yvlbyf94wa")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RelshipReason)
                      .WithMany()
                      .HasForeignKey(e => e.relship_reason_id)
                      .HasConstraintName("FKgrdgwmydu1ovly29y5b5o3xlp")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RelshipScore)
                      .WithMany()
                      .HasForeignKey(e => e.RelshipScoreId)
                      .HasConstraintName("FKbpb7gg85pmbuiri25bmju0hat")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.HomeCenterLocation)
                      .WithMany()
                      .HasForeignKey(e => e.HomeCenterId)
                      .HasConstraintName("FK15rkj9sb7mqtyitx20a9n4m")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address");

                // Primary Key
                entity.HasKey(e => e.id);

                // Properties
                entity.Property(e => e.id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                entity.Property(e => e.addressLine).HasColumnName("address_line").HasMaxLength(255);
                entity.Property(e => e.city).HasColumnName("city").HasMaxLength(255);
                entity.Property(e => e.country).HasColumnName("country").HasMaxLength(255);
                entity.Property(e => e.countryCode).HasColumnName("country_code").HasMaxLength(255);
                entity.Property(e => e.createdOn).HasColumnName("created_on").HasColumnType("datetime(6)").IsRequired(false);
                entity.Property(e => e.fullAddress).HasColumnName("full_address").HasMaxLength(255);
                entity.Property(e => e.latitude).HasColumnName("latitude");
                entity.Property(e => e.longitude).HasColumnName("longitude");
                entity.Property(e => e.state).HasColumnName("state").HasMaxLength(255);
                entity.Property(e => e.stateCode).HasColumnName("state_code").HasMaxLength(255);
                entity.Property(e => e.status).HasColumnName("status").HasMaxLength(255);
                entity.Property(e => e.updatedOn).HasColumnName("updated_on").HasColumnType("datetime(6)").IsRequired(false);
                entity.Property(e => e.createdBy).HasColumnName("created_by");
                entity.Property(e => e.updatedBy).HasColumnName("updated_by");
                entity.Property(e => e.postalCode).HasColumnName("postal_code").HasMaxLength(255);

                // Foreign Key Relationships
                entity.HasOne(e => e.createdByUser)
                      .WithMany() // Adjust if User has a collection of addresses
                      .HasForeignKey(e => e.createdBy)
                      .HasConstraintName("FKf2n4qm0147jxllli3xeutu6uv")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.updatedByUser)
                      .WithMany() // Adjust if User has a collection of addresses
                      .HasForeignKey(e => e.updatedBy)
                      .HasConstraintName("FKhmbu7a78tyb3hir23nnt10wcy")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MasterData>(entity =>
            {
                entity.ToTable("master_data");

                // Primary Key
                entity.HasKey(e => e.id);

                // Properties
                entity.Property(e => e.id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                entity.Property(e => e.createdOn).HasColumnName("created_on").HasColumnType("datetime(6)").IsRequired(false);
                entity.Property(e => e.mdName).HasColumnName("md_name").HasMaxLength(255);
                entity.Property(e => e.mdTitle).HasColumnName("md_title").HasMaxLength(255);
                entity.Property(e => e.mdType).HasColumnName("md_type").HasMaxLength(255);
                entity.Property(e => e.status).HasColumnName("status").HasMaxLength(255);
                entity.Property(e => e.updatedOn).HasColumnName("updated_on").HasColumnType("datetime(6)").IsRequired(false);
                entity.Property(e => e.createdBy).HasColumnName("created_by");
                entity.Property(e => e.updatedBy).HasColumnName("updated_by");
                entity.Property(e => e.mdScore).HasColumnName("md_score").HasMaxLength(255);

                // Foreign Key Relationships
                entity.HasOne(e => e.createdByUser)
                      .WithMany() // Adjust if User has a collection of MasterData
                      .HasForeignKey(e => e.createdBy)
                      .HasConstraintName("FKjxlsr7j04idi7yb0cjutfoij6")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.updatedByUser)
                      .WithMany() // Adjust if User has a collection of MasterData
                      .HasForeignKey(e => e.updatedBy)
                      .HasConstraintName("FK6au4d9c97e31e0p1a5m4hok7e")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DonarInfluencerMap>(entity =>

            {
                entity.ToTable("donar_influencer_map");

                // Primary Key
                entity.HasKey(d => d.id);

                entity.Property(e => e.id).HasColumnName("id");

                // Configure the foreign key for ProfileId
                entity.Property(e => e.profileId).HasColumnName("profile_id");

                // Configure the foreign key for InfluencedById
                entity.Property(e => e.influencedBy).HasColumnName("influenced_by");
                // Foreign Key Relationships
                entity.HasOne(d => d.Profile)
                      .WithMany() // Adjust this if ProfileModel has a collection of DonarInfluencerMap
                      .HasForeignKey(d => d.profileId)
                      .HasConstraintName("FKj9a68x999klndsilsnp5ap77s")
                      .OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior if necessary

                entity.HasOne(d => d.InfluencerProfile)
                      .WithMany() // Adjust this if ProfileModel has a collection of DonarInfluencerMap
                      .HasForeignKey(d => d.influencedBy)
                      .HasConstraintName("FKc1nras6llm81y4y25yh3ayjyb")
                      .OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior if necessary
            });

            modelBuilder.Entity<ProfileMdMap>(entity =>
            {
                entity.ToTable("profile_md_map"); // Table name

                // Primary Key
                entity.HasKey(e => e.id);

                // Properties
                entity.Property(e => e.id)
                    .HasColumnName("id")
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.profileId)
                    .HasColumnName("profile_id")
                    .IsRequired(false);

                entity.Property(e => e.mdId)
                    .HasColumnName("md_id")
                    .IsRequired(false);

                // Foreign Key Relationships
                entity.HasOne(e => e.profile)
                    .WithMany()
                    .HasForeignKey(e => e.profileId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.md)
                    .WithMany()
                    .HasForeignKey(e => e.mdId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TopInfluencerInfo>()
            .HasNoKey() // No primary key since it's a raw SQL result
            .ToView(null); // Ensures EF Core doesn’t treat it as a table

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CountDto>().HasNoKey();
        }
    }
}

