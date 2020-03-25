//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;


//namespace AVEVA.CDMS.WebApi
//{
//    public class CDefWFBase
//    {
//        /// <summary>
//        /// 容器中的某个点,是否在当前图元的热点附近, 用于画分支时做起点和终点判断
//        /// </summary>
//        /// <param name="point"></param>
//        /// <returns></returns>
//        public virtual Point IsPointNearHotPoint(Point point)
//        {
//            try
//            {
//                int xOffSet = this.defWorkFlowCtrl.WorkFlowSplitContainer.Panel1.HorizontalScroll.Value;
//                int yOffSet = this.defWorkFlowCtrl.WorkFlowSplitContainer.Panel1.VerticalScroll.Value;
//                //每个图形的热点在于四条边的中心
//                Point hotPl = new Point(this.Location.X + xOffSet, this.Location.Y + this.Height / 2 + yOffSet);
//                Point hotPt = new Point(this.Location.X + this.Width / 2 + xOffSet, this.Location.Y + yOffSet);
//                Point hotPb = new Point(this.Location.X + this.Width / 2 + xOffSet, this.Location.Y + this.Height + yOffSet);
//                Point hotPr = new Point(this.Location.X + this.Width + xOffSet, this.Location.Y + this.Height / 2 + yOffSet);

//                int offset = 20;

//                Size offSize = new Size(2 * offset, 2 * offset);

//                Rectangle rectL = new Rectangle(new Point(hotPl.X - offset, hotPl.Y - offset), offSize);
//                Rectangle rectT = new Rectangle(new Point(hotPt.X - offset, hotPt.Y - offset), offSize);
//                Rectangle rectB = new Rectangle(new Point(hotPb.X - offset, hotPb.Y - offset), offSize);
//                Rectangle rectR = new Rectangle(new Point(hotPr.X - offset, hotPr.Y - offset), offSize);

//                if (rectB.Contains(point))
//                    return hotPb;

//                else if (rectL.Contains(point))
//                    return hotPl;

//                else if (rectR.Contains(point))
//                    return hotPr;

//                else if (rectT.Contains(point))
//                    return hotPt;
//                else
//                    return new Point(0, 0);

//            }
//            catch (Exception e)
//            {

//            }

//            return new Point(0, 0);
//        }
//    }
//}
