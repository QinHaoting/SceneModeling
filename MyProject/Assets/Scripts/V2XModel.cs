using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public static class V2XModel{
    /**
     * 计算反射点数量
     */
    public static double NumsOfVRS(double Height, int type)
    {
        double M = 15;
        double M_max;
        double Height_max;

        switch (type)
        {
            case 0: // 低矮建筑物
                {
                    M_max = 15;
                    Height_max = 20;
                    break;
                }
            case 1: // 高大建筑物
                {
                    M_max = 50;
                    Height_max = 70;
                    break;
                }
            case 2: // 高速
                {
                    M_max = 9;
                    Height_max = 10;
                    break;
                }
            case 3: // 高架
                {
                    M_max = 20;
                    Height_max = 25;
                    break;
                }
            default:
                {
                    M_max = 20;
                    Height_max = 25;
                    break;
                }
        }
        if (Height <= Height_max)
        {
            M = M_max * Math.Sin(Math.PI / 2 * (Height / Height_max));
        }
        return M;
    }


    public static void H_NLOS(double _M, int type)
    {
        int M = (int)_M;
        double K = 10.0; // K值
        double fc = 5.9e9; // 载波频率
        double c = 3e8; // 光速
        double[] theta = new double[M]; // 随机相位
        double[] DpmiNLOS = new double[M]; // NLOS路径的距离差
        double mu = 0.5; // 随机增益
        Complex[] hNLOS = new Complex[M]; // NLOS路径信道系数

        System.Random rand = new System.Random();

        switch (type)
        {
            case 0: // 低矮建筑物
                {
                    K = 9.14;
                    mu = 0.271;
                    break;
                }
            case 1: // 高大建筑物
                {
                    K = 7.59;
                    mu = 0.185;
                    break;
                }
            case 2: // 高速
                {
                    K = 12.24;
                    mu = 0.305;
                    break;
                }
            case 3: // 高架
                {
                    K = 12.21;
                    mu = 0.230;
                    break;
                }
            default:
                {
                    K = 9.14;
                    mu = 0.271;
                    break;
                }
        }

        // 生成随机相位、距离差
        for (int i = 0; i < M; i++)
        {
            theta[i] = rand.NextDouble() * 2 * Math.PI;
            DpmiNLOS[i] = rand.NextDouble() * 100; // 假设距离差在0到100米之间
        }

        // 计算NLOS路径信道系数
        for (int i = 0; i < M; i++)
        {
            hNLOS[i] = mu * Complex.Exp(Complex.ImaginaryOne * (theta[i] - 2 * Math.PI * fc * DpmiNLOS[i] / c));
        }

        // 计算NLOS路径信道衰落
        Complex hNLOS_sum = new Complex(0, 0);
        for (int i = 0; i < M; i++)
        {
            hNLOS_sum += hNLOS[i];
        }
        Complex hNLOS_t = Complex.Sqrt(new Complex(1.0 / (K + 1), 0)) * hNLOS_sum / Math.Sqrt(M);
        Debug.Log("NLOS路径信道衰落：" + hNLOS_t.Real + " + " + hNLOS_t.Imaginary + "i");
    }
}
