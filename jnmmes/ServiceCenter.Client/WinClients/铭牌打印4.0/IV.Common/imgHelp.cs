using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;

namespace IV.Common
{
  public  class imgHelp
    {
      public byte[] SetImage(SqlDataReader reader)
      {
          return (byte[])reader["Picture"];//Image为数据库中存放Image类型字段
      }
      public Image SetByteToImage(byte[] mybyte)
      {
          Image image;
          MemoryStream mymemorystream = new MemoryStream(mybyte, 0, mybyte.Length);
          image = Image.FromStream(mymemorystream);
          return image;
      }
    

    }
}
