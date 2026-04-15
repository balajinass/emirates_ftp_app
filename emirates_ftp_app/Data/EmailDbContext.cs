//using emirates_ftp_app.Model.Email;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace emirates_ftp_app.Data
{
    internal class EmailDbContext : DbContext
    {
        public DbSet<Email_Scheduler>? WEB_EMAIL_SCHEDULER { get; set; }

        public DbSet<Email_attachment_Scheduler>? WEB_EMAIL_ATTACHMENT_SCHEDULER { get; set; }

        public DbSet<EmailTemplate>? WEB_EMAIL_TEMPLATE_MASTER { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Email_Scheduler>().HasKey(i => i.ID);
            modelBuilder.Entity<Email_attachment_Scheduler>().HasKey(b => b.ID);
            modelBuilder.Entity<Email_attachment_Scheduler>()
              .HasOne(b => b.EMAIL_LOG)
              .WithMany(i => i.ATTACHMENTS)
              .HasForeignKey(b => b.HEADER_ID);
            modelBuilder.Entity<EmailTemplate>().HasKey(b => b.ID);
        }
    }
}
