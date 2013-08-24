using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;
using System.IO;


namespace FastqAnalyzerCleaner
{
    public class ProtocolBuffersSerialization
    {
        public static String PROTOBUF_FILE_EXTENSION = ".fqprotobin";
        public static String PROTOBUF_FILE_WIDCARD = "*.fqprotobin";

        public ProtocolBuffersSerialization()
        {

        }

        public static FqSequence ProtobufDerialize(String fileName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                FqSequence o;
                using (var file = File.OpenRead(fileName))
                {
                    o = Serializer.Deserialize<FqSequence>(file);
                }
                sw.Stop();
                Console.WriteLine("Protobuf Deserialization in time: {0} of filename: {1}", sw.Elapsed, fileName);
                return o;
            }
            catch (System.Runtime.Serialization.SerializationException exception)
            {
                Console.WriteLine("Protobuf Deserialization Failed: {0}", exception.StackTrace);
                return null;
            }
            catch (IOException exception)
            {
                Console.WriteLine("Protobuf Deserialization Failed: {0}", exception.StackTrace);
                return null;
            }
        }

        public static Boolean ProtobufSerialize(FqSequence o, String fileName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                using (var file = File.Create(fileName))
                {
                    Serializer.Serialize<FqSequence>(file, o);
                    sw.Stop();
                    Console.WriteLine("Protobuf Serilization Time: {0} of Filename: {1} ", sw.Elapsed, fileName);
                    return true;
                }
            }
            catch (IOException exception)
            {
                Console.WriteLine("Protobuf Serialization Failed: {0}", exception.StackTrace);
            }
            catch (System.Runtime.Serialization.SerializationException exception)
            {
                Console.WriteLine("Protobuf Serialization Failed: {0}", exception.StackTrace);
            }
            return false;
        }

        public delegate FqFile_Component ProbufDeserializeFqFile_AsyncMethodCaller(String fileName, out int threadId);
        public FqFile_Component ProtobufDerializeFqFile(String fileName, out int threadId)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            threadId = Thread.CurrentThread.ManagedThreadId;

            try
            {
                FqFile_Component fq;
                using (var file = File.OpenRead(fileName))
                {
                    fq = Serializer.Deserialize<FqFile_Component>(file);
                }
                sw.Stop();
                Console.WriteLine("Protobuf Deserialization in time: {0} of File: {1} On Thread: {2}", sw.Elapsed, fileName, threadId);
                return fq;
            }
            catch (System.Runtime.Serialization.SerializationException exception)
            {
                Console.WriteLine("Protobuf Deserialization Failed: {0}", exception.StackTrace);
            }
            catch (IOException exception)
            {
                Console.WriteLine("Protobuf Deserialization Failed: {0}", exception.StackTrace);
            }
            return null;
        }

        public delegate Boolean ProbufSerializeFqFile_AsyncMethodCaller(FqFile_Component fq, String fileName, out int threadId);
        public Boolean ProtobufSerializeFqFile(FqFile_Component fq, String fileName, out int threadId)
        {
            Stopwatch sw = new Stopwatch();
            threadId = Thread.CurrentThread.ManagedThreadId;
            sw.Start();
            try
            {
                using (var file = File.Create(fileName))
                {
                    Serializer.Serialize<FqFile_Component>(file, fq);
                    sw.Stop();
                    Console.WriteLine("Protobuf Serilization Time: {0} of File: {1} On Thread: {2}", sw.Elapsed, fileName, threadId);
                    return true;
                }
            }
            catch (IOException exception)
            {
                Console.WriteLine("Protobuf Serialization Failed: {0}", exception.StackTrace);
            }
            catch (System.Runtime.Serialization.SerializationException exception)
            {
                Console.WriteLine("Protobuf Serialization Failed: {0}", exception.StackTrace);
            }
            return false;
        }
    }
}
