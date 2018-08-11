using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draw
{
    static class Processing
    {
        static public int q;
        static public double a1;
        static public double a2;
        static public double b;
        static public double T;
        static public bool checkLimit;
        static public Matrix lAns;
        static public Matrix x_0 = new Matrix(3, 1);
        static public string mess;
       
        static public int buttonChecked;

        static public bool checkString(string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8)
        {
            return (s1.Length != 0 && s2.Length != 0 && s3.Length != 0 && s4.Length != 0 && s5.Length != 0 && s6.Length != 0 && s7.Length != 0 && s8.Length != 0);
        }


        static public void function()
        {
            Program.x.Clear();
            Program.y.Clear();
            Program.xPrev.Clear();
            Program.yPrev.Clear();

            int k_var = (Program.k > 4) ? Program.k / 4 : Program.k;
            Matrix A = new Matrix(1, 1);
            Matrix B = new Matrix(1, 1);
            Matrix C = new Matrix(1, 1);
            Matrix E = new Matrix(1, 1);
            Matrix F = new Matrix(1, 1);
            Matrix G = new Matrix(1, 1);
            Matrix X = new Matrix(1, 1);
            lAns= new Matrix(1, 3);

            

            A.inputByFile("inputA.txt");
            B.inputByFile("inputB.txt");
            C.inputByFile("inputC.txt");
            E.inputByFile("inputE.txt");
            A.M[2][1] = -a1;
            A.M[2][2] = -a2;
            B.M[2][0] = b;

            F = F.make_F(A, T, q);
          
            G = G.make_G(A, B, T, q);

            checkLimit = Matrix.checkLimit(A, Processing.T);


            {
                Matrix.X_function_Y_const(F, G, T, C);
                if (buttonChecked == 2) minimizationJ_l2(F, G, T, C, B);
                else minimizationJ_l3(F, G, T, C, B);
                findXandYwithL_const(F, G, T, C, lAns);
            }

        }

        public static void findXandYwithL_const(Matrix F, Matrix G, double T, Matrix C, Matrix l)
        {
            Program.x.Clear();
            Program.y.Clear();
            Matrix temprorary_y = new Matrix(1, 1), temprorary_x = new Matrix(1, 1);
            Program.x.Add(Processing.x_0);


            for (int i = 0; i <= Program.k; i++)
            {
                temprorary_x = (F - ( G * l) ) * Program.x[i] + G;//*1
                Program.x.Add(temprorary_x);
                temprorary_y = C * Program.x[i];
                Program.y.Add(temprorary_y.M[0][0]);
            }
        }

        public static double J( Matrix F, Matrix G, double T, Matrix C, double l2, double l3)

        {
            Matrix vectorL =new Matrix(1, 3);
            vectorL.M[0][0] = 0.0;
            vectorL.M[0][1] = l2;
            vectorL.M[0][2] = l3;

            findXandYwithL_const( F,  G,  T,  C,  vectorL);
            double answer = 0.0;
            for (int i = 0; i < Program.k; i++)
            {
                answer += (Math.Abs(Program.x[i].M[0][0] - 1.0)) * T;
            }
            return answer;
        }

        public static void minimizationJ_l2(Matrix F, Matrix G, double T, Matrix C, Matrix B)
        {
            mess = "";
            
            int iter = 300; int i = 0;
            double eps=0.01;
            double max = x_0.M[0][0] > x_0.M[1][0] ? x_0.M[0][0] : x_0.M[1][0];
            max = max > x_0.M[2][0] ? max : x_0.M[2][0];
            if (max > 0.0) eps *= max;
            if (B.M[2][0] >= 0.7) eps *= B.M[2][0];
           
            double delta = 0.05;
            double l = 0.0;
           

            while (Math.Abs(J(F, G, T, C, 0.0, 0.0) - J(F, G, T, C, delta, 0.0)) <= eps)
                delta /= 2;


            double jPrev = J(F, G, T, C, 0.0, 0.0), jCurr = J(F, G, T, C, delta, 0.0);

            mess += " Eps = " + eps + "\n\r";
            mess += " Delta = " + delta + "\n\r";
            mess += " Istart = " + jPrev.ToString() + "\n\r";
            while (Math.Abs(jPrev - jCurr)>eps)
            {
                i++;
                jPrev = J(F, G, T, C, l, 0.0); jCurr = J(F, G, T, C, l + delta, 0.0);
                if ((J(F, G, T, C, l, 0.0) - J(F, G, T, C, l + delta, 0.0)) > 0.0) l += delta;
                else { delta *= -1.0; l += delta; }
                if (iter == i) break;
               
            }

            mess += " Iend = " + jCurr.ToString()+"\n\r";
            mess += " Amount of iter =" + i.ToString();
            Processing.lAns.M[0][1] = l;          

        }
        public static void minimizationJ_l3(Matrix F, Matrix G, double T, Matrix C, Matrix B)
        {
            mess = "";
            int iter = 300; int i = 0;
            double eps = 0.01;
            double max = x_0.M[0][0] > x_0.M[1][0] ? x_0.M[0][0] : x_0.M[1][0];
            max = max > x_0.M[2][0] ? max : x_0.M[2][0];
            if (max > 0.0) eps *= max;
            if (B.M[2][0] > 0.7) eps *= B.M[2][0];

            double delta = 0.05;
            double l = 0.0;

            while (Math.Abs(J(F, G, T, C, 0.0, 0.0) - J(F, G, T, C, delta, 0.0)) <= eps)
                delta /= 2;

            double jPrev = J(F, G, T, C, 0.0, 0.0), jCurr = J(F, G, T, C, 0.0, delta);

            mess += " Eps = " + eps + "\n\r";
            mess += " Delta = " + delta + "\n\r";
            mess += " Istart = " + jPrev.ToString() + "\n\r";
            while (Math.Abs(jPrev - jCurr) > eps)
            {
                i++;
                jPrev = J(F, G, T, C, 0.0, l); jCurr = J(F, G, T, C, 0.0, l + delta);
                if ((J(F, G, T, C, 0.0 ,l) - J(F, G, T, C, 0.0 , l + delta)) > 0.0) l += delta;
                else { delta *= -1.0; l += delta; }
                if (iter == i) break;
            }
            mess += " Iend = " + jCurr.ToString() + "\n\r";
            mess += " Amount of iter =" + i.ToString(); 
            Processing.lAns.M[0][2] = l;
        }

        public static void init_u(int k, int k_var, out int[] result)
        {
            result = new int[(int)k + 1];
            for (int i = 0; i < k + 1; i++)
            {
                if (((i - 1) / k_var) % 2 == 0)
                    result[i] = 1;
                else result[i] = -1;

            }
        }


        public static bool checkComponents(int q, double a1, double a2, double T)
        {
            return(((q<=10 && q>=1) &&( a1<=10 && a1>=1) &&(a2<=10 && a2>=1) && (T<=1 && T>=0.001)));
        }
    }
}
