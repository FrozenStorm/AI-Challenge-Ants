using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;

namespace Ants
{
    public class Knoten
    {
        int now_x;
        public int Now_x
        {
            set
            {
                now_x = value;
            }
            get
            {
                return now_x;
            }
        }
        int now_y;
        public int Now_y
        {
            set
            {
                now_y = value;
            }
            get
            {
                return now_y;
            }
        }
        int from_z;
        public int From_z
        {
            set
            {
                from_z = value;
            }
            get
            {
                return from_z;
            }
        }
        int distanzToTarget;
        public int DistanzToTarget
        {
            set
            {
                distanzToTarget = value;
            }
            get
            {
                return distanzToTarget;
            }
        }
        public Knoten(int from_z, int now_x, int now_y, int distanzToTarget)
        {
            this.distanzToTarget = distanzToTarget;
            this.now_x = now_x;
            this.now_y = now_y;
            this.from_z = from_z;
        }
    }
}
