using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.EMS;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Contract.EMS;
using ServiceCenter.MES.Service.EMS.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NHibernate;

namespace ServiceCenter.MES.Service.EMS
{
    /// <summary>
    /// 实现设备状态事件管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EquipmentStateEventService : IEquipmentStateEventContract
    {
        /// <summary>
        /// 设备状态事件数据访问读写。
        /// </summary>
        public IEquipmentStateEventDataEngine EquipmentStateEventDataEngine { get; set; }

        /// <summary>
        /// 设备数据访问读写。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine { get; set; }

        /// <summary> 添加设备状态事件 </summary>
        /// <param name="obj">设备状态事件数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentStateEvent obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            IList<EquipmentStateEvent> lsEquipmentStateEvent = new List<EquipmentStateEvent>();
            DateTime now = DateTime.Now;
            string transactionKey = Guid.NewGuid().ToString();      //创建事物主键
            ITransaction transaction = null;                        //事物对象

            try
            {
                //取得设备对象
                Equipment equipment = this.EquipmentDataEngine.Get(obj.EquipmentCode);

                if (equipment == null)
                {
                    result.Code = 2000;
                    result.Message = string.Format("设备({0})不存在！",
                                                    obj.EquipmentCode);

                    return result;
                }

                //设置设备状态
                equipment.ChangeStateName = obj.EquipmentChangeStateName;       //设备状态切换名称
                equipment.StateName = obj.EquipmentToStateName;                 //设备目标状态
                equipment.Editor = obj.Editor;                                  //编辑人
                equipment.EditTime = now;                                       //编辑时间

                //取得设备最后状态对象
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = true,
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("EquipmentCode = '{0}'",
                                           obj.EquipmentCode),
                    OrderBy = "CreateTime DESC"
                };

                lsEquipmentStateEvent = this.EquipmentStateEventDataEngine.Get(cfg);

                if (lsEquipmentStateEvent != null && lsEquipmentStateEvent.Count > 0)
                {
                    lsEquipmentStateEvent[0].IsCurrent = false;                 //当前事物标识
                    lsEquipmentStateEvent[0].EndEventKey = transactionKey;      //结束事件事物主键
                    lsEquipmentStateEvent[0].EndTime = now;                     //事件结束时间
                    lsEquipmentStateEvent[0].EditTime = now;                    //编辑时间              
                    lsEquipmentStateEvent[0].Editor = obj.Editor;               //编辑人
                }

                //设置设备事件对象属性
                obj.Key = transactionKey;                                       //设备事件主键
                obj.IsCurrent = true;                                           //当前事物标识
                obj.EditTime = now;                                             //编辑日期

                //开始事务处理
                ISession session = this.EquipmentStateEventDataEngine.SessionFactory.OpenSession();
                transaction = session.BeginTransaction();

                this.EquipmentDataEngine.Update(equipment);                             //设备对象更新

                foreach (EquipmentStateEvent preEquipmentStateEvent in lsEquipmentStateEvent)
                {
                    this.EquipmentStateEventDataEngine.Update(preEquipmentStateEvent);  //历史设备状态事件更新
                }
                
                this.EquipmentStateEventDataEngine.Insert(obj);                         //新增设备状态事件

                transaction.Commit();
                session.Close();

            }catch(Exception err)
            {
                result.Code = 1005;
                result.Message = err.Message;
            }
            return result;

            #region 20160804备份
            //MethodReturnResult result = new MethodReturnResult();
            //if (this.EquipmentStateEventDataEngine.IsExists(obj.Key))
            //{
            //    result.Code = 1001;
            //    result.Message = String.Format(StringResource.EquipmentStateEventService_IsExists, obj.Key);
            //    return result;
            //}

            //#region //注释原来的代码
            ///*             
            //ITransaction transaction = null;           
            //ISession db =null;
            //try
            //{
            //    transaction = null;
            //    db = this.EquipmentStateEventDataEngine.SessionFactory.OpenSession();
            //    transaction = db.BeginTransaction();

            //    this.EquipmentStateEventDataEngine.Insert(obj, db);

            //    Equipment e = this.EquipmentDataEngine.Get(obj.EquipmentCode, db);
            //    if (e != null)
            //    {
            //        Equipment eUpdate = e.Clone() as Equipment;
            //        eUpdate.ChangeStateName = obj.EquipmentChangeStateName;
            //        eUpdate.StateName = obj.EquipmentToStateName;
            //        eUpdate.Editor = obj.Editor;
            //        eUpdate.EditTime = obj.EditTime;
            //        this.EquipmentDataEngine.Update(eUpdate, db);

            //        if (e.Type == EnumEquipmentType.Virtual && !string.IsNullOrEmpty(e.RealEquipmentCode) && e.RealEquipmentCode!=obj.EquipmentCode)
            //        {
            //            Equipment eReal = this.EquipmentDataEngine.Get(e.RealEquipmentCode, db);
            //            if (eReal != null)
            //            {
            //                Equipment eRealUpdate = eReal.Clone() as Equipment;
            //                eRealUpdate.ChangeStateName = obj.EquipmentChangeStateName;
            //                eRealUpdate.StateName = obj.EquipmentToStateName;
            //                eRealUpdate.Editor = obj.Editor;
            //                eRealUpdate.EditTime = obj.EditTime;
            //                this.EquipmentDataEngine.Update(eRealUpdate, db);
            //            }
            //        }
            //    }
            //    transaction.Commit();
            //    db.Close();
            //}
            //catch (Exception ex)
            //{
            //    transaction.Rollback();
            //    db.Close();
            //    result.Code = 1000;
            //    result.Message = String.Format(StringResource.OtherError, ex.Message);
            //}*/
            //#endregion

