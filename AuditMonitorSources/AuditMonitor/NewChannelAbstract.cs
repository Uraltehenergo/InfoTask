using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AuditMonitor
{
    public abstract class NewChannelAbstract
    {
    #region Static
        public static double LinearTransform(double value, double oldMin, double oldMax, double newMin, double newMax)
        {
            if (oldMin != oldMax)
                return (newMax - newMin) / (oldMax - oldMin) * (value - oldMin) + newMin;
            return newMin;
        }

        public static double? MVtoThL(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] am = {
                              -5.8952244e-5, 
                              6.3391502e-2,  
                              6.7592964e-5, 
                              2.0672566e-7, 
                              5.5720884e-9, 
                              5.713386e-11,
                              3.2995593e-13, 
                              9.923242e-16, 
                              1.2079584e-18
                          };

            double[] a = {
                             -1.8656953e-5, 
                             6.3310975e-2, 
                             6.0153091e-5,
                             -8.0073134e-8, 
                             9.6946071e-11,
                             -3.6047289e-14,
                             -2.4694775e-16,
                             4.2880341e-19,
                             -2.0725297e-22
                         };

            double[] cm = {
                              1.1573067e-4,
                              1.5884573e1, 
                              4.0458544e-2, 
                              0.3170064, 
                              0.1666128, 
                              5.1946958e-2, 
                              9.5288883e-3,
                              1.0301283e-3, 
                              6.0654431e-5, 
                              1.5131878e-6
                          };

            double[] c = {
                             7.2069422e-3,
                             1.5775525e1,
                             -0.2261183,
                             9.4286756e-3,
                             -3.5394655e-4,
                             1.0050886e-5,
                             -1.9323678e-7,
                             2.3816891e-9,
                             -1.7130654e-11,
                             5.4857331e-14
                         };

            double mvCjc;
            if ((cjc >= 0) && (cjc <= 800))
            {
                mvCjc = a[0];
                for (int i = 1; i <= a.GetUpperBound(0); i++) mvCjc += a[i] * (Math.Pow(cjc, i));
            }
            else if ((cjc <= 0) && (cjc >= -200))
            {
                mvCjc = am[0];
                for (int i = 1; i <= am.GetUpperBound(0); i++) mvCjc += am[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= 0) && (mvVal <= 66.466))
            {
                val = c[0];
                for (int i = 1; i <= c.GetUpperBound(0); i++) val += c[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal <= 0) && (mvVal >= -9.488))
            {
                val = cm[0];
                for (int i = 1; i <= cm.GetUpperBound(0); i++) val += cm[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThK(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] am = {
                              0,
                              3.9450128025e-2,
                              2.3622373598e-5,
                              -3.2858906784e-7,
                              -4.9904828777e-9,
                              -6.7509059173e-11,
                              -5.7410327428e-13,
                              -3.1088872894e-15,
                              -1.0451609365e-17,
                              -1.9889266878e-20,
                              -1.6322697486e-23
                          };

            double[] a = {
                             -1.7600413686e-2,
                             3.8921204975e-2,
                             1.8558770032e-5,
                             -9.9457592874e-8,
                             3.1840945719e-10,
                             -5.6072844889e-13,
                             5.6075059059e-16,
                             -3.2020720003e-19,
                             9.7151147152e-23,
                             -1.2104721275e-26
                       };

            double[] n = {
                             1.185976e-1,
                             -1.183432e-4,
                             126.9686
                         };

            double[] cm = {
                              0,
                              2.5173462e1,
                              -1.1662878,
                              -1.0833638,
                              -8.977354e-1,
                              -3.7342377e-1,
                              -8.6632643e-2,
                              -1.0450598e-2,
                              -5.1920577e-4
                          };

            double[] c = {
                             0,
                             2.508355e1,
                             7.860106e-2,
                             -2.503131e-1,
                             8.31527e-2,
                             -1.228034e-2,
                             9.804036e-4,
                             -4.41303e-5,
                             1.057734e-6,
                             -1.052755e-8
                         };

            double[] cn = {
                              -1.318058e2,
                              4.830222e1,
                              -1.646031,
                              5.464731e-2,
                              -9.650715e-4,
                              8.802193e-6,
                              -3.11081e-8
                          };

            double mvCjc;
            if ((cjc >= 0) && (cjc <= 1372))
            {
                mvCjc = a[0];
                for (int i = 1; i <= a.GetUpperBound(0); i++) mvCjc += a[i] * (Math.Pow(cjc, i));
                mvCjc = n[0] * Math.Exp(n[1] * Math.Pow(cjc - n[2], 2));
            }
            else if ((cjc <= 0) && (cjc >= -270))
            {
                mvCjc = am[0];
                for (int i = 1; i <= am.GetUpperBound(0); i++) mvCjc += am[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= 0) && (mvVal <= 20.644))
            {
                val = c[0];
                //for (int i = 1; i <= 9; i++) val += c[i]*(Math.Pow(mvVal, i));
                for (int i = 1; i <= c.GetUpperBound(0); i++) val += c[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal <= 0) && (mvVal >= -5.891))
            {
                val = cm[0];
                //for (int i = 1; i <= 8; i++) val += cm[i]*(Math.Pow(mvVal, i));
                for (int i = 1; i <= cm.GetUpperBound(0); i++) val += cm[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 20.644) && (mvVal <= 54.886))
            {
                val = cn[0];
                //for (int i = 1; i <= 6; i++) val += cn[i]*(Math.Pow(mvVal, i));
                for (int i = 1; i <= cn.GetUpperBound(0); i++) val += cn[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThR(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] a1 = {
                              0,
                              5.28961729765e-3,
                              1.39166589782e-5,
                              -2.38855693017e-8,
                              3.56916001063e-11,
                              -4.62347666298e-14,
                              5.00777441034e-17,
                              -3.73105886191e-20,
                              1.57716482367e-23,
                              -2.81038625251e-27
                          };

            double[] a2 = {
                              2.95157925316,
                              -2.52061251332e-3,
                              1.59564501865e-5,
                              -7.64085947576e-9,
                              2.05305291024e-12,
                              -2.93359668173e-16
                          };

            double[] a3 = {
                              1.52232118209e2,
                              -2.68819888545e-1,
                              1.71280280471e-4
                              - 3.45895706453e-8
                              - 9.34633971046e-15
                          };


            double[] c1 = {
                              0,
                              1.8891380e2,
                              -9.3835290e1,
                              1.3068619e2,
                              -2.2703580e2,
                              3.5145659e2,
                              -3.8953900e2,
                              2.8239471e2,
                              -1.2607281e2,
                              3.1353611e1,
                              -3.3187769
                          };

            double[] c2 = {
                              1.334584505e1,
                              1.472644573e2,
                              -1.844024844e1,
                              4.031129726,
                              -6.249428360e-1,
                              6.468412046e-2,
                              -4.458750426e-3,
                              1.994710149e-4,
                              -5.313401790e-6,
                              6.481976217e-8
                          };

            double[] c3 = {
                              -8.199599416e1,
                              1.553962042e2,
                              -8.342197663,
                              4.279433549e-1,
                              -1.191577910e-2,
                              1.492290091e-4
                          };

            double[] c4 = {
                              3.406177836e4,
                              -7.023729171e3,
                              5.582903813e2,
                              -1.952394635e1,
                              2.560740231e-1
                          };

            double mvCjc;
            if ((cjc >= -50) && (cjc <= 1064.18))
            {
                mvCjc = a1[0];
                for (int i = 1; i <= a1.GetUpperBound(0); i++) mvCjc += a1[i] * (Math.Pow(cjc, i));
            }
            else if ((cjc >= 1064.18) && (cjc <= 1664.5))
            {
                mvCjc = a2[0];
                for (int i = 1; i <= a2.GetUpperBound(0); i++) mvCjc += a2[i] * (Math.Pow(cjc, i));
            }
            else if ((cjc >= 1664.5) && (cjc <= 1768.1))
            {
                mvCjc = a3[0];
                for (int i = 1; i <= a3.GetUpperBound(0); i++) mvCjc += a3[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= -0.226) && (mvVal <= 1.923))
            {
                val = c1[0];
                for (int i = 1; i <= c1.GetUpperBound(0); i++) val += c1[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 1.923) && (mvVal <= 11.361))
            {
                val = c2[0];
                for (int i = 1; i <= c2.GetUpperBound(0); i++) val += c2[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 11.361) && (mvVal <= 19.739))
            {
                val = c3[0];
                for (int i = 1; i <= c3.GetUpperBound(0); i++) val += c3[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 19.73) && (mvVal <= 21.103))
            {
                val = c4[0];
                for (int i = 1; i <= c4.GetUpperBound(0); i++) val += c4[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThS(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] a1 = {
                              0,
                              5.40313308631e-3,
                              1.25934289740e-5,
                              -2.32477968689e-8,
                              3.22028823036e-11,
                              -3.31465196389e-14,
                              2.55744251786e-17,
                              -1.25068871393e-20,
                              2.71443176145e-24
                          };

            double[] a2 = {
                              1.32900444085,
                              3.34509311344e-3,
                              6.54805192818e-6,
                              -1.64856259209e-9,
                              1.29989605174e-14
                          };

            double[] a3 = {
                              1.46628232636e2,
                              -2.58430516752e-1,
                              1.63693574641e-4,
                              -3.30439046987e-8,
                              -9.43223690612e-15
                          };


            double[] c1 = {
                              0,
                              1.84949460e2,
                              -8.00504062e1,
                              1.02237430e2,
                              -1.52248592e2,
                              1.88821343e2,
                              -1.59085941e2,
                              8.23027880e1,
                              -2.34181944e1,
                              2.79786260
                          };

            double[] c2 = {
                              1.291507177e1,
                              1.466298863e2,
                              -1.534713402e1,
                              3.145945973,
                              -4.163257839e-1,
                              3.187963771e-2,
                              -1.291637500e-3,
                              2.183475087e-5,
                              -1.447379511e-7,
                              8.211272125e-9

                          };

            double[] c3 = {
                              -8.087801117e1,
                              1.621573104e2,
                              -8.536869453,
                              4.719686976e-1,
                              -1.441693666e-2,
                              2.081618890e-4
                          };

            double[] c4 = {
                              5.333875126e4,
                              -1.235892298e4,
                              1.092657613e3,
                              -4.265693686e1,
                              6.247205420e-1
                          };

            double mvCjc;
            if ((cjc >= -50) && (cjc <= 1064.18))
            {
                mvCjc = a1[0];
                for (int i = 1; i <= a1.GetUpperBound(0); i++) mvCjc += a1[i] * (Math.Pow(cjc, i));
            }
            else if ((cjc >= 1064.18) && (cjc <= 1664.5))
            {
                mvCjc = a2[0];
                for (int i = 1; i <= a2.GetUpperBound(0); i++) mvCjc += a2[i] * (Math.Pow(cjc, i));
            }
            else if ((cjc >= 1664.5) && (cjc <= 1768.1))
            {
                mvCjc = a3[0];
                for (int i = 1; i <= a3.GetUpperBound(0); i++) mvCjc += a3[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= -0.235) && (mvVal <= 1.874))
            {
                val = c1[0];
                for (int i = 1; i <= c1.GetUpperBound(0); i++) val += c1[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 1.874) && (mvVal <= 10.332))
            {
                val = c2[0];
                for (int i = 1; i <= c2.GetUpperBound(0); i++) val += c2[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 10.332) && (mvVal <= 17.536))
            {
                val = c3[0];
                for (int i = 1; i <= c3.GetUpperBound(0); i++) val += c3[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 17.536) && (mvVal <= 18.694))
            {
                val = c4[0];
                for (int i = 1; i <= c4.GetUpperBound(0); i++) val += c4[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThB(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] a1 = {
                              0,
                              -2.4650818346e-4,
                              5.9040421171e-6,
                              -1.3257931636e-9,
                              1.5668291901e-12,
                              -1.6944529240e-15,
                              6.2990347094e-19
                          };

            double[] a2 = {
                              -3.8938168621,
                              2.8571747470e-2,
                              -8.4885104785e-5,
                              1.5785280164e-7,
                              -1.6835344864e-10,
                              1.1109794013e-13,
                              -4.4515431033e-17,
                              9.8975640821e-21,
                              - 9.3791330289e-25
                          };

            double[] c1 = {
                              9.8423321e1,
                              6.9971500e2,
                              -8.4765304e2,
                              1.0052644e3,
                              -8.3345952e2,
                              4.5508542e2,
                              -1.5523037e2,
                              2.9886750e1,
                              -2.4742860
                          };

            double[] c2 = {

                              2.1315071e2,
                              2.8510504e2,
                              -5.2742887e1,
                              9.9160804,
                              -1.2965303,
                              1.1195870e-1,
                              -6.0625199e-3,
                              1.8661696e-4,
                              -2.4878585e-6
                          };

            double mvCjc;
            if ((cjc >= 0) && (cjc <= 630.615))
            {
                mvCjc = a1[0];
                for (int i = 1; i <= a1.GetUpperBound(0); i++) mvCjc += a1[i] * (Math.Pow(cjc, i));
            }
            else if ((cjc >= 630.615) && (cjc <= 1820))
            {
                mvCjc = a2[0];
                for (int i = 1; i <= a2.GetUpperBound(0); i++) mvCjc += a2[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= 0.291) && (mvVal <= 2.431))
            {
                val = c1[0];
                for (int i = 1; i <= c1.GetUpperBound(0); i++) val += c1[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 2.431) && (mvVal <= 13.820))
            {
                val = c2[0];
                for (int i = 1; i <= c2.GetUpperBound(0); i++) val += c2[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThJ(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] a1 = {
                              0,
                              5.0381187815e-2,
                              3.0475836930e-5,
                              -8.5681065720e-8,
                              1.3228195295e-10,
                              -1.7052958337e-13,
                              2.0948090697e-16,
                              -1.2538395336e-19,
                              1.5631725697e-23
                          };

            double[] a2 = {
                              2.9645625681e2,
                              -1.4976127786,
                              3.1787103924e-3,
                              -3.1847686701e-6,
                              1.5720819004e-9,
                              -3.0691369056e-13
                          };

            double[] c1 = {
                              0,
                              1.9528268e1,
                              -1.2286185,
                              -1.0752178,
                              -5.9086933e-1,
                              -1.7256713e-1,
                              -2.8131513e-2,
                              -2.3963370e-3,
                              -8.3823321e-5
                          };

            double[] c2 = {
                              0,
                              1.978425e1,
                              -2.001204e-1,
                              1.036969e-2,
                              -2.549687e-4,
                              3.585153e-6,
                              -5.344285e-8,
                              5.099890e-10
                          };

            double[] c3 = {
                              -3.11358187e3,
                              3.00543684e2,
                              -9.94773230,
                              1.70276630e-1,
                              -1.43033468e-3,
                              4.73886084e-6
                          };

            double mvCjc;
            if ((cjc >= -210) && (cjc <= 760))
            {
                mvCjc = a1[0];
                for (int i = 1; i <= a1.GetUpperBound(0); i++) mvCjc += a1[i] * (Math.Pow(cjc, i));
            }
            else if ((cjc >= 760) && (cjc <= 1200))
            {
                mvCjc = a2[0];
                for (int i = 1; i <= a2.GetUpperBound(0); i++) mvCjc += a2[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= -8.095) && (mvVal <= 0))
            {
                val = c1[0];
                for (int i = 1; i <= c1.GetUpperBound(0); i++) val += c1[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 0) && (mvVal <= 42.919))
            {
                val = c2[0];
                for (int i = 1; i <= c2.GetUpperBound(0); i++) val += c2[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 42.919) && (mvVal <= 69.553))
            {
                val = c3[0];
                for (int i = 1; i <= c3.GetUpperBound(0); i++) val += c3[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThT(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] a1 = {
                              0,
                              3.8748106364e-2,
                              4.4194434347e-5,
                              1.1844323105e-7,
                              2.0032973554e-8,
                              9.0138019559e-10,
                              2.2651156593e-11,
                              3.6071154205e-13,
                              3.8493939883e-15,
                              2.8213521925e-17,
                              1.4251594779e-19,
                              4.8768662286e-22,
                              1.0795539270e-24,
                              1.3945027062e-27,
                              7.9795153927e-31
                          };

            double[] a2 = {
                              0,
                              3.8748106364e-2,
                              3.3292227880e-5,
                              2.0618243404e-7,
                              -2.1882256846e-9,
                              1.0996880928e-11,
                              -3.0815758772e-14,
                              4.5479135290e-17,
                              -2.7512901673e-20
                          };

            double[] c1 = {
                              0,
                              2.5949192e1,
                              -2.1316967e-1,
                              7.9018692e-1,
                              4.2527777e-1,
                              1.3304473e-1,
                              2.0241446e-2,
                              1.2668171e-3
                          };

            double[] c2 = {
                              0,
                              2.592800e1,
                              -7.602961e-1,
                              4.637791e-2,
                              -2.165394e-3,
                              6.048144e-5,
                              -7.293422e-7
                          };

            double mvCjc;
            if ((cjc >= -270) && (cjc <= 0))
            {
                mvCjc = a1[0];
                for (int i = 1; i <= a1.GetUpperBound(0); i++) mvCjc += a1[i] * (Math.Pow(cjc, i));
            }
            else if ((cjc >= 0) && (cjc <= 400))
            {
                mvCjc = a2[0];
                for (int i = 1; i <= a2.GetUpperBound(0); i++) mvCjc += a2[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= -5.603) && (mvVal <= 0))
            {
                val = c1[0];
                for (int i = 1; i <= c1.GetUpperBound(0); i++) val += c1[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 0) && (mvVal <= 20.872))
            {
                val = c2[0];
                for (int i = 1; i <= c2.GetUpperBound(0); i++) val += c2[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThE(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] a1 = {
                              0,
                              5.8665508708e-2,
                              4.5410977124e-5,
                              -7.7998048686e-7,
                              -2.5800160843e-8,
                              -5.9452583057e-10,
                              -9.3214058667e-12,
                              -1.0287605534e-13,
                              -8.0370123621e-16,
                              -4.3979497391e-18,
                              -1.6414776355e-20,
                              -3.9673619516e-23,
                              -5.5827328721e-26,
                              -3.4657842013e-29
                          };

            double[] a2 = {
                              0,
                              5.8665508710e-2,
                              4.5032275582e-5,
                              2.8908407212e-8,
                              -3.3056896652e-10,
                              6.5024403270e-13,
                              -1.9197495504e-16,
                              -1.2536600497e-18,
                              2.1489217569e-21,
                              -1.4388041782e-24,
                              3.5960899481e-28
                          };

            double[] c1 = {
                              0,
                              1.6977288e1,
                              -4.3514970e-1,
                              -1.5859697e-1,
                              -9.2502871e-2,
                              -2.6084314e-2,
                              -4.1360199e-3,
                              -3.4034030e-4,
                              -1.1564890e-5
                          };

            double[] c2 = {
                              0,
                              1.7057035e1,
                              -2.3301759e-1,
                              6.5435585e-3,
                              -7.3562749e-5,
                              -1.7896001e-6,
                              8.4036165e-8,
                              -1.3735879e-9,
                              1.0629823e-11,
                              -3.2447087e-14
                          };

            double mvCjc;
            if ((cjc >= -270) && (cjc <= 0))
            {
                mvCjc = a1[0];
                for (int i = 1; i <= a1.GetUpperBound(0); i++) mvCjc += a1[i] * (Math.Pow(cjc, i));
            }
            else if ((cjc >= 0) && (cjc <= 1000))
            {
                mvCjc = a2[0];
                for (int i = 1; i <= a2.GetUpperBound(0); i++) mvCjc += a2[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= -8.825) && (mvVal <= 0))
            {
                val = c1[0];
                for (int i = 1; i <= c1.GetUpperBound(0); i++) val += c1[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 0) && (mvVal <= 76.373))
            {
                val = c2[0];
                for (int i = 1; i <= c2.GetUpperBound(0); i++) val += c2[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThN(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] a1 = {
                              0,
                              2.6159105962e-2,
                              1.0957484228e-5,
                              -9.3841111554e-8,
                              -4.6412039759e-11,
                              -2.6303357716e-12,
                              -2.2653438003e-14,
                              -7.6089300791e-17,
                              -9.3419667835e-20
                          };

            double[] a2 = {
                              0,
                              2.5929394601e-2,
                              1.5710141880e-5,
                              4.3825627237e-8,
                              -2.5261169794e-10,
                              6.4311819339e-13,
                              -1.0063471519e-15,
                              9.9745338992e-19,
                              -6.0863245607e-22,
                              2.0849229339e-25,
                              -3.0682196151e-29
                          };

            double[] c1 = {
                              0,
                              3.8436847e1,
                              1.1010485,
                              5.2229312,
                              7.2060525,
                              5.8488586,
                              2.7754916,
                              7.7075166e-1,
                              1.1582665e-1,
                              7.3138868e-3
                          };

            double[] c2 = {
                              0,
                              3.86896e1,
                              -1.08267,
                              4.70205e-2,
                              -2.12169e-6,
                              -1.17272e-4,
                              5.39280e-6,
                              -7.98156e-8
                          };

            double[] c3 = {
                              1.972485e1,
                              3.300943e1,
                              -3.915159e-1,
                              9.855391e-3,
                              -1.274371e-4,
                              7.767022e-7
                          };

            double mvCjc;
            if ((cjc >= -270) && (cjc <= 0))
            {
                mvCjc = a1[0];
                for (int i = 1; i <= a1.GetUpperBound(0); i++) mvCjc += a1[i] * (Math.Pow(cjc, i));
            }
            else if ((cjc >= 0) && (cjc <= 1300))
            {
                mvCjc = a2[0];
                for (int i = 1; i <= a2.GetUpperBound(0); i++) mvCjc += a2[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= -3.990) && (mvVal <= 0))
            {
                val = c1[0];
                for (int i = 1; i <= c1.GetUpperBound(0); i++) val += c1[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 0) && (mvVal <= 20.613))
            {
                val = c2[0];
                for (int i = 1; i <= c2.GetUpperBound(0); i++) val += c2[i] * (Math.Pow(mvVal, i));
            }
            else if ((mvVal >= 20.613) && (mvVal <= 47.513))
            {
                val = c3[0];
                for (int i = 1; i <= c3.GetUpperBound(0); i++) val += c3[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThA1(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] a1 = {
                              7.1564735e-4,
                              1.1951905e-2,
                              1.6672625e-5,
                              -2.8287807e-8,
                              2.8397839e-11,
                              -1.8505007e-14,
                              7.3632123e-18,
                              -1.6148878e-21,
                              1.4901679e-25
                          };

            double[] c1 = {
                              0.9643027,
                              7.9495086e1,
                              -4.9990310,
                              0.6341776,
                              -4.7440967e-2,
                              2.1811337e-3,
                              -5.8324228e-5,
                              8.2433725e-7,
                              -4.5928480e-9
                          };

            double mvCjc;
            if ((cjc >= 0) && (cjc <= 2500))
            {
                mvCjc = a1[0];
                for (int i = 1; i <= a1.GetUpperBound(0); i++) mvCjc += a1[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= 0) && (mvVal <= 33.640))
            {
                val = c1[0];
                for (int i = 1; i <= c1.GetUpperBound(0); i++) val += c1[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThA2(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] a1 = {
                              -1.0850558e-4,
                              1.1642292e-2,
                              2.1280289e-5,
                              -4.4258402e-8,
                              5.5652058e-11,
                              -4.3801310e-14,
                              2.0228390e-17,
                              -4.9354041e-21,
                              4.8119846e-25
                          };

            double[] c1 = {
                              1.1196428,
                              8.0569397e1,
                              -6.2279122,
                              0.9337015,
                              -8.2608051e-2,
                              4.4110979e-3,
                              -1.3610551e-4,
                              2.2183851e-6,
                              -1.4527698e-8
                          };

            double mvCjc;
            if ((cjc >= 0) && (cjc <= 1800))
            {
                mvCjc = a1[0];
                for (int i = 1; i <= a1.GetUpperBound(0); i++) mvCjc += a1[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= 0) && (mvVal <= 27.232))
            {
                val = c1[0];
                for (int i = 1; i <= c1.GetUpperBound(0); i++) val += c1[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThA3(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] a1 = {
                              -1.0649133e-4,
                              1.1686475e-2,
                              1.8022157e-5,
                              -3.3436998e-8,
                              3.7081688e-11,
                              -2.5748444e-14,
                              1.0301893e-17,
                              -2.0735944e-21,
                              1.4678450e-25
                          };

            double[] c1 = {
                              0.8769216,
                              8.1483231e1,
                              -5.9344173,
                              0.8699340,
                              -7.6797687e-2,
                              4.1814387e-3,
                              -1.3439670e-4,
                              2.342409e-6,
                              -1.6988727e-8
                          };

            double mvCjc;
            if ((cjc >= 0) && (cjc <= 1800))
            {
                mvCjc = a1[0];
                for (int i = 1; i <= a1.GetUpperBound(0); i++) mvCjc += a1[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= 0) && (mvVal <= 26.773))
            {
                val = c1[0];
                for (int i = 1; i <= c1.GetUpperBound(0); i++) val += c1[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoThM(double value, double cjc, out NewEnumSignalStatus status)
        {
            double[] a1 = {
                              2.4455560e-6,
                              4.2638917e-2,
                              5.0348392e-5,
                              -4.4974485e-8
                          };

            double[] c1 = {
                              0.4548090,
                              2.2657698e-2,
                              -7.7935652e-7,
                              1.1786931e-10
                          };

            double mvCjc;
            if ((cjc >= -200) && (cjc <= 100))
            {
                mvCjc = a1[0];
                for (int i = 1; i <= a1.GetUpperBound(0); i++) mvCjc += a1[i] * (Math.Pow(cjc, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformCjcOutOfRange;
                return null;
            }

            double mvVal = value + mvCjc;
            double val;
            if ((mvVal >= -6.154) && (mvVal <= 4.722))
            {
                val = c1[0];
                for (int i = 1; i <= c1.GetUpperBound(0); i++) val += c1[i] * (Math.Pow(mvVal, i));
            }
            else
            {
                status = NewEnumSignalStatus.TransformOutOfRange;
                return null;
            }

            status = NewEnumSignalStatus.NoError;
            return val;
        }

        public static double? MVtoTh(string thType, double value, double cjc, out NewEnumSignalStatus status)
        {
            switch (thType)
            {
                case "L": return MVtoThL(value, cjc, out status);
                case "K": return MVtoThK(value, cjc, out status);
                case "R": return MVtoThR(value, cjc, out status);
                case "S": return MVtoThS(value, cjc, out status);
                case "B": return MVtoThB(value, cjc, out status);
                case "J": return MVtoThJ(value, cjc, out status);
                case "T": return MVtoThT(value, cjc, out status);
                case "E": return MVtoThE(value, cjc, out status);
                case "N": return MVtoThN(value, cjc, out status);
                case "A1": return MVtoThA1(value, cjc, out status);
                case "A2": return MVtoThA2(value, cjc, out status);
                case "A3": return MVtoThA3(value, cjc, out status);
                case "M": return MVtoThM(value, cjc, out status);
                default:
                    status = NewEnumSignalStatus.TransformError;
                    return null;
            }
        }
    #endregion

    #region Fields/AutoProperties
        //свойства канал
        public abstract string ChannelType { get; }                     //тип канала
        private readonly byte _channel;                                 //канал
        public byte Channel { get { return _channel; } }

        //свойства модуля
        public readonly NewModuleAbstract Module;                       //модуль
       
        //свойства, задаваемые пользователем
        private string _code;                                           //код сигнала (например KKS) //изменить реализацию (не дать заполнить уже существующим)
        public string Name { get; set; }                                //имя сигнала
        public virtual double? Min { get; set; }                        //минимальное значения (используется для обработки)
        public virtual double? Max { get; set; }                        //максимальное значения (используется для обработки)
        public virtual string Units { get; set; }                       //единицы измерения (справочная информация)
        public virtual double? Aperture { get; set; }                   //апертура
        public virtual string InLevel { get; set; }                     //тип значения (определяет первичную обработку)
        private string _conversion;                                     //преобразование

        //преобразование
        /*
         * тип преобразования и коэффициенты преобразования заполняются автоматически по полю _convertion;
         * Преобразование сигнала происходит по одной из формул в зависимости от _convertionType:
         * 0: без преобразование:                  y = x
         * 1: линейное:                            y = c0 + c1 * x
         * 2: полином 5-й степени:                 y = c0 + c1 * x + c2 * x^2 + c3 * x^3 + c4 * x^4 + c5 * x^5
         * 3: квадратный корень:                   y = c0 + c1 * sqrt(x + c2)
         * 4: экспанента:                          y = c0 + c1 * exp(c2 * x)
         * 5: кв. корень из полинома 5-й степени:  y = sqrt(c0 + c1 * x + c2 * x^2 + c3 * x^3 + c4 * x^4 + c5 * x^5)
        */
        public byte ConversionType { get; private set; }                //тип преобразования
        private readonly double[] _convCoef = new double[6];            //коэффициенты преобразования
        public double ConvCoef0 { get { return _convCoef[0]; } }
        public double ConvCoef1 { get { return _convCoef[1]; } }
        public double ConvCoef2 { get { return _convCoef[2]; } }
        public double ConvCoef3 { get { return _convCoef[3]; } }
        public double ConvCoef4 { get { return _convCoef[4]; } }
        public double ConvCoef5 { get { return _convCoef[5]; } }

        //отмеченный канал
        public bool Selected { get; set; }                              //выбор канала для записи в архив (опроса)

        //канал ТХС
        public abstract bool IsCjc { get; }                             //канал ТХС

        //значение
        public NewChannelValue ChannelValue { get; set; }               //значение //раньше было CurrentValue

        public DateTime? Time                                           //время опроса
        {
            get { return (ChannelValue != null) ? (DateTime?) ChannelValue.Time : null; }
        }
        public string Signal                                            //сигнал - строка, приходящая от модуля
        {
            get { return (ChannelValue != null) ? ChannelValue.Signal : null; }
        }
        public double? Val                                              //значение после первичной обработки (до преобразования)
        {
            get { return (ChannelValue != null) ? ChannelValue.Value : null; }
        }
        public double? CurrentValue                                     //значение (после преобразования), меняется не смотря на апертуру
        {
            get { return (ChannelValue != null) ? ChannelValue.CurrentValue : null; }
        }
        public double? Value                                            //значение (после преобразования) с учетом апертуры
        {
            get { return (ChannelValue != null) ? ChannelValue.Value : null; }
        }
        public NewEnumSignalStatus? Status                              //код ошибки сигнала или его обработки
        {
            get { return (ChannelValue != null) ? (NewEnumSignalStatus?)ChannelValue.Status : null; }
        }
        public bool? ValueChanged                                       //измененилось ли значение Value или Status
        {
            get { return (ChannelValue != null) ? (bool?)ChannelValue.ValueChanged : null; }
        }
        
    #endregion

    #region Properties
        public string Code
        {
            get { return _code; }
            set
            {
                if (value != null)
                    if (Module.Net != null)
                        if ((Module.Net.Channel(value) == null) && (!Regex.IsMatch(value, "^M[0-9A-F][0-9A-F]CH[0-9]+$")))
                            _code = value;
                        else
                            _code = value;
            }
        }

        public virtual string Conversion
        {
            get { return _conversion; }
            set
            {
                if (value != null)
                    if ((value.StartsWith("Расход(")) && (value.EndsWith(")")))
                    {
                        int i = "Расход(".Length;
                        string st = value.Substring(i, value.Length - i - 1).Replace(",", ".");
                        double coef;
                        if (double.TryParse(st, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out coef))
                        {
                            _conversion = "Расход(" + coef + ")";

                            ConversionType = 3;
                            _convCoef[0] = 0;
                            _convCoef[1] = coef;
                            for (i = 2; i <= 5; i++) _convCoef[i] = 0;

                            return;
                        }
                    }
                    else if ((value.StartsWith("Давление(")) && (value.EndsWith(")")))
                    {
                        int i = "Давление(".Length;
                        int j = value.IndexOf(";");

                        string st, st1;

                        if (j > 0)
                        {
                            st = value.Substring(i, j - i);
                            st1 = value.Substring(j + 1, value.Length - j - 2);
                        }
                        else
                        {
                            st = value.Substring(i, value.Length - i - 1);
                            st1 = "0";
                        }

                        st = st.Replace(",", ".");
                        st1 = st1.Replace(",", ".");

                        double coef, coef1;

                        if ((double.TryParse(st, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out coef)) &&
                            (double.TryParse(st1, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out coef1)))
                        {
                            _conversion = "Давление(" + coef + "; " + coef1 + ")";

                            ConversionType = 1;
                            _convCoef[0] = coef + coef1;
                            _convCoef[1] = 1;
                            for (i = 2; i <= 5; i++) _convCoef[i] = 0;

                            return;
                        }
                    }

                _conversion = null;

                ConversionType = 0;
                for (int i = 0; i <= 5; i++) _convCoef[i] = 0;

            }
        }
    #endregion

    #region Constructors/Destructors
        internal protected NewChannelAbstract(NewModuleAbstract module, byte channel)
        {
            _channel = channel;
            Module = module;

            string stBlank = (channel < 9) ? "0" : null;
            Code = "M" + Module.Address + "CH" + stBlank + (channel + 1);
            Name = null;
            Aperture = null;
            Min = null;
            Max = null;
            Units = null;
            InLevel = null;
            _conversion = null;
            Selected = true;

            ChannelValue = null;
        }

        internal protected NewChannelAbstract(NewModuleAbstract module, byte channel, string code) : this(module, channel)
        {
            Code = code;
        } 
    #endregion

    #region Function
        public abstract void CalcVal(string signal, ref NewEnumSignalStatus status, out double? val);

        public virtual double? SignalConversion(double value, out NewEnumSignalStatus status)
        {
            switch (ConversionType)
            {
                case 0:  // y = x
                    status = NewEnumSignalStatus.NoError;
                    return value;

                case 1: // y = c0 + c1 * x
                    try
                    {
                        status = NewEnumSignalStatus.NoError;
                        return ConvCoef0 + ConvCoef1 * value;
                    }
                    catch
                    {
                        status = NewEnumSignalStatus.ConversionCalcError;
                        return null;
                    }

                case 2: // y = c0 + c1 * x + c2 * x^2 + c3 * x^3 + c4 * x^4 + c5 * x^5
                    try
                    {
                        status = NewEnumSignalStatus.NoError;
                        return ConvCoef0 + ConvCoef1 * value + ConvCoef2 * Math.Pow(value, 2) +
                               ConvCoef3 * Math.Pow(value, 3) + ConvCoef4 * Math.Pow(value, 4) +
                               ConvCoef5 * Math.Pow(value, 5);
                    }
                    catch
                    {
                        status = NewEnumSignalStatus.ConversionCalcError;
                        return null;
                    }

                case 3: // y = c0 + c1 * sqrt(x + c2)
                    if (value >= 0)
                    {
                        try
                        {

                            status = NewEnumSignalStatus.NoError;
                            return ConvCoef0 + ConvCoef1 * Math.Sqrt(value + ConvCoef2);
                        }
                        catch
                        {
                            status = NewEnumSignalStatus.ConversionCalcError;
                            return null;
                        }
                    }
                    status = NewEnumSignalStatus.ConversionOutOfRange;
                    return null;

                case 4: // y = c0 + c1 * exp(c2 * x)
                    try
                    {
                        status = NewEnumSignalStatus.NoError;
                        return ConvCoef0 + ConvCoef1 * Math.Exp(ConvCoef2 * value);
                    }
                    catch
                    {
                        status = NewEnumSignalStatus.ConversionCalcError;
                        return null;
                    }

                case 5: // y = sqrt(c0 + c1 * x + c2 * x^2 + c3 * x^3 + c4 * x^4 + c5 * x^5)
                    try
                    {
                        var val = ConvCoef0 + ConvCoef1 * value + ConvCoef2 * Math.Pow(value, 2) +
                                  ConvCoef3 * Math.Pow(value, 3) + ConvCoef4 * Math.Pow(value, 4) +
                                  ConvCoef5 * Math.Pow(value, 5);
                        if (val >= 0)
                        {
                            status = NewEnumSignalStatus.NoError;
                            return Math.Sqrt(val);
                        }

                        status = NewEnumSignalStatus.ConversionOutOfRange;
                        return null;
                    }
                    catch
                    {
                        status = NewEnumSignalStatus.ConversionCalcError;
                        return null;
                    }

                default:
                    status = NewEnumSignalStatus.ConversionSyntaxError;
                    return null;
            }
        }
    #endregion
    }

    public class NewModuleChannels
    {
    #region Fields/Properties
        private readonly NewChannelAbstract[] _channels;

        public byte Count { get { return (byte) (_channels.GetUpperBound(0) + 1); } }
    #endregion

    #region Constructors/Destructors
        public NewModuleChannels(byte channelCount)
        {
            _channels = new NewChannelAbstract[channelCount];
        }
    #endregion

    #region ThisEnum
        public NewChannelAbstract this[byte channel]
        {
            get
            {
                if ((channel >= 0) && (channel < Count)) return _channels[channel];
                return null;
            }

            internal set
            {
                if ((channel < Count) && (_channels[channel] == null)) _channels[channel] = value;
            }
        }

        public IEnumerator GetEnumerator() { return _channels.GetEnumerator(); }
    #endregion
    }
}
