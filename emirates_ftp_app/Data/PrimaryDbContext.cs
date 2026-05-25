using emirates_ftp_app.Model.Customer;
using emirates_ftp_app.Model.Inbound.ASNDao;
using emirates_ftp_app.Model.Inbound.SalesOrderDao;
using emirates_ftp_app.Model.Inbound.SoCancelDao;
using emirates_ftp_app.Model.Inbound.SupplierDao;
using emirates_ftp_app.Model.Nass;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Data
{
    internal class PrimaryDbContext : DbContext
    {
        public PrimaryDbContext(DbContextOptions<PrimaryDbContext> options) : base(options) { }
        public DbSet<wms_edi_ftp_model> WMS_EDI_FTP { get; set; }
        //public DbSet<wms_edi_ftp_model> WMS_EDI_FTP1 { get; set; } enable for test error email check
        public DbSet<wms_el_client_import> WMS_EL_CLIENT_IMPORT { get; set; }
        public DbSet<wms_el_so_import> WMS_EL_SO_IMPORT { get; set; }
        public DbSet<wms_el_po_import> WMS_EL_PO_IMPORT { get; set; }
        public DbSet<wms_el_supplier_import> WMS_EL_SUPPLIER_IMPORT { get; set; }
        public DbSet<wms_el_so_cancel_import> WMS_EL_SO_CANCEL_IMPORT { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<wms_edi_ftp_model>(entity =>
            {
                entity.ToTable("WMS_EDI_FTP");
                entity.HasKey(e => e.SL_NO);

                entity.Property(e => e.FILE_CONTENT)
                      .HasColumnType("CLOB"); 
            });
            //// WMS_EDI_FTP1
            //modelBuilder.Entity<wms_edi_ftp_model>(entity =>
            //{
            //    entity.ToTable("WMS_EDI_FTP1");
            //    entity.HasKey(e => e.SL_NO);
            //}); enable for test error email check

            // WMS_EL_CLIENT_IMPORT
            modelBuilder.Entity<wms_el_client_import>(entity =>
            {
                entity.ToTable("WMS_EL_CLIENT_IMPORT");
                entity.HasKey(e => e.SL_NO);
            });

            // WMS_EL_SO_IMPORT
            modelBuilder.Entity<wms_el_so_import>(entity =>
            {
                entity.ToTable("WMS_EL_SO_IMPORT");
                entity.HasKey(e => e.SL_NO);
            });

            // WMS_EL_PO_IMPORT
            modelBuilder.Entity<wms_el_po_import>(entity =>
            {
                entity.ToTable("WMS_EL_PO_IMPORT");
                entity.HasKey(e => e.SL_NO);
            });

            // WMS_EL_SUPPLIER_IMPORT
            modelBuilder.Entity<wms_el_supplier_import>(entity =>
            {
                entity.ToTable("WMS_EL_SUPPLIER_IMPORT");
                entity.HasKey(e => e.SL_NO);
            });

            // WMS_EL_SO_CANCEL_IMPORT
            modelBuilder.Entity<wms_el_so_cancel_import>(entity =>
            {
                entity.ToTable("WMS_EL_SO_CANCEL_IMPORT");
                entity.HasKey(e => e.SL_NO);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