            //try
            //{
            //    result = ExecuteAddEquipmentStateEvent(obj, null, false);

            //}
            //catch (Exception err)
            //{
            //    result.Code = 1005;
            //    result.Message = err.Message;
            //}
            //return result;
            #endregion
        }
            
        public MethodReturnResult ExecuteAddEquipmentStateEvent(EquipmentStateEvent p,ISession db, bool executedWithTransaction)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"IsCurrent='1' AND EquipmentCode='{0}'
                       AND EquipmentToStateName='{1}'", p.EquipmentCode, p.EquipmentFromStateName),
                OrderBy = "CreateTime Desc "
            };
            List<Equipment> lstEquipmentForUpdate = new List<Equipment>();
            List<EquipmentStateEvent> lstEquipmentEventForUpdate = new List<EquipmentStateEvent>();

            IList<EquipmentStateEvent> lstEquipmentEvent= this.EquipmentStateEventDataEngine.Get(cfg, db);
            EquipmentStateEvent equipmentStateEventForUpdate = null;
            if(lstEquipmentEvent!=null && lstEquipmentEvent.Count>0)
            {
                equipmentStateEventForUpdate = lstEquipmentEvent[0];
                equipmentStateEventForUpdate.EndEventKey = p.Key;
                equipmentStateEventForUpdate.EndTime = p.CreateTime;
                equipmentStateEventForUpdate.IsCurrent = false;
                lstEquipmentEventForUpdate.Add(equipmentStateEventForUpdate);
            }

            Equipment e = this.EquipmentDataEngine.Get(p.EquipmentCode, db);
            if (e != null)
            {
                Equipment eUpdate = e.Clone() as Equipment;
                eUpdate.ChangeStateName = p.EquipmentChangeStateName;
                eUpdate.StateName = p.EquipmentToStateName;
                eUpdate.Editor = p.Editor;
                eUpdate.EditTime = p.EditTime;
                lstEquipmentForUpdate.Add(eUpdate);
                //this.EquipmentDataEngine.Update(eUpdate, db);
                if (e.Type == EnumEquipmentType.Virtual && !string.IsNullOrEmpty(e.RealEquipmentCode) && e.RealEquipmentCode != p.EquipmentCode)
                {
                    Equipment eReal = this.EquipmentDataEngine.Get(e.RealEquipmentCode, db);
                    if (eReal != null)
                    {
                        Equipment eRealUpdate = eReal.Clone() as Equipment;
                        eRealUpdate.ChangeStateName = p.EquipmentChangeStateName;
                        eRealUpdate.StateName = p.EquipmentToStateName;
                        eRealUpdate.Editor = p.Editor;
                        eRealUpdate.EditTime = p.EditTime;
                        lstEquipmentForUpdate.Add(eRealUpdate);
                        //this.EquipmentDataEngine.Update(eRealUpdate, db);
                    }
                }
            }

            ITransaction transaction = null;           
            try
            {
                #region //开始事物处理
                if (executedWithTransaction == false)
                {
                    db = this.EquipmentStateEventDataEngine.SessionFactory.OpenSession();
                    transaction = db.BeginTransaction();
                }
                
                foreach(Equipment obj in lstEquipmentForUpdate )
                {
                    this.EquipmentDataEngine.Update(obj, db);
                }

                foreach (EquipmentStateEvent obj in lstEquipmentEventForUpdate)
                {
                    this.EquipmentStateEventDataEngine.Update(obj, db);
                }

                p.StartTime = p.CreateTime;
                p.IsCurrent = true;
                this.EquipmentStateEventDataEngine.Insert(p, db);

                if (executedWithTransaction == false)
                {
                    transaction.Commit();
                    db.Close();
                }
                else
                {
                    db.Flush();
                }
                #endregion 
            }
            catch (Exception ex)
            {
                if (executedWithTransaction == false)
                {
                    transaction.Rollback();
                    db.Close();
                }
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary> 修改设备状态事件 </summary>
        /// <param name="obj">设备状态事件数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(EquipmentStateEvent obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentStateEventDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentStateEventService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentStateEventDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary> 删除设备状态事件 </summary>
        /// <param name="key">设备状态事件标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentStateEventDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentStateEventService_IsNotExists, key);
                return result;
            }
            try
            {
                this.EquipmentStateEventDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary> 获取设备状态事件数据 </summary>
        /// <param name="key">设备状态事件标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentStateEvent&gt;" />,设备状态事件数据.</returns>
        public MethodReturnResult<EquipmentStateEvent> Get(string key)
        {
            MethodReturnResult<EquipmentStateEvent> result = new MethodReturnResult<EquipmentStateEvent>();
            if (!this.EquipmentStateEventDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentStateEventService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EquipmentStateEventDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary> 获取设备状态事件数据集合 </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentStateEvent&gt;" />,设备状态事件数据集合。</returns>
        public MethodReturnResult<IList<EquipmentStateEvent>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<EquipmentStateEvent>> result = new MethodReturnResult<IList<EquipmentStateEvent>>();
            try
            {
                result.Data = this.EquipmentStateEventDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
    }
}
