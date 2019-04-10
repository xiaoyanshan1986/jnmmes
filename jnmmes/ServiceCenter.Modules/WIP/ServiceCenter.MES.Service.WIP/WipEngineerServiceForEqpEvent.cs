using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.RBAC;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System.ServiceModel.Activation;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.EMS;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.BaseData;
using System.Data;

namespace ServiceCenter.MES.Service.WIP
{
    public partial class WipEngineerService
    {

//        public MethodReturnResult ExecuteAddEquipmentStateEvent(EquipmentStateEvent p, ISession db, bool executedWithTransaction)
//        {
//            DateTime now = DateTime.Now;
//            MethodReturnResult result = new MethodReturnResult()
//            {
//                Code = 0
//            };

//            PagingConfig cfg = new PagingConfig()
//            {
//                IsPaging = false,
//                Where = string.Format(@"IsCurrent='1' AND EquipmentCode='{0}'
//                       AND EquipmentToStateName='{1}'", p.EquipmentCode, p.EquipmentFromStateName),
//                OrderBy = "CreateTime Desc "
//            };
//            List<Equipment> lstEquipmentForUpdate = new List<Equipment>();
//            List<EquipmentStateEvent> lstEquipmentEventForUpdate = new List<EquipmentStateEvent>();

//            IList<EquipmentStateEvent> lstEquipmentEvent = this.EquipmentStateEventDataEngine.Get(cfg, db);
//            EquipmentStateEvent equipmentStateEventForUpdate = null;
//            if (lstEquipmentEvent != null && lstEquipmentEvent.Count > 0)
//            {
//                equipmentStateEventForUpdate = lstEquipmentEvent[0];
//                equipmentStateEventForUpdate.EndEventKey = p.Key;
//                equipmentStateEventForUpdate.EndTime = p.CreateTime;
//                equipmentStateEventForUpdate.IsCurrent = false;
//                lstEquipmentEventForUpdate.Add(equipmentStateEventForUpdate);
//            }
//            /*
//            Equipment e = this.EquipmentDataEngine.Get(p.EquipmentCode, db);
//            if (e != null)
//            {
//                Equipment eUpdate = e.Clone() as Equipment;
//                eUpdate.ChangeStateName = p.EquipmentChangeStateName;
//                eUpdate.StateName = p.EquipmentToStateName;
//                eUpdate.Editor = p.Editor;
//                eUpdate.EditTime = p.EditTime;
//                lstEquipmentForUpdate.Add(eUpdate);
//                //this.EquipmentDataEngine.Update(eUpdate, db);
//                if (e.Type == EnumEquipmentType.Virtual && !string.IsNullOrEmpty(e.RealEquipmentCode) && e.RealEquipmentCode != p.EquipmentCode)
//                {
//                    Equipment eReal = this.EquipmentDataEngine.Get(e.RealEquipmentCode, db);
//                    if (eReal != null)
//                    {
//                        Equipment eRealUpdate = eReal.Clone() as Equipment;
//                        eRealUpdate.ChangeStateName = p.EquipmentChangeStateName;
//                        eRealUpdate.StateName = p.EquipmentToStateName;
//                        eRealUpdate.Editor = p.Editor;
//                        eRealUpdate.EditTime = p.EditTime;
//                        lstEquipmentForUpdate.Add(eRealUpdate);
//                        //this.EquipmentDataEngine.Update(eRealUpdate, db);
//                    }
//                }
//            }*/

//            ITransaction transaction = null;
//            try
//            {
//                #region //开始事物处理
//                if (executedWithTransaction == false)
//                {
//                    db = this.EquipmentStateEventDataEngine.SessionFactory.OpenSession();
//                    transaction = db.BeginTransaction();
//                }

//                foreach (Equipment obj in lstEquipmentForUpdate)
//                {
//                    this.EquipmentDataEngine.Update(obj, db);
//                }

//                foreach (EquipmentStateEvent obj in lstEquipmentEventForUpdate)
//                {
//                    this.EquipmentStateEventDataEngine.Update(obj, db);
//                }

//                p.StartTime = p.CreateTime;
//                p.IsCurrent = true;
//                this.EquipmentStateEventDataEngine.Insert(p, db);

//                if (executedWithTransaction == false)
//                {
//                    transaction.Commit();
//                    db.Close();
//                }
//                else
//                {
//                    db.Flush();
//                }
//                #endregion
//            }
//            catch (Exception ex)
//            {
//                if (executedWithTransaction == false)
//                {
//                    transaction.Rollback();
//                    db.Close();
//                }
//                result.Code = 1000;
//                result.Message = String.Format(StringResource.Error, ex.Message);
//            }
//            return result;
//        }

    }

}
