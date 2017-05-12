using Orkidea.Deloitte.Approval.DAL;
using Orkidea.Deloitte.Approval.Entities;
using Orkidea.Framework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkidea.Deloitte.Approval.Business
{
    public static class BizDistributionList
    {
        public static DistributionList GetSingle(string name)
        {
            string distributionListName = HexSerialization.StringToHex(Cryptography.Encrypt(name));
            return DbMngmt<DistributionList>.GetSingle(c => c.name.Equals(distributionListName));
        }

        public static DistributionList GetSingle(int id)
        {
            DistributionList distributionList = DbMngmt<DistributionList>.GetSingle(c => c.id.Equals(id));
            distributionList.name = Cryptography.Decrypt(HexSerialization.HexToString(distributionList.name));
            distributionList.email = Cryptography.Decrypt(HexSerialization.HexToString(distributionList.email));

            return distributionList;
        }

        public static IList<DistributionList> GetList()
        {
            List<DistributionList> distributionLists = new List<DistributionList>(); 
            
            distributionLists.AddRange(DbMngmt<DistributionList>.GetList().ToList());

            foreach (DistributionList distributionList in distributionLists)
            {
                distributionList.name = Cryptography.Decrypt(HexSerialization.HexToString(distributionList.name));

                string uglyList = Cryptography.Decrypt(HexSerialization.HexToString(distributionList.email));                        

                distributionList.email = uglyList.Replace(",", "<br/>");
            }

            return distributionLists;
        }

        public static void Add(DistributionList distributionList)
        {
            try
            {
                distributionList.name = HexSerialization.StringToHex(Cryptography.Encrypt(distributionList.name));
                distributionList.email = HexSerialization.StringToHex(Cryptography.Encrypt(distributionList.email));

                DbMngmt<DistributionList>.Add(distributionList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Update(DistributionList distributionList)
        {
            try
            {
                distributionList.name = HexSerialization.StringToHex(Cryptography.Encrypt(distributionList.name));
                distributionList.email = HexSerialization.StringToHex(Cryptography.Encrypt(distributionList.email));

                DbMngmt<DistributionList>.Update(distributionList);                
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Remove(DistributionList distributionList)
        {
            distributionList = GetSingle(distributionList.id);

            DbMngmt<DistributionList>.Remove(distributionList);
        }
    }
}
