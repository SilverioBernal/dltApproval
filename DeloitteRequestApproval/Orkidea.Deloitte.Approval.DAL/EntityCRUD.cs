using Orkidea.Framework.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Deloitte.Approval.DAL
{
    public class EntityCRUD<T> : IEntityCRUD<T> where T : class
    {
        public virtual IList<T> GetAll(params Expression<Func<T, object>>[] navigationProperties)
        {
            IList<T> list = null;
            try
            {
                using (var context = new dltApprovalEntities())
                {
                    //context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;
                    IQueryable<T> dbQuery = context.Set<T>();

                    //Apply eager loading
                    foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                        dbQuery = dbQuery.Include<T, object>(navigationProperty);

                    list = dbQuery
                        .AsNoTracking()
                        .ToList<T>();
                }
            }
            catch (ArgumentException ex)
            {
                string lola = ex.Message;
            }
            catch (Exception ex)
            {
                string lola = ex.Message;
                throw;
            }
            return list;
        }

        public virtual IList<T> GetList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            IList<T> list;
            using (var context = new dltApprovalEntities())
            {
                //context.Configuration.ProxyCreationEnabled = false;
                context.Configuration.LazyLoadingEnabled = false;
                IQueryable<T> dbQuery = context.Set<T>();

                //Apply eager loading
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include<T, object>(navigationProperty);

                list = dbQuery
                    .AsNoTracking()
                    .Where(where)
                    .ToList<T>();
            }
            return list;
        }

        public virtual T GetSingle(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;
            using (var context = new dltApprovalEntities())
            {
                IQueryable<T> dbQuery = context.Set<T>();

                //Apply eager loading
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include<T, object>(navigationProperty);

                item = dbQuery
                    .AsNoTracking() //Don't track any changes for the selected item
                    .FirstOrDefault(where); //Apply where clause
            }
            return item;
        }

        public virtual void Add(params T[] items)
        {
            using (var context = new dltApprovalEntities())
            {
                DbSet<T> dbSet = context.Set<T>();
                foreach (T item in items)
                {
                    dbSet.Add(item);
                    foreach (DbEntityEntry<T> entry in context.ChangeTracker.Entries<T>())
                    {
                        T entity = entry.Entity;
                        entry.State = EntityState.Added;
                    }
                }
                context.SaveChanges();
            }
        }

        public virtual T Add(T item)
        {
            using (var context = new dltApprovalEntities())
            {
                DbSet<T> dbSet = context.Set<T>();
                dbSet.Add(item);
                foreach (DbEntityEntry<T> entry in context.ChangeTracker.Entries<T>())
                {
                    T entity = entry.Entity;
                    entry.State = EntityState.Added;
                }

                context.SaveChanges();
                return item;
            }
        }

        public virtual void Update(params T[] items)
        {
            using (var context = new dltApprovalEntities())
            {
                DbSet<T> dbSet = context.Set<T>();
                foreach (T item in items)
                {
                    dbSet.Add(item);
                    foreach (DbEntityEntry<T> entry in context.ChangeTracker.Entries<T>())
                    {
                        T entity = entry.Entity;
                        entry.State = EntityState.Modified;
                    }
                }
                context.SaveChanges();
            }
        }

        public virtual void Remove(params T[] items)
        {
            using (var context = new dltApprovalEntities())
            {
                DbSet<T> dbSet = context.Set<T>();
                foreach (T item in items)
                {
                    dbSet.Add(item);
                    foreach (DbEntityEntry<T> entry in context.ChangeTracker.Entries<T>())
                    {
                        T entity = entry.Entity;
                        entry.State = EntityState.Deleted;
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
