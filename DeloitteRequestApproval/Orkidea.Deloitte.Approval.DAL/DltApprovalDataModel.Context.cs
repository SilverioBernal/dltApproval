﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Orkidea.Deloitte.Approval.DAL
{
    using Orkidea.Deloitte.Approval.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class dltApprovalEntities : DbContext
    {
        public dltApprovalEntities()
            : base("name=dltApprovalEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DistributionList> DistributionList { get; set; }
        public virtual DbSet<ApprovalRequest> ApprovalRequest { get; set; }
        public virtual DbSet<WebUser> WebUser { get; set; }
        public virtual DbSet<vwRequestReport> vwRequestReport { get; set; }
    }
}