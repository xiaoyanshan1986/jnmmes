using ServiceCenter.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceCenter.MES.Model.PPM;
using System.Threading.Tasks;

namespace ServiceCenter.MES.DataAccess.Interface.PPM
{
    //目标参数管理接口
    public interface ITargetParameterDataEngine : IDatabaseDataEngine<TargetParameter, TargetParameterKey>
    {
    }
}
