using emirates_ftp_app.Model.Nass;
using Microsoft.EntityFrameworkCore;

namespace emirates_ftp_app.Data
{
    internal class NassDbContext : DbContext
    {
        public NassDbContext(DbContextOptions<NassDbContext> options) : base(options) { }
        public DbSet<web_wms_edi_config_model> WEB_WMS_EDI_CONFIG { get; set; }
        public DbSet<web_wms_edi_module_config_model> WEB_WMS_EDI_MODULE_CONFIG { get; set; }
        public DbSet<web_wms_edi_outbound_config> WEB_WMS_EDI_OUTBOUND_CONFIG { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // WEB_WMS_EDI_CONFIG
            modelBuilder.Entity<web_wms_edi_config_model>(entity =>
            {
                entity.ToTable("WEB_WMS_EDI_CONFIG");
                entity.HasKey(e => e.PROJECT_ID); // primary key is required

                entity.Property(e => e.CREATE_USER).HasColumnName("CREATE_USER");
                entity.Property(e => e.CREATE_DATE).HasColumnName("CREATE_DATE");
                entity.Property(e => e.RUN_USER).HasColumnName("RUN_USER");
                entity.Property(e => e.RUN_DATE).HasColumnName("RUN_DATE");
                entity.Property(e => e.PROJECT_ID).HasColumnName("PROJECT_ID");
                entity.Property(e => e.PROJECT_NAME).HasColumnName("PROJECT_NAME");
                entity.Property(e => e.SAAS_ID).HasColumnName("SAAS_ID");
                entity.Property(e => e.COMPANY_CODE).HasColumnName("COMPANY_CODE");
                entity.Property(e => e.BRANCH_CODE).HasColumnName("BRANCH_CODE");
                entity.Property(e => e.LOCATION_CODE).HasColumnName("LOCATION_CODE");
                entity.Property(e => e.WAREHOUSE_CODE).HasColumnName("WAREHOUSE_CODE");
                entity.Property(e => e.CUSTOMER_CODE).HasColumnName("CUSTOMER_CODE");
                entity.Property(e => e.API_KEY).HasColumnName("API_KEY");
                entity.Property(e => e.FTP_URL).HasColumnName("FTP_URL");
                entity.Property(e => e.FTP_PORT).HasColumnName("FTP_PORT");
                entity.Property(e => e.FTP_USERNAME).HasColumnName("FTP_USERNAME");
                entity.Property(e => e.FTP_PASSWORD).HasColumnName("FTP_PASSWORD");
                entity.Property(e => e.FROM_EMAIL).HasColumnName("FROM_EMAIL");
                entity.Property(e => e.TO_EMAIL).HasColumnName("TO_EMAIL");
                entity.Property(e => e.CC_EMAIL).HasColumnName("CC_EMAIL");
                entity.Property(e => e.ERROR_EMAIL).HasColumnName("ERROR_EMAIL");
                entity.Property(e => e.LOV_STATUS).HasColumnName("LOV_STATUS");

                entity.HasMany(e => e.MODULES)
                      .WithOne()
                      .HasForeignKey(m => m.PROJECT_ID);

                entity.HasMany(e => e.OUTBOUND)
                      .WithOne() 
                      .HasForeignKey(o => o.PROJECT_ID);
            });

            // WEB_WMS_EDI_MODULE_CONFIG
            modelBuilder.Entity<web_wms_edi_module_config_model>(entity =>
            {
                entity.ToTable("WEB_WMS_EDI_MODULE_CONFIG");
                entity.HasKey(e => new { e.PROJECT_ID, e.SL_NO }); // composite key

                entity.Property(e => e.CREATE_USER).HasColumnName("CREATE_USER");
                entity.Property(e => e.CREATE_DATE).HasColumnName("CREATE_DATE");
                entity.Property(e => e.RUN_USER).HasColumnName("RUN_USER");
                entity.Property(e => e.RUN_DATE).HasColumnName("RUN_DATE");
                entity.Property(e => e.PROJECT_ID).HasColumnName("PROJECT_ID");
                entity.Property(e => e.SL_NO).HasColumnName("SL_NO");
                entity.Property(e => e.MODULE_NAME).HasColumnName("MODULE_NAME");
                entity.Property(e => e.FTP_FILE_PATH).HasColumnName("FTP_FILE_PATH");
                entity.Property(e => e.FTP_FILE_BACKUP_PATH).HasColumnName("FTP_FILE_BACKUP_PATH");
                entity.Property(e => e.FTP_FILE_ERROR_PATH).HasColumnName("FTP_FILE_ERROR_PATH");
                entity.Property(e => e.LOCAL_FILE_PATH).HasColumnName("LOCAL_FILE_PATH");
                entity.Property(e => e.LOCAL_FILE_BACKUP_PATH).HasColumnName("LOCAL_FILE_BACKUP_PATH");
                entity.Property(e => e.LOCAL_FILE_ERROR_PATH).HasColumnName("LOCAL_FILE_ERROR_PATH");
                entity.Property(e => e.LOV_STATUS).HasColumnName("LOV_STATUS");
            });

            // WEB_WMS_EDI_OUTBOUND_CONFIG
            modelBuilder.Entity<web_wms_edi_outbound_config>(entity =>
            {
                entity.ToTable("WEB_WMS_EDI_OUTBOUND_CONFIG");
                entity.HasKey(e => e.SL_NO); // Or composite key if needed
                entity.Property(e => e.CREATE_USER).HasColumnName("CREATE_USER");
                entity.Property(e => e.CREATE_DATE).HasColumnName("CREATE_DATE");
                entity.Property(e => e.RUN_USER).HasColumnName("RUN_USER");
                entity.Property(e => e.RUN_DATE).HasColumnName("RUN_DATE");
                entity.Property(e => e.PROJECT_ID).HasColumnName("PROJECT_ID");
                entity.Property(e => e.FILE_TYPE).HasColumnName("FILE_TYPE");
                entity.Property(e => e.MODULE_NAME).HasColumnName("MODULE_NAME");
                entity.Property(e => e.FTP_FILE_PATH).HasColumnName("FTP_FILE_PATH");
                entity.Property(e => e.LOCAL_FILE_PATH).HasColumnName("LOCAL_FILE_PATH");
                entity.Property(e => e.LOV_STATUS).HasColumnName("LOV_STATUS");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}