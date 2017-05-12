using Orkidea.Framework.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Deloitte.Approval.DAL
{
    public static class DbMngmt<T> where T : class
    {
        public static IList<T> GetList()
        {
            using (var context = new dltApprovalEntities())
            {
                DataEF<T> dataEF = new DataEF<T>(context);
                return dataEF.GetList();
            }
        }

        public static IList<T> GetList(Expression<Func<T, bool>> where)
        {
            using (DbContext context = new dltApprovalEntities())
            {
                DataEF<T> dataEF = new DataEF<T>(context);
                return dataEF.GetList(where);
            }
        }

        public static T GetSingle(Expression<Func<T, bool>> where)
        {
            using (DbContext context = new dltApprovalEntities())
            {
                DataEF<T> dataEF = new DataEF<T>(context);
                return dataEF.GetSingle(where);
            }
        }

        public static void Add(T item)
        {
            using (var context = new dltApprovalEntities())
            {
                DataEF<T> dataEF = new DataEF<T>(context);
                dataEF.Add(item);
            }
        }

        public static void Add(T[] item)
        {
            using (var context = new dltApprovalEntities())
            {
                DataEF<T> dataEF = new DataEF<T>(context);
                dataEF.Add(item);
            }
        }

        public static void Update(T item)
        {
            using (var context = new dltApprovalEntities())
            {
                DataEF<T> dataEF = new DataEF<T>(context);
                dataEF.Update(item);
            }
        }

        public static void Update(T[] item)
        {
            using (var context = new dltApprovalEntities())
            {
                DataEF<T> dataEF = new DataEF<T>(context);
                dataEF.Update(item);
            }
        }

        public static void Remove(T item)
        {
            using (var context = new dltApprovalEntities())
            {
                DataEF<T> dataEF = new DataEF<T>(context);
                dataEF.Remove(item);
            }
        }

        public static void Remove(T[] item)
        {
            using (var context = new dltApprovalEntities())
            {
                DataEF<T> dataEF = new DataEF<T>(context);
                dataEF.Remove(item);
            }
        }
    }
}
