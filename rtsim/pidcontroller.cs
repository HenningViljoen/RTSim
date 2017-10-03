using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rtsim
{
    [Serializable]
    public class pidcontroller
    {
        //Properties
        public double K, I, D;
        public double bias;
        public double integral;
        public double sp, pv, op, err;
        public int direction; // As controldirection
        public double maxop, minop;

        //Methods
        public pidcontroller(double aK, double aI, double aD, double aminop, double amaxop)
        {
	        K = aK;
            I = aI;
            D = aD;
            integral = 0;
            bias = 0;
            sp = 0;
            pv = 0;
            op = 0;
            err = 0;
            direction = global.Direct; // for now
            maxop = amaxop;
            minop = aminop;
        }

        public void calcerr()
        {
            err = direction * (sp - pv);
        }

        public void calcintegral()
        {
            integral += err * global.SampleT;
        }

        public void calcop()
        {
            op = K * (err + 1 / I * integral) + bias;
            if (op > maxop)
	        {
		        op = maxop;
	        }
            if (op < minop)
	        {
		        op = minop;
	        }
        }

        public void init(double asp, double apv, double aop)
        {
            sp = asp;
            pv = apv;
            calcerr();
            calcop();
            bias = aop - op;
            op = aop;
        }

        public double update(double asp, double apv)
        {
            sp = asp;
            pv = apv;
            calcerr();
            calcintegral();
            calcop();
            return op;
        }

    }
}
