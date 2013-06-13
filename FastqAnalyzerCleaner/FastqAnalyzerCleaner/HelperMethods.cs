/// <copyright file="HelperMethods.cs" author="Neil Robertson">
/// Copyright (c) 2013 All Right Reserved, Neil Alistair Robertson - neil.alistair.robertson@hotmail.co.uk
///
/// This code is the property of Neil Robertson.  Permission must be sought before reuse.
/// It has been written explicitly for the MRes Bioinfomatics course at the University 
/// of Glasgow, Scotland under the supervision of Derek Gatherer.
///
/// </copyright>
/// <author>Neil Robertson</author>
/// <email>neil.alistair.robertson@hotmail.co.uk</email>
/// <date>2013-06-1</date>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    static class HelperMethods
    {
       
        public static Boolean safeParseInt(String str)
        {
            try
            {
                Int32.Parse(str);
                return true;
            }
            catch (FormatException exception)
            {
                Console.WriteLine(exception.StackTrace);
                return false;
            }
        }

        public static Boolean safeParseDouble(String str)
        {
            try
            {
                Double.Parse(str);
                return true;
            }
            catch (FormatException exception)
            {
                Console.WriteLine(exception.StackTrace);
                return false;
            }
        }

        public static Boolean safeParseLong(String str)
        {
            try
            {
                long.Parse(str);
                return true;
            }
            catch (FormatException exception)
            {
                Console.WriteLine(exception.StackTrace);
                return false;
            }
        }

        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        public static double ConvertKilobytesToMegabytes(long kilobytes)
        {
            return kilobytes / 1024f;
        }

        public static TResult With<TInput, TResult>(this TInput o,
        Func<TInput, TResult> evaluator)
        where TResult : class
        where TInput : class
        {
            if (o == null) return null;
            return evaluator(o);
        }

        public static TResult Return<TInput, TResult>(this TInput o,
        Func<TInput, TResult> evaluator, TResult failureValue) where TInput : class
        {
            if (o == null) return failureValue;
            return evaluator(o);
        }


        public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator)
        where TInput : class
        {
            if (o == null) return null;
            return evaluator(o) ? o : null;
        }

        public static TInput Unless<TInput>(this TInput o, Func<TInput, bool> evaluator)
               where TInput : class
        {
            if (o == null) return null;
            return evaluator(o) ? null : o;
        }

        public static TInput Do<TInput>(this TInput o, Action<TInput> action)
        where TInput : class
        {
            if (o == null) return null;
            action(o);
            return o;
        }

        
    }
}
