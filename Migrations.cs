#region

using System;
using Orchard.Data;
using Orchard.Data.Migration;
using Rijkshuisstijl.UrlProtector.Models;

#endregion

namespace Rijkshuisstijl.UrlProtector
{
    public class Migrations : DataMigrationImpl
    {
        private readonly IRepository<DashboardFilterRecord> _dashboardFilterRecord;

        public Migrations(IRepository<DashboardFilterRecord> dashboardFilterRecord)
        {
            _dashboardFilterRecord = dashboardFilterRecord;
        }

        public int Create()
        {
            //Create the table
            SchemaBuilder.CreateTable("UrlFilterRecord", table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<int>("UrlPriority")
                .Column<string>("UrlPattern")
                .Column<string>("UserHostAddressPattern")
                .Column<string>("UserAgentPattern")
                .Column<string>("Description")
                .Column<bool>("ForceSsl")
                .Column<int>("FailureAction")
                .Column<string>("RedirectTo"));

            //Create the table
            SchemaBuilder.CreateTable("FilteredRequestRecord", table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<DateTime>("RequestTime")
                .Column<string>("Url")
                .Column<string>("UserAgent")
                .Column<string>("UserHostAddress"));

            //Create the table
            SchemaBuilder.CreateTable("DashboardFilterRecord", table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("UserHostAddressPattern")
                .Column<string>("UserAgentPattern")
                .Column<bool>("ReturnStatusNotFound")
                .Column<bool>("ForceSsl"));

            //Create default settings for the dashboard protection
            _dashboardFilterRecord.Create(new DashboardFilterRecord
            {
                Id = 1,
                UserAgentPattern = ".*",
                UserHostAddressPattern = ".*",
                ReturnStatusNotFound = false,
                ForceSsl = false
            });

            return 2;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("UrlFilterRecord", table => table
                .DropColumn("ReturnStatusNotFound"));
            SchemaBuilder.AlterTable("UrlFilterRecord", table => table
                .AddColumn<int>("FailureAction"));
            SchemaBuilder.AlterTable("UrlFilterRecord", table => table
                .AddColumn<string>("RedirectTo"));

            return 2;
        }
    }
}