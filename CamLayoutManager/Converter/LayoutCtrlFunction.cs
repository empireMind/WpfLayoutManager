using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamLayoutManager.Converter
{
    class LayoutCtrlFunction
    {
        /// <summary>
        /// 根据当前队列容量改变布局
        /// </summary>
        /// <param name="count">当前队列容量</param>
        /// <param name="worh">标志位 0代表横向 1代表纵向</param>
        /// <returns>(横向/纵向)布局</returns>
        public static int CountFactor(int count, int worh)
        {
            int factor = 1;
            if (count <= 1)
                factor = 1;                 //1x1布局
            //else if (count <= 2)
            //    factor = 2;
            //    //factor = worh == 0 ? 2 : 1; //2x1布局          
            else if (count <= 4)
                factor = 2;                 //2x2布局
            else if (count <= 9)
                factor = 3;                 //3x3布局
            return factor;
        }
    }
}
