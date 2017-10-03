using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rtsim
{
    public class mycar : car   //This class will be the racing car that will race against the other cars.
    {


        public mycar(double aspeed, double adt, point alocation,
                    point adestination, road aroadon, int adestination01,
                    double abreakacceleration)
            : base(aspeed, adt, alocation,
                    adestination, aroadon, adestination01,
                    abreakacceleration)
        {
            carcolour = global.MyCarColour;
            cartype = cartypes.MyCar;
        }




    }
}
