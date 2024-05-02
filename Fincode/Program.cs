// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using System.Diagnostics;  
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Tes
{
    class Fincode
    {
        static void Main()
        {
            var sw = new Stopwatch();
            
            ///// LINQスタイルとfor文での速度比較をしている /////
            // 以下の状況の市場での原資産の現在価値の計算をしている //

            double ini = 100;     // はじめの価格
            double rate = 0.01;    // short rate
            double sigma = 0.3;    // volatility
            double timespan = 1.0; // 期間を1年

            int Mt = 10_000_000;   // pathの数

            // LINQによる実装 //
            sw.Start();
            double[] std_normal_array_linq = genStandard_normal(Mt);
            //price関数を用いて標準正規分布とパラメータから期待値を計算
            double price_linq = 
                std_normal_array_linq.Select(rand1 => price(ini, sigma, rate, timespan, rand1)).Average();
            sw.Stop();
            Console.WriteLine($"LINQ Calc Time: {sw.Elapsed}"); 
            Console.WriteLine(price_linq);
            sw.Reset();

            // for文による実装1:先に乱数配列を作る方法//
            sw.Start();
            double[] std_normal_array_for = genStandard_normal(Mt);
            double sum_for1 = 0.0;
            foreach (double rand in std_normal_array_for)
            {
                sum_for1 += price(ini, sigma, rate, timespan, rand);
            }
            double price_for1 = sum_for1 / Mt;
            sw.Stop();
            Console.WriteLine($"for1 sta Calc Time: {sw.Elapsed}"); 
            Console.WriteLine(price_for1);
            sw.Reset();

            // for文による実装2:全てfor文に入れる方法 //
            sw.Start();
            double sum_for2 = 0.0;
            Normal normalDist = new Normal();
            for (int i = 0; i < Mt; i++)
            {
                sum_for2 += price(ini, sigma, rate, timespan, normalDist.Sample());
            }
            double price_for2 = sum_for2 / Mt;
            sw.Stop();
            Console.WriteLine($"for2 sta Calc Time: {sw.Elapsed}"); 
            Console.WriteLine(price_for2);
            sw.Reset();
        }
        
        //BS式によるpricing関数
        static double price(double ini, double sigma, double r, double T, double rand)
        {
           return ini * Math.Exp( (r - 0.5 * sigma * sigma) * T + sigma * Math.Sqrt(T) * rand);
        }
        //標準正規分布によるサンプリングを指定の長さ分を配列で返す関数
        static double[] genStandard_normal(int ind)
        {
            Normal normalDist = new Normal();
            var randomArray = new double[ind];
            for (int i = 0; i < ind; i++)
            {
                randomArray[i] = normalDist.Sample();
            }
            return randomArray;
        }
    }
}